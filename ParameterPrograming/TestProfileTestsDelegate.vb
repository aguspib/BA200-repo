Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Public Class TestProfileTestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Modify the list of Tests included in a Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsList">List of Tests to include in the Profile</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 28/06/2010 
        ''' </remarks>
        Public Shared Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsList As TestProfileTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pTestsList.tparTestProfileTests.Rows.Count > 0) Then
                            'Get the Test Profile ID
                            Dim profileID As Integer = pTestsList.tparTestProfileTests(0).TestProfileID
                            Dim myTestProfileTestsDAO As New tparTestProfileTestsDAO

                            'Delete all Tests currently included in the Test Profile
                            resultData = myTestProfileTestsDAO.Delete(dbConnection, profileID)

                            If (Not resultData.HasError) Then
                                'Add the Tests in the DataSet to the Test Profile
                                resultData = myTestProfileTestsDAO.Create(dbConnection, pTestsList)
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Test from all Test Profiles in which it is included. When a SampleType is informed, 
        ''' it means that the Test have to be deleted only of all the Test Profiles defined for this SampleType.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 08/07/2010 - The function in this Delegate was missing (DAO was directly called)
        ''' </remarks>
        Public Shared Function DeleteByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfileTestsDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfileTestsDAO.DeleteByTestIDSampleType(dbConnection, pTestID, pSampleType, pTestType)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Remove an specific Test from the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <param name="pTestType">Test Type</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/01/2011
        ''' </remarks>
        Public Function DeleteByTestIDAndTestProfileID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pTestID As Integer, ByVal pTestProfileID As Integer, _
                                                 ByVal pTestType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfileTestsDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfileTestsDAO.DeleteByTestIDAndTestProfileID(dbConnection, pTestID, pTestProfileID, pTestType)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all different Test Types included in the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Identifier of the Test Profile</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list of 
        '''          different Test Types included in the specified Test Profile</returns>
        ''' <remarks>
        ''' Created by:  DL 14/10/2010
        ''' Modified by: SA 18/10/2010 - Function moved here from TestProfilesDelegate
        ''' </remarks>
        Public Shared Function GetTestTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfilesDAO.GetTestTypes(dbConnection, pTestProfileID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.GetTestTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Tests (all Test Types) included in a specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <returns>GlobalDataTO containing a typed Dataset TestProfileTestsDS the list of Tests included
        '''          in the specified Test Profile plus the Name and Position of each one of them</returns>
        ''' <remarks>
        ''' Created by: SA 19/10/2010
        ''' </remarks>
        Public Shared Function ReadByTestProfileID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfilesDAO.ReadByTestProfileID(dbConnection, pTestProfileID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.ReadByTestProfileID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the TestProfiles in which the specified Test (with optionally, the SampleType) is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list
        '''          of Test Profiles in which the Test is included</returns>
        ''' <remarks>
        ''' Created by:  TR 24/11/2010
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                     Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        myGlobalDataTO = myTestProfilesDAO.ReadByTestID(dbConnection, pTestID, pSampleType, pTestType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Profiles defined for SampleTypes diferent of the specified ones and containing the specified Test.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">List of SampleType to exclude from the search</param>
        ''' <param name="pTestType">Type of Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list of Test Profiles</returns>
        ''' <remarks>
        ''' Created by: TR 13/01/2011
        ''' </remarks>
        Public Function ReadByTestIDSpecial(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                            Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        myGlobalDataTO = myTestProfilesDAO.ReadByTestIDSpecial(dbConnection, pTestID, pSampleType, pTestType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.ReadByTestIDSpecial", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Tests (all Test Types) using the specified SampleType. If a TestProfile is specified,
        ''' Tests included in it are not returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type to filter the Tests</param>
        ''' <param name="pTestProfileID">Optional parameter. When informed, it indicates the Tests included
        '''                              in the informed Test Profile must not be returned</param>
        ''' <returns>GlobalDataTO containing a typed Dataset TestProfileTestsDS with the list of Tests (of all 
        '''          Test Types) not included in the specified Profile</returns>
        ''' <remarks>
        ''' Created by:  SA 20/10/2010
        ''' </remarks>
        Public Shared Function ReadTestsNotInProfile(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, _
                                              Optional ByVal pTestProfileID As Integer = 0) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfilesDAO.ReadTestsNotInProfile(dbConnection, pSampleType, pTestProfileID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.ReadTestsNotInProfile", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns all Test Profiles info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by: RH 30/11/2011
        ''' </remarks>
        Public Function GetTestProfilesForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String, _
                                                 Optional ByVal SelectedProfiles As List(Of Integer) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New tparTestProfileTestsDAO
                        resultData = myTestProfilesDAO.GetTestProfilesForReport(dbConnection, AppLang, SelectedProfiles)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfileTestsDelegate.GetTestProfilesForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

    End Class

End Namespace