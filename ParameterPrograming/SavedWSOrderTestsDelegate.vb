Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class SavedWSOrderTestsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Add new Order Tests to an specific Saved Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSOrderTests">Typed DataSet SavedWSOrderTestsDS containing the list of WS Order Tests to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' </remarks>
        Public Function AddOrderTestsToSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSOrderTests As SavedWSOrderTestsDS, _
                                               Optional pSavedWSID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.Create(dbConnection, pSavedWSOrderTests, pSavedWSID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.AddOrderTestsToSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Cancel message is received from LIS and the AwosID has not been already added to the active WS, it has to be
        ''' searched in the list of LIS Saved WorkSession and in case it exists in one of them, it has to be deleted. If the AwosID 
        ''' corresponds to a Calculated Tests, the Tests included in its formula have to be also deleted if it is possible 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAWOSID">Identifier of the AWOS to cancel</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: JCM 22/05/2013
        ''' Modified by: SA 24/05/2013 - Function moved from class XMLMessagesDelegate
        '''                            - Function GetByAwosID is called out of the DB Transaction
        '''                            - Replaced several IFs for a SELECT CASE 
        ''' </remarks>
        Public Function CancelAWOSInLISSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, pAWOSID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Search the AwosID in the LIS Saved WS that have not been still added to the active WS
                resultData = GetByAwosID(Nothing, pAWOSID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                    If (mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Count = 0) Then
                        'The AWOS ID cannot be cancelled due to it does not exist in the active WorkSession
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.LIS_AWOS_NOT_IN_WS.ToString()
                    Else
                        'Begin DB Transaction to Cancel the AwosID
                        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                            If (Not dbConnection Is Nothing) Then
                                Select Case (mySavedWSOrderTestsDS.tparSavedWSOrderTests.First.TestType)
                                    Case "STD"
                                        'For STD Tests, the AWOS ID can be deleted if it is not needed for another requested Calculated Test.
                                        'In case it is needed for another requested Calculated Test, the LIS information for the Standard Test is removed
                                        If (Not String.IsNullOrEmpty(mySavedWSOrderTestsDS.tparSavedWSOrderTests.First.CalcTestIDs)) Then
                                            'Delete the LIS fields for the Standard Test in the Saved WS
                                            With (mySavedWSOrderTestsDS.tparSavedWSOrderTests.First)
                                                resultData = UpdateAsManualOrderTest(dbConnection, pAWOSID, .TestID, .TestType, .SavedWSID)
                                            End With
                                        Else
                                            'The OrderTest is for an independent STD Test. The AwosID can be deleted from the LIS Saved WS
                                            With (mySavedWSOrderTestsDS.tparSavedWSOrderTests.First)
                                                resultData = Delete(dbConnection, .SavedWSID, .SavedWSOrderTestID)
                                            End With
                                        End If

                                    Case "CALC"
                                        'Verify if the OrderTest can be deleted.  When possible, delete also OrderTests included in the formula of the Calculated Test
                                        resultData = DeleteCALCInLISSavedWS(dbConnection, mySavedWSOrderTestsDS.tparSavedWSOrderTests.First)

                                    Case Else
                                        'The OrderTest is for an ISE or OFFS Test. The AwosID can be deleted from the LIS Saved WS
                                        With (mySavedWSOrderTestsDS.tparSavedWSOrderTests.First)
                                            resultData = Delete(dbConnection, .SavedWSID, .SavedWSOrderTestID)
                                        End With
                                End Select

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.CancelAWOSInLISSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete from a Saved WS that have been requested to be loaded, all Tests (whatever type) that have
        ''' been deleted or that are incomplete (this last case is only for Calculated Tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing an integer value with the total number of deleted Order Tests</returns>
        ''' <remarks> 
        ''' Created by: GDS 30/03/2010
        ''' Modified by: SA 22/10/2010 - When verifying if the Formula of a Calculated Test has changed, do not remove the first character; 
        '''                              the Formula is stored without the = character when the WS is Saved
        '''              SA 01/02/2011 - For Off-System Tests, verify the all Test/SampleType in the Saved WS still exists in the DB  
        ''' </remarks>
        Public Function ClearDeletedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim deletedElements As Integer = 0
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO

                        'Delete from the Saved WS all Tests (whatever type) that have been removed from the DB
                        resultData = mySavedWSOrderTestsDAO.ClearDeletedElements(dbConnection, pSavedWSID)
                        If (Not resultData.HasError) Then
                            deletedElements = CType(resultData.AffectedRecords, Integer)

                            'For Standard Tests, it can happens that the Test exists but the SampleType is not used anymore
                            resultData = mySavedWSOrderTestsDAO.ReadTestsByType(dbConnection, pSavedWSID, "STD")
                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                Dim myTestSamplesDS As New TestSamplesDS
                                Dim myTestSamplesDelegate As New TestSamplesDelegate
                                For Each savedWSOrderTestRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                    'Verify if the Test/SampleType still exists in the DB
                                    resultData = myTestSamplesDelegate.GetDefinition(dbConnection, savedWSOrderTestRow.TestID, savedWSOrderTestRow.SampleType)

                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                        myTestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)
                                        If (myTestSamplesDS.tparTestSamples.Rows.Count = 0) Then
                                            'Delete from the Saved WS, all Order Tests containing the removed Test/SampleType
                                            resultData = mySavedWSOrderTestsDAO.DeleteSavedTests(dbConnection, pSavedWSID, "STD", _
                                                                                                 savedWSOrderTestRow.TestID, savedWSOrderTestRow.SampleType)
                                            If (Not resultData.HasError) Then deletedElements += resultData.AffectedRecords
                                        End If
                                    End If
                                    If (resultData.HasError) Then Exit For
                                Next
                            End If

                            If (Not resultData.HasError) Then
                                'For Calculated Tests, it can happens that the Test exists but its Formula has been changed
                                resultData = mySavedWSOrderTestsDAO.ReadTestsByType(dbConnection, pSavedWSID, "CALC")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                    Dim myCalcTestDS As New CalculatedTestsDS
                                    Dim myCalcTestDelegate As New CalculatedTestsDelegate
                                    For Each savedWSOrderTestRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                        'Get the current Formula for the Calculated Test
                                        resultData = myCalcTestDelegate.GetCalcTest(dbConnection, savedWSOrderTestRow.TestID)

                                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                            myCalcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                            If (myCalcTestDS.tparCalculatedTests.Rows.Count = 1) Then
                                                'If the Formula has changed, the Calculated Test should be deleted from the Saved WorkSession 
                                                '(the = is removed before comparing values)
                                                If (savedWSOrderTestRow.FormulaText.Trim <> myCalcTestDS.tparCalculatedTests(0).FormulaText.Trim) Then
                                                    'Delete from the Saved WS, all Order Tests containing the Calculated Test which Formula has been changed
                                                    resultData = mySavedWSOrderTestsDAO.DeleteSavedTests(dbConnection, pSavedWSID, "CALC", savedWSOrderTestRow.TestID)
                                                    If (Not resultData.HasError) Then deletedElements += resultData.AffectedRecords

                                                Else

                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'For OFF-SYSTEM Tests, it can happens that the Test exists but the SampleType is not used anymore
                                resultData = mySavedWSOrderTestsDAO.ReadTestsByType(dbConnection, pSavedWSID, "OFFS")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                    Dim myTestSamplesDS As New OffSystemTestSamplesDS
                                    Dim myTestSamplesDelegate As New OffSystemTestSamplesDelegate
                                    For Each savedWSOrderTestRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                        'Verify if the Test/SampleType still exists in the DB
                                        resultData = myTestSamplesDelegate.GetListByOffSystemTestID(dbConnection, savedWSOrderTestRow.TestID, savedWSOrderTestRow.SampleType)

                                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                            myTestSamplesDS = DirectCast(resultData.SetDatos, OffSystemTestSamplesDS)

                                            If (myTestSamplesDS.tparOffSystemTestSamples.Rows.Count = 0) Then
                                                'Delete from the Saved WS, all Order Tests containing the removed Test/SampleType
                                                resultData = mySavedWSOrderTestsDAO.DeleteSavedTests(dbConnection, pSavedWSID, "OFFS", _
                                                                                                     savedWSOrderTestRow.TestID, savedWSOrderTestRow.SampleType)
                                                If (Not resultData.HasError) Then deletedElements += resultData.AffectedRecords
                                            End If
                                        End If
                                        If (resultData.HasError) Then Exit For
                                    Next
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            resultData.SetDatos = deletedElements

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
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.ClearDeletedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there are Order Tests for the specified SampleID in at least a LIS Saved WS pending to process. This function is used
        ''' when XML Messages are processes and a Patient Node contains a Patient Identifier that exists in tparPatients table, but LIS has 
        ''' not sent demographics for it; in that case, if the Patient is not IN USE, then it is deleted from Patients table, where not IN USE
        ''' means:
        '''   ** It is not in the active WorkSession
        '''   ** It is not in any LIS Saved WorkSession pending to process (this is what this function verifies)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleID">Sample Identifier to search</param>
        ''' <returns>GlobalDataTO containing a Boolean value: when TRUE, it means there is at least an Order Test in a LIS Saved WS for 
        '''          the informed SampleID</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2013 
        ''' </remarks>
        Public Function CountBySampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.CountBySampleID(dbConnection, pSampleID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.CountBySampleID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Order Test from a Saved WS and, if this is empty  (it does not have other Order Tests), it is also deleted
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Saved WS Identifier</param>
        ''' <param name="pSavedWSOrderTestID">Saved WS Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks> 
        ''' Created by:  SA 23/04/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, ByVal pSavedWSOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.Delete(dbConnection, pSavedWSOrderTestID)

                        If (Not resultData.HasError) Then
                            'If the Saved WS is empty, delete it
                            Dim mySavedWSDelegate As New SavedWSDelegate
                            resultData = mySavedWSDelegate.DeleteEmptySavedWS(dbConnection, pSavedWSID)
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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests included in the specified Saved Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Saved WS Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.DeleteAll(dbConnection, pSavedWSID)

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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.DeleteAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Manage the deletion of AwosID of Calculated Tests when a Cancel for it is received from LIS. When the Awos can be deleted, all Tests 
        ''' included in the Formula of the corresponding Calculated Test are also deleted when it is possible. For formula members that are also
        ''' Calculated Tests, this function is called recursively to verify if it (and all its linked Tests) can be deleted.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSOrderTestsRow">Row of a SavedWSOrderTestsDS containing all data of the Order Test for a Calculated Test to delete</param>
        ''' <returns>GlobalDataTO containing a typed OrdersDS containing all different Patients/StatFlags in the informed Saved WS</returns>
        ''' <remarks>
        ''' Created by:  JCM 22/05/2013
        ''' Modified by: SA  24/05/2013 - Changed pBDConnection by dbConnection when calling functions UpdateAsManualOrderTest, Delete and UpdateCalcTestsLinks
        '''                             - Changed row.SavedWSID by row.SavedWSOrderTestID when calling function UpdateCalcTestsLinks
        '''                             - Changed the For/Next to rebuild field CalcTestNames: read content of field row.CalcTestNames instead of 
        '''                               pSavedWSOrderTestsRow.CalcTestNames
        '''                             - Added error control
        ''' </remarks>
        Public Function DeleteCALCInLISSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, pSavedWSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If the Calculated Test is included in the formula of another requested Calculated Test, then it cannot be deleted
                        If (Not String.IsNullOrEmpty(pSavedWSOrderTestsRow.CalcTestIDs)) Then
                            'If LIS fields are informed, they are deleted
                            resultData = UpdateAsManualOrderTest(dbConnection, pSavedWSOrderTestsRow.AwosID, pSavedWSOrderTestsRow.TestID, pSavedWSOrderTestsRow.TestType, _
                                                                 pSavedWSOrderTestsRow.SavedWSID)
                        Else
                            'The OrderTest is for an independent CALC Test. It can be deleted and it has to be verified if the Order Tests 
                            'for the Tests included in the formula of the Calculated Test can be also deleted

                            'The OrderTest is deleted from the LIS Saved WS
                            resultData = Delete(dbConnection, pSavedWSOrderTestsRow.SavedWSID, pSavedWSOrderTestsRow.SavedWSOrderTestID)
                            If (Not resultData.HasError) Then
                                'Get from the LIS Saved WS all Order Tests for the same SampleID and having field CalcTestIDs informed
                                Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO

                                resultData = mySavedWSOrderTestsDAO.GetOrderTestsBySampleID(dbConnection, pSavedWSOrderTestsRow.SavedWSID, pSavedWSOrderTestsRow.SampleID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim ordersCandidatesToDelete As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                    Dim calcTestsIds As String()
                                    Dim rebuildCalcTestsIds As String = String.Empty
                                    Dim calcTestIdsHasMoreThanOne As Boolean = False
                                    For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In ordersCandidatesToDelete.tparSavedWSOrderTests.Rows
                                        'Get the list of Identificadores of Calculated Tests in the field
                                        calcTestsIds = row.CalcTestIDs.Split(CChar(","))
                                        calcTestIdsHasMoreThanOne = (calcTestsIds.Count > 1)

                                        rebuildCalcTestsIds = String.Empty
                                        For Each calcTest As String In calcTestsIds
                                            If (calcTest.Trim = pSavedWSOrderTestsRow.TestID.ToString) Then
                                                'If the Order Test was needed only for the deleted Calculated Test, then it can be deleted
                                                If (Not calcTestIdsHasMoreThanOne) Then
                                                    If (String.IsNullOrEmpty(row.AwosID)) Then
                                                        'The OrderTest was not requested by LIS; it can be deleted
                                                        If (row.TestType = "STD") Then
                                                            'The Order Test is for an Standard Test that is not needed for another Order Tests; it can be deleted
                                                            resultData = Delete(dbConnection, row.SavedWSID, row.SavedWSOrderTestID)
                                                        Else
                                                            'The Order Test is for a Calculated test. This function is called recursively to verify if the Calculated Test 
                                                            'and its formula members can be also deleted
                                                            resultData = DeleteCALCInLISSavedWS(dbConnection, row)
                                                        End If
                                                    Else
                                                        'The OrderTest was requested by LIS. It cannot be deleted, but fields CalcTestIDs and 
                                                        'CalcTestNames have to be emptied
                                                        resultData = mySavedWSOrderTestsDAO.UpdateCalcTestsLinks(dbConnection, row.SavedWSOrderTestID, "", "")
                                                    End If
                                                    If (resultData.HasError) Then Exit For
                                                End If
                                            Else
                                                'Add the CalcTestID to the list of Calculated Test IDs that have to remain in the rebuilt field
                                                rebuildCalcTestsIds &= CStr(IIf(String.IsNullOrEmpty(rebuildCalcTestsIds), "", ", ")) & calcTest.Trim
                                            End If
                                        Next

                                        If (Not resultData.HasError) Then
                                            If (calcTestIdsHasMoreThanOne) Then
                                                'If the Order Test was linked to several Calculated Tests, rebuild also field CalcTestNames
                                                Dim rebuildCalcTestNames As String = String.Empty
                                                For Each calcTest As String In row.CalcTestNames.Split(CChar(","))
                                                    If (calcTest.Trim <> pSavedWSOrderTestsRow.TestName.ToString) Then
                                                        rebuildCalcTestNames &= CStr(IIf(String.IsNullOrEmpty(rebuildCalcTestNames), "", ", ")) & calcTest.Trim
                                                    End If
                                                Next

                                                'Finally set new value of fields CalcTestIDs and CalcTestNames for the Order Test in the Saved WS
                                                resultData = mySavedWSOrderTestsDAO.UpdateCalcTestsLinks(dbConnection, row.SavedWSOrderTestID, rebuildCalcTestsIds, rebuildCalcTestNames)
                                            End If
                                        End If
                                    Next
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.DeleteCALCInLISSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all different Patients/StatFlags in the informed Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed OrdersDS containing all different Patients/StatFlags in the informed Saved WS</returns>
        ''' <remarks>
        ''' Created by:  SA 13/06/2012
        ''' </remarks>
        Public Function GetAllDifferentPatients(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.ReadAllDifferentPatients(dbConnection, pSavedWSID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetAllDifferentPatients", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all information needed to build the Rejected Delayed message for each Order Test in a group of LIS Saved WS that have to be deleted 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with information needed to build the Rejected Delayed message
        '''          for each Order Test in a group of LIS Saved WS that have to be deleted from LIS Utilities Screen</returns>
        ''' <remarks>
        ''' Created by:  TR 23/04/2013
        ''' Modified by: AG 30/04/2013 - Inform SampleType (instead of overwrite SampleClass), TestIDString (instead of overwrite TestType), and
        '''                              QC (instead of qc; ES waits upper chars)
        ''' </remarks>
        Public Function GetAllLISOrderTestToReject(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        myGlobalDataTO = mySavedWSOrderTestsDAO.GetAllLISOrderTestToReject(dbConnection)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, SavedWSOrderTestsDS)

                            Dim rejectedAwosDS As New OrderTestsLISInfoDS
                            Dim rejectedAwosRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow

                            Dim myLISMappingsDAO As New vcfgLISMappingsDAO  'NOTE: Call the DAO instead of the delegate because the circular references!!!
                            Dim myAllTestTypeMapDelegate As New AllTestByTypeDelegate

                            For Each swSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                'Set value of field SampleClass
                                If (swSOrderTestsRow.SampleClass = "PATIENT" AndAlso Not swSOrderTestsRow.ExternalQC) Then
                                    swSOrderTestsRow.SampleClass = "patient"

                                ElseIf (swSOrderTestsRow.SampleClass = "CTRL" OrElse _
                                       (swSOrderTestsRow.SampleClass = "PATIENT" AndAlso swSOrderTestsRow.ExternalQC)) Then
                                    swSOrderTestsRow.SampleClass = "QC"
                                End If

                                'Get the LIS Value for the Sample Type 
                                myGlobalDataTO = myLISMappingsDAO.GetLISValue(dbConnection, "SAMPLE_TYPES", swSOrderTestsRow.SampleType)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'Set value of field Sample Type with the mapped LIS Value
                                swSOrderTestsRow.SampleType = myGlobalDataTO.SetDatos.ToString()

                                'Get the LIS Value for the TestType/TestID
                                myGlobalDataTO = myAllTestTypeMapDelegate.GetLISValue(dbConnection, swSOrderTestsRow.TestType, swSOrderTestsRow.TestID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    'Set value of field TestName with the mapped LIS Value
                                    swSOrderTestsRow.TestName = myGlobalDataTO.SetDatos.ToString()

                                    'Fill the rejecteDS 
                                    rejectedAwosRow = rejectedAwosDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                    rejectedAwosRow.AwosID = swSOrderTestsRow.AwosID
                                    rejectedAwosRow.SpecimenID = swSOrderTestsRow.SpecimenID
                                    rejectedAwosRow.SampleClass = swSOrderTestsRow.SampleClass
                                    rejectedAwosRow.TestType = swSOrderTestsRow.TestType
                                    rejectedAwosRow.TestIDString = swSOrderTestsRow.TestName 'AG 02/05/2013
                                    rejectedAwosRow.SampleType = swSOrderTestsRow.SampleType
                                    rejectedAwosRow.ESOrderID = swSOrderTestsRow.ESOrderID
                                    rejectedAwosRow.ESPatientID = swSOrderTestsRow.ESPatientID

                                    If (Not swSOrderTestsRow.IsLISPatientIDNull) Then rejectedAwosRow.LISPatientID = swSOrderTestsRow.LISPatientID
                                    If (Not swSOrderTestsRow.IsLISOrderIDNull) Then rejectedAwosRow.LISOrderID = swSOrderTestsRow.LISOrderID
                                    rejectedAwosDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(rejectedAwosRow)
                                Else
                                    Exit For
                                End If
                            Next

                            If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = rejectedAwosDS
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetAllLISOrderTestToReject", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search if the specified AwosID exists in the group of Order Tests of LIS Saved WS that have not been still added to the active WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAwosID">Awos Identifier to search in the group of Saved WS Order Tests</param>
        ''' <returns>GlobalDataTO containing a SavedWSOrderTestsDS with all information of the saved Order Test for the informed Awos ID</returns>
        ''' <remarks>
        ''' Created by: SA 23/04/2013 
        ''' </remarks>
        Public Function GetByAwosID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.ReadByAwosID(dbConnection, pAwosID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetByAwosID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of the Order Tests included in the specified Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with the list of the Order Tests
        '''          included in the specified Saved WS</returns> 
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' </remarks>
        Public Function GetOrderTestsBySavedWSID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.ReadBySavedWSID(dbConnection, pSavedWSID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetOrderTestsBySavedWSID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all OrderTests of the specified SampleClass in the informed Saved WS. Used when a new Analyzer is connected and 
        ''' there is a WorkSession with status different of EMPTY and OPEN
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <param name="pStatFlag">Flag indicating if the OrderTests were requested for Stat (when True) or for Routine (when False)</param>
        ''' <param name="pNewAnalyzerID">Identifier of the new connected Analyzer</param>
        ''' <param name="pSampleID">Patient/Sample Identifier. Optional parameter informed only when SampleClass=PATIENT</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with all OrderTests of the specified SampleClass in 
        '''          the informed Saved WS</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' </remarks>
        Public Function GetOrderTestsToChangeAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                                      ByVal pSampleClass As String, ByVal pStatFlag As Boolean, ByVal pNewAnalyzerID As String, _
                                                      Optional ByVal pSampleID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.ReadBySavedWSIDToChangeAnalyzer(dbConnection, pSavedWSID, pSampleClass, pStatFlag, pNewAnalyzerID, pSampleID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetOrderTestsToChangeAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get from the specified LIS Saved WS, all Specimens with Tests for more than one Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the LIS Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet </returns>
        ''' <remarks>
        ''' Created by: SA 17/07/2013
        ''' </remarks>
        Public Function GetSpecimensWithSeveralSampleTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
                        resultData = mySavedWSOrderTestsDAO.GetSpecimensWithSeveralSampleTypes(dbConnection, pSavedWSID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.GetSpecimensWithSeveralSampleTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a LIS WorkOrder is cancelled from LIS and it exists in a Saved WS (saved manually for user), this Order Test remains in
        ''' the saved WS but as manual work order (LIS fields are set to NULL)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAwosID">Awos Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pSavedWSID">Identifier of a LIS Saved WS. Optional parameter.  When informed, values are updated in the specified Saved WS; 
        '''                          when not informed (default value), the filter by NOT LIS Saved WS is applied</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/05/2013
        ''' Modified by: SA 24/05/2013 - Added optional parameter pSavedWSID
        ''' </remarks>
        Public Function UpdateAsManualOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String, ByVal pTestID As Integer, ByVal pTestType As String, _
                                                Optional ByVal pSavedWSID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparSavedWSOrderTestsDAO
                        resultData = myDAO.UpdateAsManualOrderTest(dbConnection, pAwosID, pTestID, pTestType, pSavedWSID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.UpdateAsManualOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Test (whatever type) is deleted, all Order Tests for that TestType/TestID in LIS Saved Work Sessions have to be marked as deleted
        ''' (DeletedTestFlag = TRUE) and additionally, the current LIS mapping value for the TestType/TestID, has to be saved in the field TestName 
        ''' to allow building the RejectedDelayed message properly (this message will be built and sent when the LIS Orders Download is executed and the
        ''' LIS Saved WorkSessions are processed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code; optional parameter. When informed, it means that the Test exists but its relation with the 
        '''                           Sample Type has been deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2013
        ''' Modified by: AG 10/05/2013 - Added optional parameter pSampleType
        ''' </remarks>
        Public Function UpdateDeletedTestFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                              Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAllTestsByTypeDelegate As New AllTestByTypeDelegate
                        resultData = myAllTestsByTypeDelegate.GetLISValue(dbConnection, pTestType, pTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim testMappedValue As String = CStr(resultData.SetDatos)

                            If (testMappedValue <> String.Empty) Then
                                Dim myDAO As New tparSavedWSOrderTestsDAO
                                resultData = myDAO.UpdateDeletedTestFlag(dbConnection, pTestType, pTestID, testMappedValue, pSampleType)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSOrderTestsDelegate.UpdateDeletedTestFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
