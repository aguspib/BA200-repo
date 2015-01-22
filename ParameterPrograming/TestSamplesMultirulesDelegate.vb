Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Public Class TestSamplesMultirulesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' For a Test/SampleType, insert all available MultiRules with the last Selected status for each one of them
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesMultirulesDS">Typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''                                        of each one of them for an specific Test/SampleType</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' </remarks>
        Public Function AddMultiRulesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTmpTestSamplesMultirules As New TestSamplesMultirulesDS
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO

                        For Each multiRuleRow As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow In pTestSamplesMultirulesDS.tparTestSamplesMultirules.Rows
                            'Move this record to a temporary DataSet
                            myTmpTestSamplesMultirules.tparTestSamplesMultirules.ImportRow(multiRuleRow)

                            'Create the link
                            myGlobalDataTO = myTestSampleMultirulesDAO.Create(dbConnection, myTmpTestSamplesMultirules)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Clear the temporary DataSet
                            myTmpTestSamplesMultirules.tparTestSamplesMultirules.Clear()
                        Next

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
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.AddMultiRules", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the link between all available Westgard Rules and an specific TestID (and SampleType, optionally) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 06/04/2011
        ''' </remarks>
        Public Function DeleMultiRulesByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                               Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO
                        myGlobalDataTO = myTestSampleMultirulesDAO.DeleteByTestIDSampleType(pDBConnection, pTestID, pSampleType)

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
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.AddMultiRules", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of available Westgard Rules and the status of each one of them for the specified Test/SampleType. Optionally,
        ''' the function can return also the Rules selected to be applied
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pOnlyActiveRules">When TRUE, it indicates that only the Westgard Rules selected to be applied 
        '''                                for the Test/Sample Type will be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''          of each one of them for the specified Test/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: TR 17/05/2011 - Added optional parameter pOnlyActiveRules to get only the group of Westgard Rules
        '''                              selected to be applied for the Test/SampleType
        ''' </remarks>
        Public Function GetByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                 ByVal pSampleType As String, Optional ByVal pOnlyActiveRules As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO
                        myGlobalDataTO = myTestSampleMultirulesDAO.ReadByTestIDAndSampleType(pDBConnection, pTestID, pSampleType, pOnlyActiveRules)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.GetByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a Test/SampleType, insert all available MultiRules with the last Selected status for each one of them, deleting 
        ''' first the previous Selected status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesMultirulesDS">Typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''                                        of each one of them for an specific Test/SampleType</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 06/04/2011
        ''' Modified by: SA 15/05/2012 - Removed call to function GetByTestIDAndSampleType, it is not needed
        ''' 
        ''' </remarks>
        Public Function SaveMultiRules(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pTestSamplesMultirulesDS.tparTestSamplesMultirules.Count > 0) Then
                            'Delete current Selected status of all available Westgard Rules for the Test/SampleType
                            myGlobalDataTO = DeleMultiRulesByTestID(dbConnection, pTestSamplesMultirulesDS.tparTestSamplesMultirules(0).TestID, _
                                                                    pTestSamplesMultirulesDS.tparTestSamplesMultirules(0).SampleType)

                            If (Not myGlobalDataTO.HasError) Then
                                'Insert the new Selected status of all available Westgard Rules for the Test/SampleType

                                myGlobalDataTO = AddMultiRulesNEW(dbConnection, pTestSamplesMultirulesDS)
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
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.SaveMultiRules", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "NEW QC FUNCTIONS"

        ''' <summary>
        ''' For a Test/SampleType, insert all available MultiRules with the last Selected status for each one of them
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesMultirulesDS">Typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''                                        of each one of them for an specific Test/SampleType</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' </remarks>
        Public Function AddMultiRulesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTmpTestSamplesMultirules As New TestSamplesMultirulesDS
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO

                        For Each multiRuleRow As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow In pTestSamplesMultirulesDS.tparTestSamplesMultirules.Rows
                            'Move this record to a temporary DataSet
                            myTmpTestSamplesMultirules.tparTestSamplesMultirules.ImportRow(multiRuleRow)

                            'Create the link
                            myGlobalDataTO = myTestSampleMultirulesDAO.CreateNEW(dbConnection, myTmpTestSamplesMultirules)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Clear the temporary DataSet
                            myTmpTestSamplesMultirules.tparTestSamplesMultirules.Clear()
                        Next

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
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.AddMultiRules", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the link between all available Westgard Rules and an specific TestType/TestID (and SampleType, optionally) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 06/04/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function DeleMultiRulesByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                  Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO
                        myGlobalDataTO = myTestSampleMultirulesDAO.DeleteByTestIDSampleTypeNEW(pDBConnection, pTestType, pTestID, pSampleType)

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
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.AddMultiRules", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of available Westgard Rules and the status of each one of them for the specified Test/SampleType. Optionally,
        ''' the function can return also the Rules selected to be applied
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pOnlyActiveRules">When TRUE, it indicates that only the Westgard Rules selected to be applied 
        '''                                for the Test/Sample Type will be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''          of each one of them for the specified Test/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: TR 17/05/2011 - Added optional parameter pOnlyActiveRules to get only the group of Westgard Rules
        '''                              selected to be applied for the Test/SampleType
        '''              SA 05/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function GetByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                    ByVal pSampleType As String, Optional ByVal pOnlyActiveRules As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSampleMultirulesDAO As New tparTestSamplesMultirulesDAO
                        myGlobalDataTO = myTestSampleMultirulesDAO.ReadByTestIDAndSampleTypeNEW(pDBConnection, pTestType, pTestID, pSampleType, pOnlyActiveRules)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.GetByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a Test/SampleType, insert all available MultiRules with the last Selected status for each one of them, deleting 
        ''' first the previous Selected status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesMultirulesDS">Typed DataSet TestSamplesMultirulesDS with the list of available Rules and the status
        '''                                        of each one of them for an specific Test/SampleType</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 06/04/2011
        ''' Modified by: SA 15/05/2012 - Removed call to function GetByTestIDAndSampleType, it is not needed
        '''              RH 11/06/2012 - Added parameter TestType to new version of DeleMultiRulesByTestID delegates
        ''' </remarks>
        Public Function SaveMultiRulesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If (pTestSamplesMultirulesDS.tparTestSamplesMultirules.Count > 0) Then
                            'Delete current Selected status of all available Westgard Rules for the Test/SampleType
                            myGlobalDataTO = DeleMultiRulesByTestIDNEW( _
                                    dbConnection, _
                                    pTestSamplesMultirulesDS.tparTestSamplesMultirules(0).TestType, _
                                    pTestSamplesMultirulesDS.tparTestSamplesMultirules(0).TestID, _
                                    pTestSamplesMultirulesDS.tparTestSamplesMultirules(0).SampleType)

                            If (Not myGlobalDataTO.HasError) Then
                                'Insert the new Selected status of all available Westgard Rules for the Test/SampleType
                                myGlobalDataTO = AddMultiRulesNEW(dbConnection, pTestSamplesMultirulesDS)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestSamplesMultirulesDelegate.SaveMultiRules", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace

