Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants

Namespace Biosystems.Ax00.BL
    Public Class CalculatedTestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Calculated Test (basic data, formula members and reference ranges)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTest">Typed DataSet CalculatedTestsDS with data of the Calculated Test to add</param>
        ''' <param name="pFormulaValues">Typed DataSet FormulasDS containing all values included in the formula defined
        '''                              for the Calculated Test</param>
        ''' <param name="pRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges defined for the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with all data of the added Calculated Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 22/06/2010 - Changed the function logic and call AddFormula instead of Create
        '''              SA 16/12/2010 - Added parameter for the DS containing the Reference Ranges. Call function SaveReferenceRanges
        '''                              to save the Reference Ranges for the Calculated Test
        '''              SA 14/01/2011 - Removed validation of duplicated Name or ShortName; it is done from the screen now
        '''              AG 01/09/2014 - BA-1869 when new CALC test is created the CustomPosition informed = MAX current value + 1
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTest As CalculatedTestsDS, ByVal pFormulaValues As FormulasDS, _
                            ByVal pRefRanges As TestRefRangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Insert the new Calculated Test 
                        Dim calTestToAdd As New tparCalculatedTestsDAO

                        'AG 01/09/2014 - BA-1869 new calc test customposition value = MAX current value + 1
                        resultData = calTestToAdd.GetLastCustomPosition(dbConnection)
                        If Not resultData.HasError Then
                            If resultData.SetDatos Is Nothing OrElse resultData.SetDatos Is DBNull.Value Then
                                pCalcTest.tparCalculatedTests(0).CustomPosition = 1
                            Else
                                pCalcTest.tparCalculatedTests(0).CustomPosition = DirectCast(resultData.SetDatos, Integer) + 1
                            End If
                            'AG 01/09/2014 - BA-1869

                            resultData = calTestToAdd.Create(dbConnection, pCalcTest)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                'Get the generated CalcTestID from the dataset returned 
                                Dim generatedID As Integer = -1
                                generatedID = DirectCast(resultData.SetDatos, CalculatedTestsDS).tparCalculatedTests(0).CalcTestID

                                'Set value of Calculated Test ID in the dataset containing the Formula Values
                                For i As Integer = 0 To pFormulaValues.tparFormulas.Rows.Count - 1
                                    pFormulaValues.tparFormulas(i).CalcTestID = generatedID
                                Next i

                                'Set value of Calculated Test ID in the dataset containing the Reference Ranges
                                For i As Integer = 0 To pRefRanges.tparTestRefRanges.Rows.Count - 1
                                    pRefRanges.tparTestRefRanges(i).TestID = generatedID
                                Next

                                'Insert the Formula Values
                                Dim testFormulaToAdd As New FormulasDelegate
                                resultData = testFormulaToAdd.AddFormula(dbConnection, pFormulaValues, True)

                                If (Not resultData.HasError) Then
                                    'Finally, insert the Reference Ranges
                                    If (pRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                        resultData = SaveReferenceRanges(dbConnection, pRefRanges)
                                    End If
                                End If
                            End If

                        End If 'AG 01/09/2014 - BA-1869

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'The added Calculated Test is returned (without Formula)
                            resultData.SetDatos = pCalcTest
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all the specified Calculated Tests (Test Definition, Formula and Reference Ranges).
        ''' Additionally, it deletes all related Calculated Tests that contain these specified Calculated Tests in their formulas.
        ''' Detailled functionality:
        ''' *Step 1: For all Calculated Tests that have the Calculated Tests from step 2 included in their formula: perform the 6 actions stated below:
        '''    1. Mark as DeletedFlag if exists into a saved Worksession from LIS.
        '''    2. Remove the Calculated Test from all Test Profiles in which it is included.
        '''    3. Delete Reference Ranges for the affected Calculated Test.
        '''    4. Delete the elements of the Formula of the affected Calculated Test.
        '''    5. Delete the affected Calculated Test.
        '''    6. If the affected CalculatedTest exists in Historic Module, mark it as closed.
        ''' *Step 2: For all affected Calculated Tests: also perform the 6 actions stated above.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTests">Typed DataSet CalculatedTestsDS with the list of Calculated Tests to delete</param>        
        ''' <returns>GlobalDataTO containing a CalculatedTestsDS with all Calculated Tests also deleted due to the removed one was part of their Formulas</returns>
        ''' <remarks>
        ''' Modified by: SA 22/06/2010 - Changed function logic to allow deletion of several Calculated Tests. Removed 
        '''                              Concurrency Error control. After deleting a Calculated Test, set EnableStatus=False
        '''                              for all Calculated Tests having the deleted one included in their Formula 
        '''              TR 25/11/2010 - For each Calculated Test to delete, search if it is included in the Formula of another 
        '''                              Calculated Tests to delete also those affected elements and remove them of all Profiles 
        '''                              in which they are included. Due to related elements are now deleted, the setting of
        '''                              EnableStatus is not needed anymore
        '''              TR 25/11/2010 - For each Calculated Test to delete, remove it from all Profiles in which it is included.
        '''              SA 17/12/2010 - For each Calculated Test to delete, remove also its Reference Ranges
        '''              TR 04/09/2012 - Add call to function HIST_CloseCalculatedTest to mark the Calculated Test as closed on Historic Module
        '''              SA 13/09/2012 - If the deleted Calculated Test is included in the Formula of another Calculated Test and this exists 
        '''                              in Historic Module, then it is also marked as closed
        '''              XB 18/02/2013 - Fix the use of parameter pTestType which is not used in this function nowadays (BugsTracking #1136)
        '''              AG 10/05/2013 - Mark as deleted test if this test form part of a LIS saved worksession
        '''              SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Return the CalculatedTestsDS containing all Calculated Tests also deleted
        '''                                                            due to the removed one was part of their Formulas
        '''              WE 24/11/2014 - RQ00035C (BA-1867): Updated Summary description.
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTests As CalculatedTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTempCalcTestDS As New CalculatedTestsDS
                        Dim myFormulaDelegate As New FormulasDelegate
                        Dim myCalcTestsDAO As New tparCalculatedTestsDAO
                        Dim myTestProfileDelegate As New TestProfilesDelegate
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate
                        Dim mySavedWS As New SavedWSOrderTestsDelegate

                        For Each calculatedTest As CalculatedTestsDS.tparCalculatedTestsRow In pCalcTests.tparCalculatedTests.Rows
                            'Search if the Calculated Test is included in the Formula of another Calculated Test
                            resultData = GetRelatedCalculatedTest(dbConnection, calculatedTest.CalcTestID, "CALC") 'AG 04/09/2014 - BA-1869 add new parameter TESTType
                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                myTempCalcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                For Each tempCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow In myTempCalcTestDS.tparCalculatedTests.Rows
                                    'Mark as DeletedFlag if exists into a saved Worksession from LIS
                                    resultData = mySavedWS.UpdateDeletedTestFlag(dbConnection, "CALC", tempCalcTestRow.CalcTestID)
                                    If resultData.HasError Then Exit For

                                    'Remove the Calculated Test from all Profiles in which it is included 
                                    resultData = myTestProfileDelegate.DeleteByTestIDSampleType(dbConnection, tempCalcTestRow.CalcTestID, "", "CALC")
                                    If (resultData.HasError) Then Exit For

                                    'Delete Reference Ranges for the affected Calculated Test
                                    resultData = myTestRefRangesDelegate.DeleteByTestID(dbConnection, tempCalcTestRow.CalcTestID, "", "CALC")
                                    If (resultData.HasError) Then Exit For

                                    'Delete the elements of the Formula of the affected Calculated Test
                                    resultData = myFormulaDelegate.Delete(dbConnection, tempCalcTestRow.CalcTestID)
                                    If (resultData.HasError) Then Exit For

                                    'Delete the affected Calculated Test
                                    resultData = myCalcTestsDAO.Delete(dbConnection, tempCalcTestRow.CalcTestID)
                                    If (resultData.HasError) Then Exit For

                                    If (HISTWorkingMode) Then
                                        'If the affected CalculatedTest exists in Historic Module, mark it as closed
                                        resultData = HIST_CloseCalculatedTest(dbConnection, tempCalcTestRow.CalcTestID)
                                        If (resultData.HasError) Then Exit For
                                    End If
                                Next

                                'It was error in one of the executed deletion; process is aborted 
                                If (resultData.HasError) Then Exit For
                            Else
                                'Error getting the list of affected Calculated Tests; deletion process is aborted
                                Exit For
                            End If

                            'Mark as DeletedFlag if exists into a saved Worksession from LIS
                            resultData = mySavedWS.UpdateDeletedTestFlag(dbConnection, "CALC", calculatedTest.CalcTestID)
                            If resultData.HasError Then Exit For

                            'Remove the Calculated Test from all Profiles in which it is included 
                            resultData = myTestProfileDelegate.DeleteByTestIDSampleType(dbConnection, calculatedTest.CalcTestID, "", "CALC")
                            If resultData.HasError Then Exit For

                            'Delete Reference Ranges for the affected Calculated Test
                            resultData = myTestRefRangesDelegate.DeleteByTestID(dbConnection, calculatedTest.CalcTestID, "", "CALC")
                            If (resultData.HasError) Then Exit For

                            'Delete the elements of the Calculated Test Formula
                            resultData = myFormulaDelegate.Delete(dbConnection, calculatedTest.CalcTestID)
                            If (resultData.HasError) Then Exit For

                            'Delete the Calculated Test
                            resultData = myCalcTestsDAO.Delete(dbConnection, calculatedTest.CalcTestID)
                            If (resultData.HasError) Then Exit For

                            If (HISTWorkingMode) Then
                                'If the CalculatedTest exists in Historic Module, mark it as closed
                                resultData = HIST_CloseCalculatedTest(dbConnection, calculatedTest.CalcTestID)
                                If (resultData.HasError) Then Exit For
                            End If
                        Next

                        If (Not resultData.HasError) Then
                            'Return the CalculatedTestsDS containing the Calculated Test indirectly affected...
                            resultData.SetDatos = myTempCalcTestDS

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Calculated Tests in which the informed Test Identifier (it can be the ID of a Standard, ISE, Off-System or Calculated Test) is 
        ''' included in their Formula. Additionally, it deletes all related Calculated Tests that contain these affected Calculated Tests in their formulas.
        ''' Also it performs some Calculated Test related actions regarding Test Profiles, Reference Ranges, Calc.Test Formulas, Historic Module and LIS Saved WS.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Optional parameter. When informed, it indicates the Test will be searched in the defined Formulas 
        '''                           linked to the specified SampleType</param>
        ''' <param name="pTestType">Optional parameter. The informed ID corresponds to a Standard, ISE, Off-system or Calculated Test (STD,ISE,OFFS,CALC).</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 17/05/2010
        ''' Modified by: TR 25/11/2010 - Name changed to DeleteCalculatedTestByTestID due to the functionality was changed to implement delete 
        '''                              instead of disabling
        '''              SA 13/09/2012 - Instead of calling functions GetCalcTest and Delete for each Calculated Test in which the informed 
        '''                              TestType/TestID/SampleType is included, load all the IDs in a CalculatedTestsDS and finally call Delete function
        '''              WE 24/11/2014 - RQ00035C (BA-1867): Updated Summary and Parameters description.
        ''' </remarks>
        Public Function DeleteCalculatedTestbyTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") _
                                                     As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        ' Search in the Formula table if there is/are any record(s) with the informed TestType/TestID/SampleType and return 
                        ' the ID of all Calculated Tests in which the informed TestType/TestID/SampleType is included.
                        Dim myFormulaDelegate As New FormulasDelegate
                        myGlobalDataTO = myFormulaDelegate.ReadFormulaByTestID(dbConnection, pTestID, pSampleType, pTestType)

                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myFormulasDS As FormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                            'Load the ID of all returned Calculated Tests in a CalculatedTestsDS
                            Dim tempCalcTestDS As New CalculatedTestsDS
                            Dim myCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow

                            Dim myCalculatedTestDAO As New tparCalculatedTestsDAO
                            For Each formulaRow As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                                myCalcTestRow = tempCalcTestDS.tparCalculatedTests.NewtparCalculatedTestsRow()
                                myCalcTestRow.CalcTestID = formulaRow.CalcTestID
                                tempCalcTestDS.tparCalculatedTests.AddtparCalculatedTestsRow(myCalcTestRow)

                                'myGlobalDataTO = GetCalcTest(dbConnection, formulaRow.CalcTestID)
                                'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                '    tempCalTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                                '    myGlobalDataTO = Delete(dbConnection, tempCalTestDS)
                                '    If (myGlobalDataTO.HasError) Then Exit For
                                'Else
                                '    Exit For
                                'End If
                            Next

                            If (tempCalcTestDS.tparCalculatedTests.Rows.Count > 0) Then
                                ' If there is at least 1 affected Calculated Test => Delete all affected Calculated Tests and perform related actions.
                                myGlobalDataTO = Delete(dbConnection, tempCalcTestDS)
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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.DeleteCalculatedTestbyTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search test data for the informed Test Name - used for LIMS import process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestName">Calculated Test Name to search by</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS containing data of the 
        '''          informed Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2010
        ''' </remarks>
        Public Function ExistsCalcTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestName As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparCalculatedTestsDAO
                        resultData = myDAO.ReadByCalcTestName(dbConnection, pCalcTestName)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.ExistsCalcTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is already a Calculated Test with the informed Calculated Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalTestName">Calculated Test Name to be validated</param>
        ''' <param name="pNameToSearch">Value indicating which is the name to validate: the short name or the long one</param>
        ''' <param name="pCalTestID">Calculated Test Identifier. It is an optional parameter informed
        '''                          only in case of updation</param>
        ''' <param name="pReturnBoolean">Flag indicating the type of value to return inside the GlobalDataTO. When TRUE (default value),
        '''                              the function returns True/False; when FALSE, the function returns the obtained CalculatedTestsDS</param>
        ''' <returns>If pReturnBoolean = TRUE ==> GlobalDataTO containing a boolean value: True if there is another Calculated Test with the same 
        '''                                       name; otherwise, False
        '''          If pReturnBoolean = FALSE ==> GlobalDataTO containing the obtained CalculatedTestsDS</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/10/2010 - Added N preffix for multilanguage when comparing by fields CalcTestName or CalcTestLongName
        '''              SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Added new optional parameter pReturnBoolean with default value TRUE.
        '''                                                            When its value is FALSE, instead of return True/False, the function
        '''                                                            will return the obtained CalculatedTestsDS inside the GlobalDataTO
        ''' </remarks>
        Public Function ExistsCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalTestName As String, _
                                             ByVal pNameToSearch As String, Optional ByVal pCalTestID As Integer = 0, _
                                             Optional ByVal pReturnBoolean As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calTestToUpdate As New tparCalculatedTestsDAO
                        resultData = calTestToUpdate.ExistsCalculatedTest(dbConnection, pCalTestName, pNameToSearch, pCalTestID, pReturnBoolean)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.ExistsCalcTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get list of Standard/ISE/Off-System Tests or Calculated Tests that can be included in the Formula defined for a Calculated Test.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Only selects Tests of type <pTestType></pTestType>. Allowed values are: (STD, ISE, OFFS, STD_ISE_OFFS, CALC)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AllowedTestsDS containing all Tests of the specified type that can be used in the 
        '''          Formula of a Calculated Test.</returns>
        ''' <remarks>
        ''' Modified by: SA 22/06/2010 - Some errors fixed. Changed the way of getting the Test Icons. Unify functions 
        '''                              GetAllowedTestList, GetAllowedCalcList and GetAllowedStandardTestList: the three functions
        '''                              do the same and only GetAllowedCalcList was used
        '''              WE 07/11/2014 - RQ00035C (BA-1867).
        ''' </remarks>
        Public Function GetAllowedTestList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of Tests by Test Type
                        Dim myDAO As New tparCalculatedTestsDAO
                        resultData = myDAO.ReadAllowedTestList(dbConnection, pTestType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myAllowedTests As AllowedTestsDS = DirectCast(resultData.SetDatos, AllowedTestsDS)

                            'Get the Icon Path in Application Configuration file
                            Dim systemTestsIconPath As String = String.Empty
                            Dim userTestsIconPath As String = String.Empty
                            Dim calcTestsIconPath As String = String.Empty
                            Dim ISETestsIconPath As String = String.Empty
                            Dim offSystemTestsIconPath As String = String.Empty

                            Dim resultDS As New PreloadedMasterDataDS
                            Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate

                            If (pTestType = "STD" OrElse pTestType = "STD_ISE_OFFS") Then
                                ' (1) Get Icons for Standard Tests.
                                ' 1.1 - Get the Icons needed for Standard Tests (Biosystems Tests)
                                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TESTICON")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        systemTestsIconPath = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If

                                ' 1.2 - Get the Icons needed for Standard Tests (User defined Tests)
                                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "USERTEST")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        userTestsIconPath = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If
                            End If

                            If (pTestType = "CALC") Then
                                ' (2) Get the Icon needed for Calculated Tests.
                                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TCALC")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        calcTestsIconPath = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If
                            End If

                            ' Get the icon path for ISE {GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS = "TISE_SYS"}.
                            If (pTestType = "ISE" Or pTestType = "STD_ISE_OFFS") Then
                                ' (3) Get Icon for ISE Tests.
                                ' There are only Factory-defined ISE Tests.
                                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TISE_SYS")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        ISETestsIconPath = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If
                            End If

                            ' Get the icon path for Off-System {GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS = "TOFF_SYS"}.
                            If (pTestType = "OFFS" Or pTestType = "STD_ISE_OFFS") Then
                                ' (4) Get Icon for Off-System Tests.
                                ' Assumption: there is no separation between Factory and User-defined Off-System Tests icon.
                                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TOFF_SYS")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        offSystemTestsIconPath = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If
                            End If

                            'Inform the correspondent Icon for each returned Test
                            If (Not resultData.HasError) Then
                                For Each allowedTestRow As AllowedTestsDS.tparAllowedTestsRow In myAllowedTests.tparAllowedTests.Rows
                                    allowedTestRow.BeginEdit()
                                    If (allowedTestRow.TestTypeCode = "STD") Then
                                        If (allowedTestRow.PreloadedTest) Then
                                            allowedTestRow.IconPath = systemTestsIconPath
                                        Else
                                            allowedTestRow.IconPath = userTestsIconPath
                                        End If
                                    ElseIf (allowedTestRow.TestTypeCode = "CALC") Then
                                        allowedTestRow.IconPath = calcTestsIconPath

                                    ElseIf (allowedTestRow.TestTypeCode = "ISE") Then
                                        allowedTestRow.IconPath = ISETestsIconPath

                                    ElseIf (allowedTestRow.TestTypeCode = "OFFS") Then
                                        ' Assumption: there is no separation between Factory and User-defined Off-System Tests icon.
                                        allowedTestRow.IconPath = offSystemTestsIconPath

                                    End If
                                    allowedTestRow.EndEdit()
                                Next
                                resultData.SetDatos = myAllowedTests
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetAllowedTestList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ' ''' <summary>
        ' ''' Look up the Icon path for a specific Test from the Preloaded Masterdata table.
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pTestType">Only selects Tests of type <pTestType></pTestType>. Allowed values are: (STD, ISE, OFFS, STD_ISE_OFFS, CALC)</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet AllowedTestsDS containing all Tests of the specified type that can be used in the 
        ' '''          Formula of a Calculated Test.</returns>
        ' ''' <remarks>
        ' ''' Modified by: WE 10/11/2014 - RQ00035C (BA-1867).
        ' ''' </remarks>
        'Private Function GetIconPath(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Dim resultDS As New PreloadedMasterDataDS
        '    Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                ' Get the Icon path for the specific Tests.
        '                resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TESTICON")
        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
        '                    If (resultDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
        '                        resultData.SetDatos = resultDS.tfmwPreloadedMasterData(0).FixedItemDesc
        '                    End If
        '                End If

        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetAllowedTestList", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function





        ''' <summary>
        ''' Get the list of all defined Calculated Tests using the specified SampleType (or having at least
        ''' a Test using the specified SampleType in the formula)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with data of the CalculatedTests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalculatedTests As New tparCalculatedTestsDAO
                        resultData = myCalculatedTests.ReadBySampleType(dbConnection, pSampleType, pCustomizedTestSelection) 'AG 29/08/2014 BA-1869 use new parameter
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pID">Unique Calculated Test Identifier (CalcTestID or BiosystemsID)</param>
        ''' <param name="pSearchByBiosystemsID">When TRUE, the search is executed by field BiosystemsID instead of by field CalcTestID.
        '''                                     Optional parameter with FALSE as default value</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with data of the specified Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  SA 07/05/2010
        ''' Modified by: SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Added optional parameter pSearchByBiosystemsID to allow search the
        '''                                                            Calculated Test by BiosystemsID instead of by CalcTestID 
        '''                                                            (needed in UpdateVersion process)
        ''' </remarks>
        Public Function GetCalcTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pID As Integer, _
                                    Optional ByVal pSearchByBiosystemsID As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalculatedTests As New tparCalculatedTestsDAO
                        resultData = myCalculatedTests.Read(dbConnection, pID, pSearchByBiosystemsID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetCalcTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Calculated Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with the list of all defined
        '''          Calculated Tests</returns>
        ''' <remarks></remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO 'CalculatedTestsDS
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of defined Calculated Test
                        Dim myCalculatedTests As New tparCalculatedTestsDAO
                        resultData = myCalculatedTests.ReadAll(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim resultDS As CalculatedTestsDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                            If (resultDS.tparCalculatedTests.Rows.Count > 0) Then
                                For Each calTestRow As CalculatedTestsDS.tparCalculatedTestsRow In resultDS.tparCalculatedTests.Rows
                                    calTestRow.BeginEdit()
                                    calTestRow.IconPath = IIf(calTestRow.InUse, "INUSETCALC", "TCALC").ToString
                                    calTestRow.EndEdit()
                                Next
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Calculated Tests in which Formula the informed Calculated Test is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test to search in formulas</param>
        ''' <param name="pTestType">Type type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with the list of related Calculated Tests</returns>
        ''' <remarks>
        ''' Created by:  TR 25/11/2010
        ''' AG 04/09/2014 - BA-1869 add parameter testtype
        ''' </remarks>
        Public Function GetRelatedCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalculatedTests As New tparCalculatedTestsDAO
                        resultData = myCalculatedTests.GetRelatedCalculatedTest(dbConnection, pCalcTestID, pTestType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.GetRelatedCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Modify an exiting Calculated Test (basic data and formula members)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestDS">Typed DataSet CalculatedTestsDS with data of the Calculated Test to update</param>
        ''' <param name="pFormulaValuesDS">Typed DataSet FormulasDS containing all values included in the formula defined for the Calculated Test</param>
        ''' <param name="pRefRangesDS">Typed DataSet TestRefRangesDS containing all Reference Ranges defined for the Calculated Test</param>
        ''' <param name="pHaveDependencies">Optional parameter. When TRUE, it indicates it is needed to verify if there are Profiles and/or other
        '''                                 Calculated Tests affected by the data updated (mainly regarding the change of the SampleType)</param>
        ''' <param name="pUpdateHistory">Optional parameter. It is set to TRUE when fields CalcTestLongName, MeasureUnit and/or DecimalsAllowed 
        '''                               are updated, and in this case, it is needed to verify if the Calculated Test exists in Historic Module to 
        '''                               update the data also in it</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with all data of the added Calculated Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/06/2010
        ''' Modified by: SA 16/12/2010 - Added parameter for the DS containing the Reference Ranges. Call function SaveReferenceRanges
        '''                              to save the Reference Ranges for the Calculated Test
        '''              TR 13/01/2011 - If the changes in the specified Calculated Test affect other elements, then update/delete all of them
        '''              SA 14/01/2011 - Added updation/deletion for affected Calculated Tests and the Profiles in which they are included.
        '''                              Removed validation of duplicated Name or ShortName; it is done from the screen now
        '''              TR 04/09/2012 - Added optional parameter pUpdateHistory to indicate if data of the Calculated Test has to be updated in Historic Module
        '''                            - Added optional parameter pCloseHistory to indicate if the Calculated Test has to be marked as closed in Historic Module
        '''                            - Depending on value of the added parameters call function to update data or mark the Test as closed in Historic Module
        '''              SA 17/09/2012 - Removed parameter pCloseHistory and its related functionality; it is not needed due to function HIST_UpdateByCalcTestID
        '''                              verifies if field FormulaText has been changed and in this case close the Calculated Test
        '''              SA 20/09/2012 - Added call to function UpdateFormulaText to verify if it is needed to update the FormulaText field of affected
        '''                              Calculated Tests
        '''              XB 18/02/2013 - Fix the use of parameter pTestType which is not used in this function nowadays (BugsTracking #1136)
        ''' </remarks>
        Public Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestDS As CalculatedTestsDS, ByVal pFormulaValuesDS As FormulasDS, _
                               ByVal pRefRangesDS As TestRefRangesDS, Optional ByVal pHaveDependencies As Boolean = False, _
                               Optional ByVal pUpdateHistory As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update basic data of the specified Calculated Test
                        Dim calTestToUpdate As New tparCalculatedTestsDAO

                        resultData = calTestToUpdate.Update(dbConnection, pCalcTestDS)
                        If (Not resultData.HasError) Then
                            'DL 08/02/2013. Begin
                            'Update the Formula members of the specified Calculated Test
                            If pFormulaValuesDS.tparFormulas.Rows.Count > 0 Then
                                Dim myFormulaDelegate As New FormulasDelegate
                                resultData = myFormulaDelegate.AddFormula(dbConnection, pFormulaValuesDS, False)
                            End If
                            'DL 08/02/2013. End
                        End If

                        If (Not resultData.HasError) Then
                            'Finally, update the Reference Ranges
                            If (pRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                                resultData = SaveReferenceRanges(dbConnection, pRefRangesDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Update/remove all affected elements
                            If (pHaveDependencies) Then
                                'Get all Sample Types of the Tests included in the Formula defined for the Calculated Test
                                Dim qFormulaList As List(Of String) = (From a In pFormulaValuesDS.tparFormulas _
                                                                      Where a.ValueType = "TEST" _
                                                                     Select a.SampleType).Distinct.ToList()

                                'Build a String List containing all different SampleTypes linked by commas
                                Dim mySampleTypeList As String = ""
                                For Each diffSampleType As String In qFormulaList
                                    mySampleTypeList &= "'" & diffSampleType & "',"
                                Next

                                'Finally, remove the last comma 
                                If (mySampleTypeList.Length > 0) Then
                                    mySampleTypeList = mySampleTypeList.Remove(mySampleTypeList.Count - 1, 1)
                                End If

                                'Get affected Profiles and remove the Calculated Test from all of them
                                Dim myTestProfileTestDelegate As New TestProfileTestsDelegate
                                resultData = myTestProfileTestDelegate.ReadByTestIDSpecial(dbConnection, pCalcTestDS.tparCalculatedTests(0).CalcTestID, _
                                                                                           mySampleTypeList, "CALC")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myTestProfileTestDS As TestProfileTestsDS = DirectCast(resultData.SetDatos, TestProfileTestsDS)

                                    For Each testProfileTestRow As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                                        resultData = myTestProfileTestDelegate.DeleteByTestIDAndTestProfileID(dbConnection, testProfileTestRow.TestID, _
                                                                                                              testProfileTestRow.TestProfileID, "CALC")
                                        If (resultData.HasError) Then Exit For
                                    Next
                                End If

                                'Get all affected Calculated Tests
                                Dim myFormulasDelegate As New FormulasDelegate
                                resultData = myFormulasDelegate.ReadFormulaByTestID(dbConnection, pCalcTestDS.tparCalculatedTests(0).CalcTestID, _
                                                                                    mySampleTypeList, "CALC", True)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myFormulasDS As FormulasDS = DirectCast(resultData.SetDatos, FormulasDS)

                                    Dim myTestProfileTestsDS As TestProfileTestsDS
                                    Dim myCalcTestsDAO As New tparCalculatedTestsDAO
                                    Dim myTestRefRangesDelegate As New TestRefRangesDelegate

                                    For Each formRow As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                                        'Search all Profiles in which the affected Calculated Test is included
                                        resultData = myTestProfileTestDelegate.ReadByTestIDSpecial(dbConnection, formRow.CalcTestID, "", "CALC")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myTestProfileTestsDS = DirectCast(resultData.SetDatos, TestProfileTestsDS)

                                            'Remove the affected Calculated Test from all Profiles in which it is included 
                                            For Each testProfileTestRow As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestsDS.tparTestProfileTests.Rows
                                                resultData = myTestProfileTestDelegate.DeleteByTestIDAndTestProfileID(dbConnection, testProfileTestRow.TestID, _
                                                                                                                      testProfileTestRow.TestProfileID, "CALC")
                                                If (resultData.HasError) Then Exit For
                                            Next
                                        End If
                                        If (resultData.HasError) Then Exit For

                                        'Delete Reference Ranges for the affected Calculated Test
                                        resultData = myTestRefRangesDelegate.DeleteByTestID(dbConnection, formRow.CalcTestID, "", "CALC")
                                        If (resultData.HasError) Then Exit For

                                        'Delete the elements of the Calculated Test Formula
                                        resultData = myFormulasDelegate.Delete(dbConnection, formRow.CalcTestID)
                                        If (resultData.HasError) Then Exit For

                                        'Delete the Calculated Test
                                        resultData = myCalcTestsDAO.Delete(dbConnection, formRow.CalcTestID)
                                        If (resultData.HasError) Then Exit For
                                    Next

                                    'Finally, verify if due the changes executed there are empty Profiles to delete them
                                    Dim myTestProfilesDAO As New TparTestProfilesDAO
                                    resultData = myTestProfilesDAO.DeleteEmptyProfiles(dbConnection)
                                End If
                            End If
                        End If

                        'Update data of the Calculated Test in Historic Module
                        If (HISTWorkingMode) Then
                            If (Not resultData.HasError) Then
                                If (pUpdateHistory) Then
                                    'Prepare the DS needed to update data in Historic Module
                                    Dim myHisCALCTestsDS As New HisCalculatedTestsDS
                                    Dim myHisCALCTestsRow As HisCalculatedTestsDS.thisCalculatedTestsRow

                                    myHisCALCTestsRow = myHisCALCTestsDS.thisCalculatedTests.NewthisCalculatedTestsRow
                                    myHisCALCTestsRow.CalcTestID = pCalcTestDS.tparCalculatedTests(0).CalcTestID
                                    myHisCALCTestsRow.CalcTestLongName = pCalcTestDS.tparCalculatedTests(0).CalcTestLongName
                                    myHisCALCTestsRow.DecimalsAllowed = CInt(pCalcTestDS.tparCalculatedTests(0).Decimals)
                                    myHisCALCTestsRow.MeasureUnit = pCalcTestDS.tparCalculatedTests(0).MeasureUnit
                                    myHisCALCTestsRow.FormulaText = pCalcTestDS.tparCalculatedTests(0).FormulaText

                                    myHisCALCTestsDS.thisCalculatedTests.AddthisCalculatedTestsRow(myHisCALCTestsRow)

                                    'Update data of the Calculated Test if it exists in Historic Module
                                    resultData = HIST_UpdateByCalcTestID(dbConnection, myHisCALCTestsDS)
                                End If
                            End If
                        End If

                        'Finally, verify if there is needed to update field FormulaText of Calculated Tests in which formula the modified 
                        'Calculated Test is included (this happens when the long name of the Calculated Test is changed)
                        If (Not resultData.HasError) Then
                            resultData = UpdateFormulaText(dbConnection, "CALC", pCalcTestDS.tparCalculatedTests(0).CalcTestID)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When the long name of a Standard, ISE, Off-System or Calculated Test is modified and it is included in the Formula of another Calculated Tests, value of
        ''' field FormulaText for these Calculated Tests has to be rebuilt and updated in both, Parameters Programming and Historic tables.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD, ISE, OFFS or CALC</param>
        ''' <param name="pTestID">Identifier of an STD, ISE, OFFS or CALC Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012
        ''' Modified by: WE 11/11/2014 - RQ00035C (BA-1867) - Updated Summary and Parameters description with ISE and Off-System as possible input value for parameter pTestType.
        ''' </remarks>
        Public Function UpdateFormulaText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search all Calculated Tests in which the specified Test is included (if any)
                        Dim myComponentsDS As New FormulasDS
                        Dim newFormulaText As String = String.Empty
                        Dim myFormulasDelegate As New FormulasDelegate
                        Dim myCalcTestsDAO As New tparCalculatedTestsDAO

                        myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(dbConnection, pTestID, String.Empty, pTestType, False)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myFormulasDS As FormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                            For Each affectedCalcTest As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                                'Search all components of the Formula of the Calculated Test in process 
                                myGlobalDataTO = myFormulasDelegate.GetFormulaValues(dbConnection, affectedCalcTest.CalcTestID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myComponentsDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                                    'Build the FormulaText 
                                    newFormulaText = String.Empty
                                    For Each formulaItem As FormulasDS.tparFormulasRow In myComponentsDS.tparFormulas.Rows
                                        If (formulaItem.ValueType = "TEST") Then
                                            newFormulaText &= formulaItem.TestName
                                        Else
                                            newFormulaText &= formulaItem.Value
                                        End If
                                    Next

                                    'If the FormulaText has changed (due to the name of at least one of its component tests have been changed),
                                    'update the field for the Calculated Test in process and, if it exists in Historic Module, update also the 
                                    'field for the Historic Calculated Test
                                    If (affectedCalcTest.FormulaText <> newFormulaText) Then
                                        myGlobalDataTO = myCalcTestsDAO.UpdateFormulaText(dbConnection, affectedCalcTest.CalcTestID, newFormulaText)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        If (HISTWorkingMode) Then
                                            myGlobalDataTO = HIST_UpdateFormulaText(dbConnection, affectedCalcTest.CalcTestID, newFormulaText)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    End If
                                Else
                                    'Error getting the Formula components
                                    Exit For
                                End If
                            Next
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdateFormulaText", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Calculated Test added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False only for Calculated Test that have been 
        '''                                  excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Calculated Test 
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                        ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparCalculatedTestsDAO
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
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
                        Dim myDAO As New tparCalculatedTestsDAO
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdateInUseByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Validate if the deletion of the specified Calculated Test affects other Calculated Tests (the ones having them in their Formula)
        ''' and/or the Test Profiles including them
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by:  TR 25/11/2010
        ''' Modified by: SA 03/01/2011 - Error control improved
        ''' </remarks>
        Public Function ValidatedDependencies(ByVal pCalculatedTestDS As CalculatedTestsDS) As GlobalDataTO
            Dim myResult As New GlobalDataTO

            Try
                Dim imageBytes As Byte()
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate()

                Dim myDependeciesElementsDS As New DependenciesElementsDS
                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                Dim myTempCalculateTestDS As New CalculatedTestsDS
                Dim myTempDependeciesElements As New DependenciesElementsDS
                Dim mytempCalTestRow As CalculatedTestsDS.tparCalculatedTestsRow

                Dim myGlobalDataTO As New GlobalDataTO
                Dim myFormulasDS As New FormulasDS
                Dim myRelatedElements As New FormulasDS

                Dim myTestProfileTestDS As New TestProfileTestsDS
                Dim myTestProfilesTestDelegate As New TestProfileTestsDelegate

                For Each calTestRow As CalculatedTestsDS.tparCalculatedTestsRow In pCalculatedTestDS.tparCalculatedTests.Rows
                    'Clean the Test Profiles DS for each new Calculated Test to process
                    myTestProfileTestDS.tparTestProfileTests.Clear()

                    'Get the Test Profiles in which the Calculated Test is included
                    myGlobalDataTO = myTestProfilesTestDelegate.ReadByTestID(Nothing, calTestRow.CalcTestID, "", "CALC")

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                        'Get icon for TestProfiles; load the returned Test Profiles in the list of affected Elements to return
                        imageBytes = preloadedDataConfig.GetIconImage("PROFILES")
                        For Each testProfTest As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                            myDependenciesElementsRow.Type = imageBytes
                            myDependenciesElementsRow.Name = testProfTest.TestProfileName
                            myDependenciesElementsRow.FormProfileMember = calTestRow.CalcTestLongName

                            'Insert on dependencies table
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                        Next
                    Else
                        myResult = myGlobalDataTO
                        Exit For
                    End If

                    'Get all Calculated Tests Formulas in which the Calculated Test is included
                    Dim myFormulasDelegate As New FormulasDelegate
                    myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(Nothing, calTestRow.CalcTestID, "", "CALC")

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                        'Get icon for Calculated Tests; load the returned Calculated Tests in the list of affected Elements to return
                        imageBytes = preloadedDataConfig.GetIconImage("TCALC")

                        'If myFormulasDS.tparFormulas.Count > 0 Then
                        For Each formRow As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                            myDependenciesElementsRow.Type = imageBytes
                            myDependenciesElementsRow.Name = formRow.TestName
                            myDependenciesElementsRow.FormProfileMember = formRow.FormulaText
                            myDependenciesElementsRow.Related = False

                            'Insert on dependencies table
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)

                            'Search on dependencies found if have other dependencies.
                            myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(Nothing, formRow.CalcTestID, formRow.SampleType, "CALC")

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myRelatedElements = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                                For Each RelatedElementRow As FormulasDS.tparFormulasRow In myRelatedElements.tparFormulas.Rows
                                    myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                    myDependenciesElementsRow.Type = imageBytes
                                    myDependenciesElementsRow.Name = RelatedElementRow.TestName
                                    myDependenciesElementsRow.FormProfileMember = RelatedElementRow.FormulaText
                                    myDependenciesElementsRow.Related = True 'Set the Related to true.

                                    'Insert on dependencies table
                                    myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                Next
                                mytempCalTestRow = myTempCalculateTestDS.tparCalculatedTests.NewtparCalculatedTestsRow()

                                'Verify if there are any other dependencies
                                mytempCalTestRow.CalcTestID = formRow.CalcTestID
                                mytempCalTestRow.CalcTestLongName = formRow.TestName
                                myTempCalculateTestDS.tparCalculatedTests.AddtparCalculatedTestsRow(mytempCalTestRow)

                                'Call this same function recursively
                                myGlobalDataTO = ValidatedDependencies(myTempCalculateTestDS)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTempDependeciesElements = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                                    For Each temDepElementRow As DependenciesElementsDS.DependenciesElementsRow In myTempDependeciesElements.DependenciesElements.Rows
                                        myDependeciesElementsDS.DependenciesElements.ImportRow(temDepElementRow)
                                    Next
                                Else
                                    myResult = myGlobalDataTO
                                    Exit For
                                End If
                            Else
                                myResult = myGlobalDataTO
                                Exit For
                            End If
                        Next
                        If (Not myGlobalDataTO.HasError) Then myResult.SetDatos = myDependeciesElementsDS
                    Else
                        Exit For
                        myResult = myGlobalDataTO
                    End If
                Next
            Catch ex As Exception
                myResult = New GlobalDataTO()
                myResult.HasError = True
                myResult.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResult.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.ValidatedDependencies", EventLogEntryType.Error, False)
            End Try
            Return myResult
        End Function

        ''' <summary>
        ''' Validate if the change in the SampleType of a Calculated Test affects other Calculated Tests (the ones having them in their
        ''' Formula) and/or Test Profiles including them
        ''' </summary>
        ''' <param name="pCalcTestDataDS">Typed DataSet CalculatedTestsDS containing the ID and Name of the Calculated Test</param>
        ''' <param name="pSampleTypes">List of SampleTypes of all Tests included in the Formula of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by:  TR 13/01/2011
        ''' Modified by: SA 14/01/2010 - Added searching of Calculated Tests in which Formula the informed one is included, and 
        '''                              also of Profiles in which the found ones are included
        '''              TR 05/05/2011 - Added parameter pDBConnection to reuse this method also for the update process
        ''' </remarks>
        Public Function ValidatedDependenciesOnUpdate(ByVal pCalcTestDataDS As CalculatedTestsDS, ByVal pSampleTypes As String, _
                                                      ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myTestProfileTestDS As New TestProfileTestsDS
                Dim myTestProfileTestDelegate As New TestProfileTestsDelegate

                Dim imageBytes As Byte()
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate()

                Dim myDependeciesElementsDS As New DependenciesElementsDS
                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                'Get the Test Profiles in which the Calculated Test is included and having a SampleType not included in the specified list
                myGlobalDataTO = myTestProfileTestDelegate.ReadByTestIDSpecial(pDBConnection, pCalcTestDataDS.tparCalculatedTests(0).CalcTestID, pSampleTypes, "CALC")

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                    If (pDBConnection Is Nothing) Then 'TR 31/01/2012 Validate connection to know if is comming from Update process
                        'Get icon for TestProfiles; load the returned Test Profiles in the list of affected Elements to return
                        imageBytes = preloadedDataConfig.GetIconImage("PROFILES")
                        For Each testProTestRow As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                            myDependenciesElementsRow.Type = imageBytes
                            myDependenciesElementsRow.Name = testProTestRow.TestProfileName
                            myDependenciesElementsRow.FormProfileMember = pCalcTestDataDS.tparCalculatedTests(0).CalcTestLongName

                            'Insert on dependencies table
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                        Next
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'Get all Calculated Tests Formulas in which the Calculated Test is included with a SampleType different of the informed ones
                    Dim myFormulasDelegate As New FormulasDelegate
                    myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(pDBConnection, pCalcTestDataDS.tparCalculatedTests(0).CalcTestID, pSampleTypes, "CALC", True)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myFormulasDS As FormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                        If (pDBConnection Is Nothing) Then 'TR 31/01/2012 Validate connection to know if is comming from Update process
                            'Get icon for Calculated Tests; load the returned Calculated Tests in the list of affected Elements to return
                            imageBytes = preloadedDataConfig.GetIconImage("TSTD_USER")
                            For Each formRow As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                                myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                myDependenciesElementsRow.Type = imageBytes
                                myDependenciesElementsRow.Name = formRow.TestName
                                myDependenciesElementsRow.FormProfileMember = formRow.FormulaText
                                myDependenciesElementsRow.Related = False

                                'Insert on dependencies table
                                myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)

                                'Find if the affected Calculated Tests are included in Profiles and in this case add them also to the list of 
                                'affected Elements...
                                myGlobalDataTO = myTestProfileTestDelegate.ReadByTestID(pDBConnection, formRow.CalcTestID, "", "CALC")

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                                    'Get icon for TestProfiles; load the returned Test Profiles in the list of affected Elements to return
                                    imageBytes = preloadedDataConfig.GetIconImage("PROFILES")
                                    For Each testProTestRow As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                                        myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                                        myDependenciesElementsRow.Type = imageBytes
                                        myDependenciesElementsRow.Name = testProTestRow.TestProfileName
                                        myDependenciesElementsRow.FormProfileMember = formRow.TestName

                                        'Insert on dependencies table
                                        myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                    Next
                                End If
                            Next
                        End If
                    End If
                End If

                'Return the DataSet with the list of affected elements
                If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = myDependeciesElementsDS
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.ValidatedDependenciesOnUpdate", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        '''  Update the LISValue by the pCalcTestID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalcTestID">Calculated Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparCalculatedTestsDAO
                        myGlobalDataTO = myDAO.UpdateLISValueByTestID(dbConnection, pCalcTestID, pLISValue)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdatepdateLISValueByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets all CALC tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
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
                        Dim myDAO As New tparCalculatedTestsDAO
                        resultData = myDAO.GetCustomizedSortedTestSelectionList(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "CalculatedTestsDelegate.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for CALC tests
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
                        Dim myDAO As New tparCalculatedTestsDAO
                        myGlobalDataTO = myDAO.UpdateCustomPositionAndAvailable(dbConnection, pTestsSortingDS)

                        'Update CALCTEST available in cascade depending their components (all components available -> CalcTest available // else CalcTest NOT available
                        Dim calcTestConfiguredAsNotAvailableList As List(Of ReportsTestsSortingDS.tcfgReportsTestsSortingRow)
                        calcTestConfiguredAsNotAvailableList = (From a As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.Available = False Select a).ToList
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = Me.UpdateAvailableCascadeByComponents(dbConnection, calcTestConfiguredAsNotAvailableList)
                        End If
                        calcTestConfiguredAsNotAvailableList = Nothing

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function


        ''' <summary>
        ''' Update calcTest Available value depending his components: All Available -- calcTest available (*) // Some NOT available -- calcTest not available
        ''' (* Exception those calcTest that user has configured as not Available - included in parameter pExceptions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExceptions"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 17/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateAvailableCascadeByComponents(ByVal pDBConnection As SqlClient.SqlConnection, Optional pExceptions As List(Of ReportsTestsSortingDS.tcfgReportsTestsSortingRow) = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparCalculatedTestsDAO
                        myGlobalDataTO = myDAO.UpdateAvailableCascadeByComponents(dbConnection, False)

                        'Special code AVAILABLE = FALSE: CALCTEST have 1 recursive level so we need call twice this method
                        'For example:
                        '1st STEP UREA-UV not avaiable affects ... BUN but not affects BUN/CREATININE because when query was executed the BUN was available
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myDAO.UpdateAvailableCascadeByComponents(dbConnection, False)
                        End If

                        If Not myGlobalDataTO.HasError Then

                            'Exceptions treatment
                            Dim exceptCalcTestID As String = String.Empty
                            If Not pExceptions Is Nothing AndAlso pExceptions.Count > 0 Then
                                For Each row As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pExceptions
                                    If exceptCalcTestID = String.Empty Then
                                        exceptCalcTestID = row.TestID.ToString
                                    Else
                                        exceptCalcTestID &= ", " & row.TestID.ToString
                                    End If
                                Next
                            End If
                            myGlobalDataTO = myDAO.UpdateAvailableCascadeByComponents(dbConnection, True, exceptCalcTestID)

                            'Special code AVAILABLE = TRUE: CALCTEST have 1 recursive level so we need call twice this method
                            'For example:
                            '1st STEP ALBUMIN not avaiable affects ... GLOBULIN & ALBUMIN/GLOBULIN
                            '2on STEP ALBUMIN avaiable affects ... GLOBULIN but ALBUMIN/GLOBULIN continues not available because when query was executed the GLOBULIN was not available
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myDAO.UpdateAvailableCascadeByComponents(dbConnection, True, exceptCalcTestID)
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

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.UpdateAvailableCascadeByComponents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' When a Calculated Test is deleted in Calculated Tests Programming Screen, if it exists in the corresponding table in Historics Module, 
        ''' then it is marked as deleted by updating field ClosedCalcTest to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Calculated Test Identifier in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 20/02/2012
        ''' Modified by: SA 24/02/2012 - Name of the function changed from Delete to CloseCalculatedTest; changed the entry parameter, it is the 
        '''                              ID of the Calculated Test in Parameters Programming, not in Historics Module; before calling the corresponding
        '''                              DAO function, verify if the Calculated Test already exists in Historics Module
        '''              TR 04/09/2012 - Method has been moved from class HisCalculatedTestDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisCalculatedTests are called
        ''' </remarks>
        Private Function HIST_CloseCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there is an open version of the Calculated Test in Historics
                        Dim myDAO As New thisCalculatedTestsDAO()
                        resultData = myDAO.ReadByCalcTestID(dbConnection, pCalcTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisCalculatedTestsDS = DirectCast(resultData.SetDatos, HisCalculatedTestsDS)
                            If (auxiliaryDS.thisCalculatedTests.Rows.Count > 0) Then
                                'There is an open version of the Calculated Test  in Historics module, mark it as Closed
                                resultData = myDAO.CloseCalculatedTest(dbConnection, auxiliaryDS.thisCalculatedTests.First.HistCalcTestID)
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
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.HIST_CloseCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When basic data of a Calculated Test (CalcTestLongName, MeasureUnit and/or Decimals) is changed in Calculated Tests Programming Screen, 
        ''' values are updated for the corresponding record in the Historics Module table (the one having the same  CalcTestID and ClosedCalcTest=False)
        ''' But when besides basic data, the Formula of the Test is changed, then the Calculated Test is marked as closed  and the data is not updated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalcTestsDS">Typed DS HisCalculatedTestsDS containing data of the Calculated Test to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 22/02/2012
        ''' Modified by: SA 24/02/2012 - Changed the function name from Update to UpdateByCalcTestID; changed the type of the entry DS from HisCalculatedTestsDS
        '''                              to CalculatedTestsDS due to this function is called from class CalculatedTestsDelegate; other changes in the function
        '''                              logic due to changes in name and parameters in the DAO functions
        '''              TR 04/09/2012 - Method has been moved from class HisCalculatedTestDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisCalculatedTests are called
        ''' </remarks>
        Private Function HIST_UpdateByCalcTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalcTestsDS As HisCalculatedTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there is an open version of the Calculated Test in Historics
                        Dim myDAO As New thisCalculatedTestsDAO()
                        resultData = myDAO.ReadByCalcTestID(dbConnection, pHisCalcTestsDS.thisCalculatedTests.First.CalcTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisCalculatedTestsDS = DirectCast(resultData.SetDatos, HisCalculatedTestsDS)

                            If (auxiliaryDS.thisCalculatedTests.Rows.Count > 0) Then
                                'There is an open version of the Calculated Test in Historics module, verify if the Formula has been changed
                                If (pHisCalcTestsDS.thisCalculatedTests.First.FormulaText = auxiliaryDS.thisCalculatedTests.First.FormulaText) Then
                                    'Data of the current version of the Calculated Test will be updated - inform value of field HistCalcTestID in the entry DS
                                    pHisCalcTestsDS.thisCalculatedTests.First.HistCalcTestID = auxiliaryDS.thisCalculatedTests.First.HistCalcTestID
                                    resultData = myDAO.Update(dbConnection, pHisCalcTestsDS)
                                Else
                                    'The current version of the Calculated Test is marked as closed
                                    resultData = myDAO.CloseCalculatedTest(dbConnection, auxiliaryDS.thisCalculatedTests.First.HistCalcTestID)
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
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.HIST_UpdateByCalcTestID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' If the text of the formula of a Calculated Test has been changed due to the long name of a Standard, ISE, Off-System or Calculated Test included in the formula
        ''' has been changed, if the Calculated Test exists in Historic Module, the FormulaText field is updated also for it.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <param name="pNewFormulaText">New text of the Formula of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012 - Method has been placed here instead of in class HisCalculatedTestDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisCalculatedTests are called
        ''' Modified by: WE 11/11/2014 - RQ00035C (BA-1867) - Updated Summary description with ISE and Off-System as possible sources for changing its name.
        ''' </remarks>
        Private Function HIST_UpdateFormulaText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, ByVal pNewFormulaText As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there is an open version of the Calculated Test in Historics
                        Dim myDAO As New thisCalculatedTestsDAO()
                        resultData = myDAO.ReadByCalcTestID(dbConnection, pCalcTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisCalculatedTestsDS = DirectCast(resultData.SetDatos, HisCalculatedTestsDS)

                            If (auxiliaryDS.thisCalculatedTests.Rows.Count > 0) Then
                                'There is an open version of the Calculated Test in Historics module, update the FormulaText
                                resultData = myDAO.UpdateFormulaText(dbConnection, auxiliaryDS.thisCalculatedTests.First.HistCalcTestID, pNewFormulaText)
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
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.HIST_UpdateFormulaText", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Save Reference Ranges for a Calculated Test: add new ranges, update values of existing ranges, and/or remove deleted 
        ''' existing ranges 
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pNewRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to add</param>
        ''' <param name="pUpdatedRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to update</param>
        ''' <param name="pDeletedRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 28/10/2010
        ''' </remarks>
        Private Function SaveCalcTestRefRanges(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pNewRefRanges As TestRefRangesDS, _
                                               ByVal pUpdatedRefRanges As TestRefRangesDS, ByVal pDeletedRefRanges As TestRefRangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRefRangesDel As New TestRefRangesDelegate

                        'Remove deleted Ranges
                        If (pDeletedRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                            resultData = myRefRangesDel.Delete(dbConnection, pDeletedRefRanges, "CALC")
                        End If

                        'Add new Ranges
                        If (Not resultData.HasError) Then
                            If (pNewRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                resultData = myRefRangesDel.Create(dbConnection, pNewRefRanges, "CALC")
                            End If
                        End If

                        'Update values of modified Ranges
                        If (Not resultData.HasError) Then
                            If (pUpdatedRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                resultData = myRefRangesDel.Update(dbConnection, pUpdatedRefRanges, "CALC")
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
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.SaveCalcTestRefRanges", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Receives all added/updated/deleted Reference Ranges for a Calculated Test, places them in a three separated 
        ''' DataSets and call the function that execute the saving of the Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRefRangesDS">Typed DataSet TestRefRangesDS containing all added/updated/deleted Reference
        '''                            Ranges for a Calculated Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/12/2010
        ''' </remarks>
        Private Function SaveReferenceRanges(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRefRangesDS As TestRefRangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Separate added, updated and deleted Reference Ranges in three different DS
                        Dim newCalcRefRangesDS As New TestRefRangesDS
                        Dim updatedCalcRefRangesDS As New TestRefRangesDS
                        Dim deletedCalcRefRangesDS As New TestRefRangesDS

                        'Dim myGlobalbase As New GlobalBase
                        Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)

                        'CREATE: Get all added Reference Ranges
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsNew = True _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            myTestRefRanges(0).BeginEdit()
                            myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                            myTestRefRanges(0).TS_DateTime = Now
                            myTestRefRanges(0).EndEdit()

                            newCalcRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'UPDATE: Get all updated Reference Ranges
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsNew = False _
                                            And a.IsDeleted = False _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            myTestRefRanges(0).BeginEdit()
                            myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                            myTestRefRanges(0).TS_DateTime = Now
                            myTestRefRanges(0).EndEdit()

                            updatedCalcRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'DELETE: Get all Reference Ranges marked to delete
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsDeleted = True _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            deletedCalcRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'TR 18/02/2013 -Set the connection, before was nothing.
                        'Save Reference Ranges
                        resultData = SaveCalcTestRefRanges(dbConnection, newCalcRefRangesDS, updatedCalcRefRangesDS, deletedCalcRefRangesDS)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsDelegate.SaveReferenceRanges", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class

End Namespace