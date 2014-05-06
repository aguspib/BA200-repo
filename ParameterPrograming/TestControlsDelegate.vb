Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Text.RegularExpressions

Namespace Biosystems.Ax00.BL

    Public Class TestControlsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Link a TestType/Test/SampleType to an specific Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the TestType/Test/SampleType to link</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 05/04/2011
        ''' Modified by: TR 07/04/2011 - Implemented creation logic
        '''              SA 16/05/2011 - After linking a Control to a Test/SampleType, increment value of field NumberOfControls for it
        '''              SA 06/10/2011 - If for the Test/SampleType there are less than three linked Controls marked as active, the new one is 
        '''                              create as active
        '''              TR 12/01/2012 - Add optional parammeter pNoActiveControl to validate if new test control has selected active controls Screen module.
        '''              RH 11/06/2012 - Use of TestType
        ''' </remarks>
        Public Function AddTestControlsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS, _
                                           Optional ByVal pNoActiveControl As Boolean = True) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControlsDAO As New tparTestControlsDAO
                        myGlobalDataTO = myTestControlsDAO.CountActiveByTestIDAndSampleTypeNEW(dbConnection, pTestControlDS.tparTestControls(0).TestType, _
                                                                                               pTestControlDS.tparTestControls(0).TestID, _
                                                                                               pTestControlDS.tparTestControls(0).SampleType)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (pNoActiveControl) Then
                                'If the number of active Controls for the Test/SampleType is less than 3, then the new linked Control is marked as active
                                If (DirectCast(myGlobalDataTO.SetDatos, Integer) < 3) Then
                                    pTestControlDS.tparTestControls(0).BeginEdit()
                                    pTestControlDS.tparTestControls(0).ActiveControl = True
                                    pTestControlDS.tparTestControls(0).EndEdit()
                                End If
                            End If

                            'Create the new link between the Test/SampleType and the Control
                            myGlobalDataTO = myTestControlsDAO.CreateNEW(dbConnection, pTestControlDS)

                            If (Not myGlobalDataTO.HasError) Then
                                'Update value of field NumberOfControls for the Test/SampleType, according the TestType
                                If (pTestControlDS.tparTestControls(0).TestType = "STD") Then
                                    Dim myTestSamplesDelegate As New TestSamplesDelegate
                                    myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestControlDS.tparTestControls.First.TestID, _
                                                                                               pTestControlDS.tparTestControls.First.SampleType)
                                ElseIf (pTestControlDS.tparTestControls(0).TestType = "ISE") Then
                                    Dim myISETestSamplesDelegate As New ISETestSamplesDelegate
                                    myGlobalDataTO = myISETestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestControlDS.tparTestControls.First.TestID, _
                                                                                                     pTestControlDS.tparTestControls.First.SampleType)
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
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.AddTestControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Count the number of Controls linked to the informed Test/SampleType that are currently marked as active Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Controls linked to the informed TestType/Test/SampleType 
        '''          that are currently marked as active Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 19/09/2012
        ''' </remarks>
        Public Function CountActiveByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                            ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControlsDAO As New tparTestControlsDAO
                        resultData = myTestControlsDAO.CountActiveByTestIDAndSampleTypeNEW(dbConnection, pTestType, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.CountActiveByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all the Tests/SampleTypes linked to the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 05/04/2011
        ''' Modified by: SA 10/05/2011 - Removed parameter for the Sample Type
        ''' </remarks>
        Public Function DeleteByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControlsDAO As New tparTestControlsDAO
                        resultData = myTestControlsDAO.DeleteByControlID(dbConnection, pControlID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.DeleteByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Controls linked to the specified Standard or ISE Test and, optionally SampleType and Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2010
        ''' Modified by: SA 11/05/2011 - Changed optional parameter pControlID for pControlIDList, a string list
        '''                              containing the list of ControlIDs that have to be deleted for the informed Test/SampleType
        '''              SA 16/05/2011 - After unlinking the Controls not in list from the Test/SampleType, decrement value of field NumberOfControls for it
        '''              RH 11/06/2012 - Added new parameter TestType, due to this function can be used for Standard and ISE Tests
        ''' </remarks>
        Public Function DeleteTestControlsByTestIDAndTestTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                 ByVal pTestType As String, Optional ByVal pSampleType As String = "", _
                                                                 Optional ByVal pControlIDList As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If there is information for a previous Control Lot, it has to be deleted
                        Dim myPrevTestControlLotsDelegate As New PreviousTestControlLotsDelegate
                        myGlobalDataTO = myPrevTestControlLotsDelegate.DeleteByTestIDNEW(dbConnection, pTestID, pTestType, pSampleType, pControlIDList)

                        If (Not myGlobalDataTO.HasError) Then
                            'Remove the Test/Control
                            Dim myTestControlsDAO As New tparTestControlsDAO
                            myGlobalDataTO = myTestControlsDAO.DeleteTestControlByTestIDNEW(dbConnection, pTestID, pTestType, pSampleType, pControlIDList)
                        End If

                        If (Not String.IsNullOrEmpty(pControlIDList)) Then
                            If (Not myGlobalDataTO.HasError) Then
                                'For the Test/SampleType, update value of field NumberOfControls to exclude the number of unlinked Controls
                                If (pTestType = "STD") Then
                                    Dim myTestSamplesDelegate As New TestSamplesDelegate
                                    myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestID, pSampleType)

                                ElseIf (pTestType = "ISE") Then
                                    Dim myISETestSamplesDelegate As New ISETestSamplesDelegate
                                    myGlobalDataTO = myISETestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestID, pSampleType)
                                End If

                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.DeleteTestControlsByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Controls linked to the specified Standard Test and, optionally SampleType and Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 17/05/2010
        ''' Modified by: SA 11/05/2011 - Changed optional parameter pControlID for pControlIDList, a string list
        '''                              containing the list of ControlIDs that have to be deleted for the informed Test/SampleType
        '''              SA 16/05/2011 - After unlinking the Controls not in list from the Test/SampleType, decrement value of field NumberOfControls for it
        '''              SA 15/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function DeleteTestControlsByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                      ByVal pTestType As String, _
                                                      ByVal pTestID As Integer, _
                                                      ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String, _
                                                      Optional ByVal pSampleType As String = "", _
                                                      Optional ByVal pControlIDList As String = "" ) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If there is information for a previous Control Lot, it has to be deleted
                        Dim myPrevTestControlLotsDelegate As New PreviousTestControlLotsDelegate
                        myGlobalDataTO = myPrevTestControlLotsDelegate.DeleteByTestIDNEW(dbConnection, pTestID, pTestType, pSampleType, pControlIDList)

                        If (Not myGlobalDataTO.HasError) Then

                            'DL 11/10/2012. Begin
                            If String.Compare(pAnalyzerID, String.Empty, False) <> 0 And String.Compare(pWorkSessionID, String.Empty, False) <> 0 Then
                                Dim myTestControls As New tparTestControlsDAO
                                myGlobalDataTO = myTestControls.ReadControlByTests(dbConnection, pTestID, pTestType)

                                If Not myGlobalDataTO.HasError Then
                                    Dim myTestControlsData As New TestControlsDS
                                    myTestControlsData = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                                    Dim myVirtualRotorPosDelegate As New VirtualRotorsPositionsDelegate
                                    Dim mySelectedElementInfo As New WSRotorContentByPositionDS
                                    Dim myVirtualRotorPosDS As New VirtualRotorPosititionsDS
                                    Dim myVRotorsDelegate As New VirtualRotorsDelegate

                                    For Each row As TestControlsDS.tparTestControlsRow In myTestControlsData.tparTestControls.Rows
                                        'Delete positions in virtual rotor position and update positions in the rotor when this is not in use
                                        myGlobalDataTO = myVirtualRotorPosDelegate.GetPositionsByControlID(Nothing, row.ControlID)

                                        If Not myGlobalDataTO.HasError Then
                                            myVirtualRotorPosDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                            For Each myvirtualRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions.Rows
                                                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                                myRCPRow = mySelectedElementInfo.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                                myRCPRow.ControlID = myvirtualRow.ControlID
                                                myRCPRow.AnalyzerID = pAnalyzerID
                                                myRCPRow.WorkSessionID = pWorkSessionID
                                                myRCPRow.CellNumber = myvirtualRow.CellNumber
                                                myRCPRow.RingNumber = myvirtualRow.RingNumber
                                                myRCPRow.RotorType = "SAMPLES"
                                                myRCPRow.TubeContent = "CTRL"
                                                mySelectedElementInfo.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)
                                            Next myvirtualRow
                                        End If

                                        For Each myRotorContPosResetElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                                            myRotorContPosResetElem.BeginEdit()
                                            myRotorContPosResetElem.SetElementIDNull()
                                            myRotorContPosResetElem.SetBarCodeInfoNull()
                                            myRotorContPosResetElem.SetBarcodeStatusNull()
                                            myRotorContPosResetElem.SetScannedPositionNull()
                                            myRotorContPosResetElem.MultiTubeNumber = 0
                                            myRotorContPosResetElem.SetTubeTypeNull()
                                            myRotorContPosResetElem.SetTubeContentNull()
                                            myRotorContPosResetElem.RealVolume = 0
                                            myRotorContPosResetElem.RemainingTestsNumber = 0
                                            myRotorContPosResetElem.Status = "FREE"
                                            myRotorContPosResetElem.ElementStatus = "NOPOS"
                                            myRotorContPosResetElem.Selected = False
                                            myRotorContPosResetElem.EndEdit()
                                        Next

                                        'Clear content of selected rotor positions 
                                        Dim myWSRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                                        myGlobalDataTO = myWSRotorContentByPosDAO.Update(dbConnection, mySelectedElementInfo)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'Remove virtual rotor position
                                        myGlobalDataTO = myVRotorsDelegate.DeleteByControlID(Nothing, row.ControlID, pAnalyzerID, pWorkSessionID)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Next

                                End If
                            End If
                            'DL 11/10/2012. End

                            'Remove the Test/Control
                            Dim myTestControlsDAO As New tparTestControlsDAO
                            myGlobalDataTO = myTestControlsDAO.DeleteTestControlByTestIDNEW(dbConnection, pTestID, pTestType, pSampleType, pControlIDList)
                        End If

                        If (pControlIDList <> "") Then
                            If (Not myGlobalDataTO.HasError) Then
                                'For the Test/SampleType, update value of field NumberOfControls to exclude the number of unlinked Controls
                                If (pTestType = "STD") Then
                                    Dim myTestSamplesDelegate As New TestSamplesDelegate
                                    myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestID, pSampleType)
                                Else
                                    Dim myISETestSamplesDelegate As New ISETestSamplesDelegate
                                    myGlobalDataTO = myISETestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestID, pSampleType)
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.DeleteTestControlsByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the basic information of the Controls defined for the specified TestType/TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD or ISE</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlID">Control Identifier. Optional parameter</param>
        ''' <param name="pOnlyActiveControls">When True, it indicates only Controls linked as Active have to be returned, but only if
        '''                                   the specified TestType/TestID/Sample Type has the Quality Control feature enabled</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with data of defined Controls for the informed 
        '''          TestType/TestID/Sample Type</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 21/05/2012 - Added parameter for TestType. The function called in DAO Class will depend on the informed TestType
        ''' </remarks>
        Public Function GetControlsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                       ByVal pSampleType As String, Optional ByVal pControlID As Integer = 0, _
                                       Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControls As New tparTestControlsDAO

                        If (pTestType = "STD") Then
                            resultData = myTestControls.ReadBySTDTestIDAndSampleType(dbConnection, pTestID, pSampleType, pControlID, pOnlyActiveControls)

                        ElseIf (pTestType = "ISE") Then
                            resultData = myTestControls.ReadByISETestIDAndSampleType(dbConnection, pTestID, pSampleType, pControlID, pOnlyActiveControls)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests/SampleTypes linked to the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        '''          linked to the specified Control</returns>
        ''' <remarks>
        ''' Created by:  DL
        ''' Modified by: SA 11/05/2011 - Removed parameter for the SampleType; inform also the Icon in the DS
        '''              SA 15/06/2012 - Get also the ICON for ISE Tests
        ''' </remarks>
        Public Function GetTestControlsByControlIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControls As New tparTestControlsDAO
                        resultData = myTestControls.ReadTestsByControlIDNEW(dbConnection, pControlID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myControlTestsDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                            'Get Icons for Preloaded and User-defined Standard Tests, and also the Icon for ISE Tests
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                            Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                            Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")

                            'According to value of PreloadedTest field for each Test/SampleType, inform field TestTypeIcon with the name of the proper Icon 
                            For Each testControlRow As TestControlsDS.tparTestControlsRow In myControlTestsDS.tparTestControls.Rows
                                If (testControlRow.TestType = "STD") Then
                                    If (testControlRow.PreloadedTest) Then
                                        testControlRow.TestTypeIcon = imageTest
                                    Else
                                        testControlRow.TestTypeIcon = imageUserTest
                                    End If
                                Else
                                    testControlRow.TestTypeIcon = imageISETest
                                End If
                            Next
                            myControlTestsDS.AcceptChanges()
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetTestControlsByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests/SampleTypes linked to the specified list of Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlIDList">List of Control Identifiers</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        '''          linked to the specified list of Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 24/05/2011
        ''' </remarks>
        Public Function GetTestControlsByControlIDListNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlIDList As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControls As New tparTestControlsDAO
                        resultData = myTestControls.ReadTestsByControlIDListNEW(dbConnection, pControlIDList)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetTestControlsByControlIDList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create/update/delete links between Tests/SampleTypes and Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestControlsDS">Typed DS TestControlsDS containing the list of links to create and/or update</param>
        ''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes to unlink from the Control</param>
        ''' <param name="pControlScreen">Flag indicating if the function has been called from the screen of Controls Programming
        '''                              Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 07/04/2011
        ''' Modified by: DL 19/04/2011 - Added optional parameter pControlScreen 
        '''              SA 11/05/2011 - Changed the way of deleting the links for the Controls that are not in the list
        '''                              of created/updated for the Test/SampleType
        '''              SA 16/05/2011 - After unlinking a Control from a Test/SampleType (when pControLScreen is True), decrement value of 
        '''                              field NumberOfControls for it
        '''              RH 11/06/2012 - Use of TestType
        '''              SA 22/06/2012 - When the function has been called from the screen of Controls Programming, variable myNotActiveControls
        '''                              has to be set to TRUE to mark the Control as active for the TestType/Test/SampleType when it has less 
        '''                              than three Controls marked as active
        ''' </remarks>
        Public Function SaveTestControlsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlsDS As TestControlsDS, _
                                            Optional ByVal pDeletedTestControlsDS As SelectedTestsDS = Nothing, _
                                            Optional ByVal pControlScreen As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Variable used to create a list with all created and updated Controls 
                        Dim myControIDlList As String = ""
                        Dim tmpTestControlDS As New TestControlsDS

                        'Validate if there are Controls selected as active
                        Dim myNoActiveControls As Boolean = True
                        If (Not pControlScreen) Then
                            myNoActiveControls = (pTestControlsDS.tparTestControls.Where(Function(a) a.ActiveControl).Count = 0)
                        End If

                        For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows
                            If (Not testControlRow.IsControlIDNull) Then
                                'Import row to a temporal structure
                                tmpTestControlDS.tparTestControls.ImportRow(testControlRow)

                                'Validate if the link between the TestType/Test/SampleType and the Control already exists or if it has to be created
                                myGlobalDataTO = GetControlsNEW(dbConnection, testControlRow.TestType, testControlRow.TestID, testControlRow.SampleType, testControlRow.ControlID)
                                If (myGlobalDataTO.HasError) Then Exit For

                                If (DirectCast(myGlobalDataTO.SetDatos, TestControlsDS).tparTestControls.Count > 0) Then
                                    'Update data of the existing TestType/Test/SampleType and Control
                                    myGlobalDataTO = UpdateTestControlsNEW(dbConnection, tmpTestControlDS)
                                Else
                                    'Create a new TestType/Test/SampleType and Control
                                    myGlobalDataTO = AddTestControlsNEW(dbConnection, tmpTestControlDS, myNoActiveControls)
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Clear the temporal struture and add the ControlID to the string list
                                    tmpTestControlDS.tparTestControls.Clear()
                                    myControIDlList &= testControlRow.ControlID & ","
                                Else
                                    Exit For
                                End If
                            End If
                        Next

                        If (Not pControlScreen) Then
                            If (myControIDlList.Length > 0) Then
                                'Remove the last comma (,)
                                myControIDlList = myControIDlList.Remove(myControIDlList.Length - 1)

                                'For the Test/SampleType, delete all links with Controls that are not in the built string list
                                myGlobalDataTO = DeleteTestControlsByTestIDAndTestTypeNEW(dbConnection, pTestControlsDS.tparTestControls(0).TestID, _
                                                                                          pTestControlsDS.tparTestControls(0).TestType, _
                                                                                          pTestControlsDS.tparTestControls(0).SampleType, _
                                                                                          myControIDlList)
                            End If
                        Else
                            If (Not pDeletedTestControlsDS Is Nothing) Then
                                Dim myTestSamplesDelegate As New TestSamplesDelegate
                                Dim myISETestSamplesDelegate As New ISETestSamplesDelegate

                                For Each deletedTC As SelectedTestsDS.SelectedTestTableRow In pDeletedTestControlsDS.SelectedTestTable.Rows
                                    'If there is a previous saved Lot for the Control, delete the Test/SampleType from it
                                    Dim myPreviousTestControlLotsDelegate As New PreviousTestControlLotsDelegate
                                    myGlobalDataTO = myPreviousTestControlLotsDelegate.DeleteNEW(dbConnection, deletedTC.ControlID, deletedTC.TestID, _
                                                                                                 deletedTC.SampleType, deletedTC.TestType)

                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Remove the link between the Control and the SampleType
                                    Dim myTestControlDAO As New tparTestControlsDAO
                                    myGlobalDataTO = myTestControlDAO.DeleteNEW(dbConnection, deletedTC.ControlID, deletedTC.TestID, _
                                                                                deletedTC.SampleType, deletedTC.TestType)

                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Update value of field NumberOfControls for the unlinked Test/SampleType, according the TestType
                                    If (deletedTC.TestType = "STD") Then
                                        myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControls(dbConnection, deletedTC.TestID, deletedTC.SampleType)

                                    ElseIf (deletedTC.TestType = "ISE") Then
                                        myGlobalDataTO = myISETestSamplesDelegate.UpdateNumOfControls(dbConnection, deletedTC.TestID, deletedTC.SampleType)
                                    End If
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.SaveTestControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''<summary>
        ''' Update values for a TestType/Test/SampleType linked to an specific Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the linked TestType/Test/SampleType to updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 07/04/2011
        ''' </remarks>
        Public Function UpdateTestControlsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControlsDAO As New tparTestControlsDAO
                        myGlobalDataTO = myTestControlsDAO.UpdateNEW(dbConnection, pTestControlDS)

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.UpdateTestControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From identifiers of Test/SampleType and Control/Lot in QC Module, verifies if the link between the Test/SampleType
        ''' and the Control still exists in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if the link between the Test/SampleType and the Control
        '''          still exists in table tparTestControls</returns>
        ''' <remarks>
        ''' Created by:  SA 04/01/2012 
        ''' </remarks>
        Public Function VerifyLinkByQCModuleIDsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                   ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControls As New tparTestControlsDAO
                        resultData = myTestControls.VerifyLinkByQCModuleIDsNEW(dbConnection, pQCTestSampleID, pQCControlLotID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.VerifyLinkByQCModuleIDs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TEMPORARY - TO SIMULATE QC RESULTS"
        ''' <summary>
        ''' Get all Tests/SampleTypes having QC active and at least a linked Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Tests/SampleTypes having
        '''          QC active and at least a linked Control/Lot</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2012
        ''' Modified by: SA 29/05/2012 - Inform the icon according value of fields TestType and PreloadedTest    
        ''' </remarks>
        Public Function GetAllNEW(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControlsDAO As New tparTestControlsDAO
                        resultData = myTestControlsDAO.ReadAllNEW(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestControlsDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                            'Get Icons for Preloaded and User-defined Standard Tests, and for ISE Tests
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                            Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                            Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")

                            'According value of TestType and PreloadedTest fields for each Test/SampleType, inform field TestTypeIcon with the name of the proper Icon 
                            For Each testControlRow As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls.Rows
                                testControlRow.BeginEdit()
                                If (String.Compare(testControlRow.TestType, "STD", False) = 0) Then
                                    If (testControlRow.PreloadedTest) Then
                                        testControlRow.TestTypeIcon = imageTest
                                    Else
                                        testControlRow.TestTypeIcon = imageUserTest
                                    End If
                                ElseIf (String.Compare(testControlRow.TestType, "ISE", False) = 0) Then
                                    testControlRow.TestTypeIcon = imageISETest
                                End If
                                testControlRow.EndEdit()
                            Next

                            resultData.SetDatos = myTestControlsDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of active Controls/Lots linked to the specified TestType/TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD or ISE</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of active Controls/Lots linked to the
        '''          informed Standard or ISE TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2012
        ''' Modified by: SA 29/05/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function GetAdditionalInformationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, _
                                                    ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestControls As New tparTestControlsDAO
                        If (String.Compare(pTestType, "STD", False) = 0) Then
                            resultData = myTestControls.ReadAdditionalInfoForSTDTests(dbConnection, pTestID, pSampleType)
                        ElseIf (String.Compare(pTestType, "ISE", False) = 0) Then
                            resultData = myTestControls.ReadAdditionalInfoForISETests(dbConnection, pTestID, pSampleType)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetAdditionalInformationNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Link a Test/SampleType to an specific Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the Test/SampleType to link</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  DL 05/04/2011
        '''' Modified by: TR 07/04/2011 - Implemented creation logic
        ''''              SA 16/05/2011 - After linking a Control to a Test/SampleType, increment value of field NumberOfControls for it
        ''''              SA 06/10/2011 - If for the Test/SampleType there are less than three linked Controls marked as active, the new one is 
        ''''                              create as active
        ''''              TR 12/01/2012 - Add optional parammeter pNoActiveControl to validate if new test control has selected active controls Screen module.
        '''' </remarks>
        'Public Function AddTestControlsOLD(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                ByVal pTestControlDS As TestControlsDS, Optional ByVal pNoActiveControl As Boolean = True) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControlsDAO As New tparTestControlsDAO

        '                myGlobalDataTO = myTestControlsDAO.CountActiveByTestIDAndSampleType(dbConnection, _
        '                                 pTestControlDS.tparTestControls(0).TestID, pTestControlDS.tparTestControls(0).SampleType)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    'Validate if Control Screen 
        '                    If pNoActiveControl Then
        '                        'If the number of active Controls for the Test/SampleType is less than 3, then the new linked Control is marked as active
        '                        If (DirectCast(myGlobalDataTO.SetDatos, Integer) < 3) Then
        '                            pTestControlDS.tparTestControls(0).BeginEdit()
        '                            pTestControlDS.tparTestControls(0).ActiveControl = True
        '                            pTestControlDS.tparTestControls(0).EndEdit()
        '                        End If
        '                    End If

        '                    'Create the new link between the Test/SampleType and the Control
        '                    myGlobalDataTO = myTestControlsDAO.Create(dbConnection, pTestControlDS)
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'Update value of field NumberOfControls for the Test/SampleType 
        '                        Dim myTestSamplesDelegate As New TestSamplesDelegate
        '                        myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControls(dbConnection, pTestControlDS.tparTestControls.First.TestID, _
        '                                                                                   pTestControlDS.tparTestControls.First.SampleType)
        '                    End If

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'When the Database Connection was opened locally, then the Commit is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    Else
        '                        'When the Database Connection was opened locally, then the Rollback is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.AddTestControls", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete all Controls linked to the specified Standard Test and, optionally SampleType and Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        '''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 17/05/2010
        '''' Modified by: SA 11/05/2011 - Changed optional parameter pControlID for pControlIDList, a string list
        ''''                              containing the list of ControlIDs that have to be deleted for the informed Test/SampleType
        ''''              SA 16/05/2011 - After unlinking the Controls not in list from the Test/SampleType, decrement value of field NumberOfControls for it
        '''' </remarks>
        'Public Function DeleteTestControlsByTestIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
        '                                           Optional ByVal pControlIDList As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'If there is information for a previous Control Lot, it has to be deleted
        '                Dim myPrevTestControlLotsDelegate As New PreviousTestControlLotsDelegate
        '                myGlobalDataTO = myPrevTestControlLotsDelegate.DeleteByTestIDOLD(dbConnection, pTestID, pSampleType, pControlIDList)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Remove the Test/Control
        '                    Dim myTestControlsDAO As New tparTestControlsDAO
        '                    myGlobalDataTO = myTestControlsDAO.DeleteTestControlByTestIDold(dbConnection, pTestID, pSampleType, pControlIDList)
        '                End If

        '                If (pControlIDList <> "") Then
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'For the Test/SampleType, update value of field NumberOfControls to exclude the number of unlinked Controls
        '                        Dim myTestSamplesDelegate As New TestSamplesDelegate
        '                        myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControlsOLD(dbConnection, pTestID, pSampleType)
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.DeleteTestControlsByTestID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the basic information of the Controls defined for the specified Test and Sample Type
        '''' </summary>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with data of defined Controls 
        ''''          for the informed Test and Sample Type</returns>
        '''' <remarks></remarks>
        'Public Function GetControlsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                            Optional ByVal pControlID As Integer = 0, Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadByTestIDAndSampleTypeOLD(dbConnection, pTestID, pSampleType, pControlID, pOnlyActiveControls)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetControls", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes linked to the specified Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        ''''          linked to the specified Control</returns>
        '''' <remarks>
        '''' Created by:  DL
        '''' Modified by: SA 11/05/2011 - Removed parameter for the SampleType; inform also the Icon in the DS
        '''' </remarks>
        'Public Function GetTestControlsByControlIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadTestsByControlIDOLD(dbConnection, pControlID)

        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    Dim myControlTestsDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

        '                    'Get Icons for Preloaded and User-defined Standard Tests
        '                    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
        '                    Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
        '                    Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")

        '                    'According value of PreloadedTest field for each Test/SampleType, inform field TestTypeIcon with the name of the proper Icon 
        '                    For Each testControlRow As TestControlsDS.tparTestControlsRow In myControlTestsDS.tparTestControls.Rows
        '                        testControlRow.BeginEdit()
        '                        If (testControlRow.PreloadedTest) Then
        '                            testControlRow.TestTypeIcon = imageTest
        '                        Else
        '                            testControlRow.TestTypeIcon = imageUserTest
        '                        End If
        '                        testControlRow.EndEdit()
        '                    Next
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetTestControlsByControlID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes linked to the specified list of Controls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlIDList">List of Control Identifiers</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        ''''          linked to the specified list of Controls</returns>
        '''' <remarks>
        '''' Created by:  SA 24/05/2011
        '''' </remarks>
        'Public Function GetTestControlsByControlIDListOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlIDList As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadTestsByControlIDListOLD(dbConnection, pControlIDList)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetTestControlsByControlIDList", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Create/update/delete links between Tests/SampleTypes and Controls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestControlsDS">Typed DS TestControlsDS containing the list of links to create and/or update</param>
        '''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes to unlink from the Control</param>
        '''' <param name="pControlScreen">Flag indicating if the function has been called from the screen of Controls Programming
        ''''                              Optional parameter</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 07/04/2011
        '''' Modified by: DL 19/04/2011 - Added optional parameter pControlScreen 
        ''''              SA 11/05/2011 - Changed the way of deleting the links for the Controls that are not in the list
        ''''                              of created/updated for the Test/SampleType
        ''''              SA 16/05/2011 - After unlinking a Control from a Test/SampleType (when pControLScreen is True), decrement value of 
        ''''                              field NumberOfControls for it
        '''' </remarks>
        'Public Function SaveTestControlsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlsDS As TestControlsDS, _
        '                                 Optional ByVal pDeletedTestControlsDS As SelectedTestsDS = Nothing, _
        '                                 Optional ByVal pControlScreen As Boolean = False) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Variable used to create a list with all created and updated Controls 
        '                Dim myControIDlList As String = ""
        '                Dim tmpTestControlDS As New TestControlsDS

        '                'TR 12/01/2012 -Variable used on creationg  to validate if there are any active control.
        '                Dim myNoActiveControls As Boolean = False
        '                'Validate if there are any active control
        '                myNoActiveControls = (pTestControlsDS.tparTestControls.Where(Function(a) a.ActiveControl).Count = 0)
        '                'TR 12/01/2012 -END.
        '                For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows

        '                    If Not testControlRow.IsControlIDNull Then
        '                        'Import row to a temporal structure
        '                        tmpTestControlDS.tparTestControls.ImportRow(testControlRow)

        '                        'Validate if the link between the Test/SampleType and the Control already exists or if it has to be created
        '                        myGlobalDataTO = GetControlsOLD(dbConnection, testControlRow.TestID, testControlRow.SampleType, testControlRow.ControlID)
        '                        If (myGlobalDataTO.HasError) Then Exit For

        '                        If (DirectCast(myGlobalDataTO.SetDatos, TestControlsDS).tparTestControls.Count > 0) Then
        '                            'Update data of the existing Test/Control
        '                            myGlobalDataTO = UpdateTestControlsOLD(dbConnection, tmpTestControlDS)
        '                        Else
        '                            'Create a new Test/Control
        '                            myGlobalDataTO = AddTestControlsOLD(dbConnection, tmpTestControlDS, myNoActiveControls)
        '                        End If

        '                        If (Not myGlobalDataTO.HasError) Then
        '                            'Clear the temporal struture and add the ControlID to the string list
        '                            tmpTestControlDS.tparTestControls.Clear()
        '                            myControIDlList &= testControlRow.ControlID & ","
        '                        Else : Exit For
        '                        End If
        '                    End If

        '                Next

        '                If (Not pControlScreen) Then
        '                    If (myControIDlList.Length > 0) Then
        '                        'Remove the last comma (,)
        '                        myControIDlList = myControIDlList.Remove(myControIDlList.Length - 1)

        '                        'For the Test/SampleType, delete all links with Controls that are not in the built string list
        '                        myGlobalDataTO = DeleteTestControlsByTestIDOLD(dbConnection, pTestControlsDS.tparTestControls(0).TestID, _
        '                                                                    pTestControlsDS.tparTestControls(0).SampleType, myControIDlList)
        '                    End If
        '                Else
        '                    If (Not pDeletedTestControlsDS Is Nothing) Then
        '                        For Each deletedTC As SelectedTestsDS.SelectedTestTableRow In pDeletedTestControlsDS.SelectedTestTable.Rows
        '                            'If there is a previous saved Lot for the Control, delete the Test/SampleType from it
        '                            Dim myPreviousTestControlLotsDelegate As New PreviousTestControlLotsDelegate
        '                            myGlobalDataTO = myPreviousTestControlLotsDelegate.DeleteOLD(dbConnection, deletedTC.ControlID, deletedTC.TestID, deletedTC.SampleType)
        '                            If (myGlobalDataTO.HasError) Then Exit For

        '                            'Remove the link between the Control and the SampleType
        '                            Dim myTestControlDAO As New tparTestControlsDAO
        '                            myGlobalDataTO = myTestControlDAO.DeleteOLD(dbConnection, deletedTC.ControlID, deletedTC.TestID, deletedTC.SampleType)
        '                            If (myGlobalDataTO.HasError) Then Exit For

        '                            'Update value of field NumberOfControls for the unlinked Test/SampleType 
        '                            Dim myTestSamplesDelegate As New TestSamplesDelegate
        '                            myGlobalDataTO = myTestSamplesDelegate.UpdateNumOfControlsOLD(dbConnection, deletedTC.TestID, deletedTC.SampleType)
        '                        Next
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
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
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.SaveTestControls", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        ''''<summary>
        '''' Update values for a Test/SampleType linked to an specific Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the linked Test/SampleType to updated</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 07/04/2011
        '''' </remarks>
        'Public Function UpdateTestControlsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControlsDAO As New tparTestControlsDAO
        '                myGlobalDataTO = myTestControlsDAO.UpdateOLD(dbConnection, pTestControlDS)

        '                If (Not myGlobalDataTO.HasError) Then
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
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.UpdateTestControls", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' From identifiers of Test/SampleType and Control/Lot in QC Module, verifies if the link between the Test/SampleType
        '''' and the Control still exists in table tparTestControls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a boolean value indicating if the link between the Test/SampleType and the Control
        ''''          still exists in table tparTestControls</returns>
        '''' <remarks>
        '''' Created by:  SA 04/01/2012 
        '''' </remarks>
        'Public Function VerifyLinkByQCModuleIDsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                        ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.VerifyLinkByQCModuleIDsOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.VerifyLinkByQCModuleIDs", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        'Public Function GetAllOLD(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadAllOLD(dbConnection)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetAll", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        'Public Function GetAdditionalInformation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                         ByVal pSampleType As String, ByVal pControlID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadAdditionalInformation(dbConnection, pTestID, pSampleType, pControlID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestControlsDelegate.GetAdditionalInformation", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace



