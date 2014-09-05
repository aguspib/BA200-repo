Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
'Imports System.Configuration
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.TO
'Imports Biosystems.Ax00.BL.Framework
'Imports System.Drawing

Namespace Biosystems.Ax00.BL

    Public Class TestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Test on the Tests Table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestDS">Typed Dataset TestsDS containing the data of the Test to add</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 02/03/2010
        ''' AG 01/09/2014 - BA-1869 when new STD test is created the CustomPosition informed = MAX current value + 1
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestDS As TestsDS, Optional pIsPreloadedTest As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO
                        If (pTestDS.tparTests.Rows.Count > 0) Then
                            'Generate the next TestID 
                            myGlobalDataTO = GetNextTestID(dbConnection, pIsPreloadedTest)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                pTestDS.tparTests(0).TestID = CType(myGlobalDataTO.SetDatos, Integer)

                                'Get the next Test Position
                                myGlobalDataTO = GetNextTestPosition(dbConnection)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    pTestDS.tparTests(0).TestPosition = CType(myGlobalDataTO.SetDatos, Integer)

                                    'AG 01/09/2014 - BA-1869 Get the next Custom Position
                                    ''Finally, create the new Test
                                    'myGlobalDataTO = myTestsDAO.Create(dbConnection, pTestDS)
                                    myGlobalDataTO = GetNextCustomPosition(dbConnection)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        pTestDS.tparTests(0).CustomPosition = CType(myGlobalDataTO.SetDatos, Integer)

                                        'Finally, create the new Test
                                        myGlobalDataTO = myTestsDAO.Create(dbConnection, pTestDS)
                                    End If
                                    'AG 01/09/2014 - BA-1869

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all basic parameters of the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing all basic parameters 
        '''          of the informed Test</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 09/02/2010 - Added parameter for DBConnection; return a GlobalDataTO
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.Read(dbConnection, pTestID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.Read", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Read all information needed for Biochemical calculations for an Standard Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with all data needed for Biochemical calculations</returns>
        ''' <remarks>
        ''' Created by:  AG 03/07/2012
        ''' </remarks>
        Public Function ReadForCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.ReadForCalculations(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' ???
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pSampleType"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 09/12/2010
        ''' </remarks>
        Public Function ReadByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.ReadByTestIDSampleType(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ReadByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Standard Biosystems Tests (PreloadedTest = TRUE) or all Standard User Tests (PreloadedTest = FALSE),
        ''' depending on value of parameter pPreloadedTest
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreloadedTest">True to get Biosystems Tests; False to get User Tests</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of obtained Tests</returns>
        ''' <remarks>
        ''' Created by: TR 23/03/2011
        ''' </remarks>
        Public Function ReadByPreloadedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreloadedTest As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.ReadByPreloadedTest(dbConnection, pPreloadedTest)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ReadByPreloadedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of an Standard Test searching by Short Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pShortName">Test Short Name</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing all basic parameters 
        '''          of the informed Test</returns>
        ''' <remarks>
        ''' Created by:  TR 16/11/2010
        ''' </remarks>
        Public Function ReadByShortName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pShortName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.ReadByShortName(dbConnection, pShortName)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ReadByShortName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search test data for the informed Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestName">Test Name to search by</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing data of the 
        '''          informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2010
        ''' </remarks>
        Public Function ExistsTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.ReadByTestName(dbConnection, pTestName)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ExistsTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values of the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestDS">Typed DataSet TestsDS containing the data of the Test to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 02/03/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestDS As TestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        myGlobalDataTO = mytparTestsDAO.Update(dbConnection, pTestDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' Delete the specified Test 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Identifier of the Test to delete</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by: TR 16/03/2010
        '''' </remarks>
        'Private Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim mytparTestsDAO As New tparTestsDAO
        '                'Delete Test.
        '                myGlobalDataTO = mytparTestsDAO.Delete(dbConnection, pTestID)
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
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.Delete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function

        ''' <summary>
        ''' Update the Test Position for all Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pListTestPositionTO">List containing the TestPosition for each TestID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 29/03/2010
        ''' </remarks>
        Public Function UpdateTestPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pListTestPositionTO As List(Of TestPositionTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        myGlobalDataTO = mytparTestsDAO.UpdateTestPosition(dbConnection, pListTestPositionTO)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Standard Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Standard Tests that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Standard Tests
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparTestsDAO
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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
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
                        Dim myDAO As New tparTestsDAO
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateInUseByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCustomizedTestSelection">Default FALSE same order as until v3.0.2. When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing data of all Standard Tests</returns>
        ''' <remarks>
        ''' Created by: TR 08/02/2010
        ''' AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pCustomizedTestSelection As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()
                        myGlobalDataTO = myTestsDAO.ReadAll(dbConnection, pCustomizedTestSelection) 'AG 29/08/2014 BA-1869 - Inform pForTestSelectionScreen
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all tests with information if has contaminations defined (as contaminator)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/12/2010
        ''' </remarks>
        Public Function ReadAllContaminatorsList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()
                        myGlobalDataTO = myTestsDAO.ReadAllContaminators(dbConnection)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ReadAllContaminatorsList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Reading Number for a Test ID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: GDS 27/04/2010
        ''' </remarks>
        Public Function GetTestReadingNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()
                        myGlobalDataTO = myTestsDAO.Read(dbConnection, pTestID)

                        If Not myGlobalDataTO.HasError Then
                            If Not DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).IsSecondReadingCycleNull Then
                                myGlobalDataTO.SetDatos = DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).SecondReadingCycle
                            Else
                                myGlobalDataTO.SetDatos = DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).FirstReadingCycle
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetTestReadingNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the next Identifier for an User-defined Standard Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an Integer value with the next identifier for an User-defined Standard Test</returns>
        ''' <remarks>
        ''' Created by: TR 03/03/2010
        ''' MODIFIED BY: TR 05/02/2013 -Add new parameter isPreloaded use to generate the TestID for preloaded(factory) test.
        ''' </remarks>
        Public Function GetNextTestID(ByVal pDBConnection As SqlClient.SqlConnection, Optional pIsPreloaded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()
                        Dim mySWParameters As New SwParametersDelegate()

                        'TR 05/02/2013 -New implementation for uhpdate process. to generate preloaded test ID.
                        If Not pIsPreloaded Then
                            myGlobalDataTO = mySWParameters.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.FIRST_USER_TESTID.ToString, Nothing)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim mySwParametersDS As ParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)

                                If (mySwParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                                    'AG 06/11/2012 - START: BA400 Change Test / Calculated Test programming on server Database
                                    'ACTIVATE only when you are creating new BioSystems tests. When finish comment these lines
                                    'mySwParametersDS.tfmwSwParameters(0).BeginEdit()
                                    'mySwParametersDS.tfmwSwParameters(0).ValueNumeric = 0
                                    'mySwParametersDS.tfmwSwParameters(0).EndEdit()
                                    'mySwParametersDS.tfmwSwParameters.AcceptChanges()
                                    'AG 06/11/2012 - END: BA400 Change Test / Calculated Test programming on server Database

                                    myGlobalDataTO = myTestsDAO.GetLastTestID(dbConnection, CType(mySwParametersDS.tfmwSwParameters(0).ValueNumeric, Integer))

                                    If (Not myGlobalDataTO.HasError) Then
                                        If (myGlobalDataTO.SetDatos Is Nothing OrElse myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                            myGlobalDataTO.SetDatos = CType(mySwParametersDS.tfmwSwParameters(0).ValueNumeric, Integer)
                                        Else
                                            myGlobalDataTO.SetDatos = CType(myGlobalDataTO.SetDatos, Integer) + 1
                                        End If
                                    End If
                                End If

                            End If

                        Else
                            myGlobalDataTO = myTestsDAO.GetLastTestID(pDBConnection, 0, pIsPreloaded)
                            If (Not myGlobalDataTO.HasError) Then
                                If (myGlobalDataTO.SetDatos Is Nothing OrElse myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                    myGlobalDataTO.SetDatos = 1
                                Else
                                    myGlobalDataTO.SetDatos = CType(myGlobalDataTO.SetDatos, Integer) + 1
                                End If
                            End If
                        End If
                        'TR 05/02/2013 -END.


                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetNextTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the next Test Position 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an Integer value with the next Test Position</returns>
        ''' <remarks>
        ''' Created by: TR 03/03/2010
        ''' </remarks>
        Public Function GetNextTestPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()

                        myGlobalDataTO = myTestsDAO.GetLastTestPosition(dbConnection)
                        If (Not myGlobalDataTO.HasError) Then
                            If (myGlobalDataTO.SetDatos Is Nothing OrElse myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                myGlobalDataTO.SetDatos = 1
                            Else
                                myGlobalDataTO.SetDatos = CType(myGlobalDataTO.SetDatos, Integer) + 1
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetNextTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the next custom Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an Integer value with the next Custom Position</returns>
        ''' <remarks>
        ''' Created by: AG 01/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetNextCustomPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO()

                        myGlobalDataTO = myTestsDAO.GetLastCustomPosition(dbConnection)
                        If (Not myGlobalDataTO.HasError) Then
                            If (myGlobalDataTO.SetDatos Is Nothing OrElse myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                myGlobalDataTO.SetDatos = 1
                            Else
                                myGlobalDataTO.SetDatos = CType(myGlobalDataTO.SetDatos, Integer) + 1
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetNextCustomPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of all Standard Tests for the specified Sample Type. When a SampleClass is informed, then 
        ''' only Test/SampleType with Controls or Experimental Calibrators defined are returned 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleTypeCode">Sample Type to filter the Tests</param>
        ''' <param name="pSampleClass">Optional parameter. When informed, get only Test/SampleType for which Controls or Calibrators
        '''                            have been defined</param>
        ''' <param name="pCustomizedTestSelection">Default FALSE same order as until v3.0.2. When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing basic data of the obtained Standard Tests</returns>
        ''' <remarks>
        ''' Modified by: SA 27/06/2010 - Changed the way of getting the Icon Path for System and User Tests
        '''              DL 14/10/2010 - Added new optional parameter for the Test Type and pass it when calling the DAO function
        '''              SA 18/10/2010 - Get Icon according the TestType
        '''              SA 22/10/2010 - Remove the last two changes applied: this function is used only to load the available STD Tests
        '''                              filtered by SampleType for the informed SampleClass. Removed also optional parameter for TestProfileID
        '''                              due to this function is not called from the screen of Programming Test Profiles anymore. Name changed 
        '''                              to GetBySampleType. Removed the getting of the ICON according the TestType
        '''              AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen (define 2 last parameters as required)
        ''' </remarks>
        Public Function GetBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleTypeCode As String, _
                                         ByVal pSampleClass As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testsDAO As New tparTestsDAO
                        resultData = testsDAO.GetBySampleType(dbConnection, pSampleTypeCode, pSampleClass, pCustomizedTestSelection) 'AG 29/08/2014 BA-1869 - Inform pCustomizedTestSelection
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create or Update a Test with all its related data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestDS">Typed DataSet TestsDS with Test data</param>
        ''' <param name="pTestSamplesDS">Typed DataSet TestSamplesDS with Test data by SampleType</param>
        ''' <param name="pReagentsDS">Typed DataSet ReagentsDS with data of the Reagents used for the Test</param>
        ''' <param name="pTestReagentesDS">Typed DataSet TestReagentsDS with data of link between Reagents and the Test</param>
        ''' <param name="pTestReagentsVolumesDS">Typed DataSet TestReagentsVolumesDS with Reagent volume needed for each Test/SampleType</param>
        ''' <param name="pTestRefRangesDS">Typed DataSet TestRefRangesDS with the Reference Ranges defined for the Test/SampleType</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 10/03/2010
        ''' Modified by: SG 17/06/2010 - Added saving of Reference Ranges by Test/SampleType
        '''              AG 13/01/2011 - Changes in code for saving the Reference Ranges by Test/SampleType
        '''              TR 12/09/2011 - Changes in updating of needed Reagents
        '''              TR 18/10/2011 - Move transaction management out of Try/Catch block (due to there are several Exit Try in code)
        '''              AG 08/10/2012 - add the flag parameter informing if the history test programming has to be saved or not
        ''' </remarks>
        Public Function SaveTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestDS As TestsDS, ByVal pTestSamplesDS As TestSamplesDS, _
                                 ByVal pTestReagentsVolumesDS As TestReagentsVolumesDS, ByVal pReagentsDS As ReagentsDS, _
                                 ByVal pTestReagentesDS As TestReagentsDS, ByVal pTestRefRangesDS As TestRefRangesDS, _
                                 ByVal pHistUpdateFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim prevTestID As Integer = 0
                        Dim prevReagenID As Integer = 0
                        Dim tempTestsDS As New TestsDS()
                        Dim tempReagentsDS As New ReagentsDS
                        Dim tempTestReagentsDS As New TestReagentsDS()
                        Dim newTestRefRangesDS As New TestRefRangesDS 'AG 13/01/2011
                        Dim updatedTestRefRangesDS As New TestRefRangesDS 'AG 13/01/2011
                        Dim deletedTestRefRangesDS As New TestRefRangesDS 'AG 13/01/2011
                        Dim myReagentsDelegate As New ReagentsDelegate()
                        Dim myTestSamplesDelegate As New TestSamplesDelegate()
                        Dim mytestReagentsDelegate As New TestReagentsDelegate()
                        Dim myTestReagentsVolumesDelegate As New TestReagentsVolumeDelegate()
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate() 'SG 17/06/2010
                        Dim qTestReagent As New List(Of TestReagentsDS.tparTestReagentsRow)
                        Dim qReagentsVolume As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                        Dim MyGlobalBase As New GlobalBase 'TR 30/03/202

                        For Each testRow As TestsDS.tparTestsRow In pTestDS.tparTests.Rows
                            Dim myTestID As Integer = testRow.TestID

                            tempTestsDS.tparTests.ImportRow(testRow)
                            prevTestID = testRow.TestID

                            If (testRow.NewTest) Then
                                'TR 05/02/2013 -Implemente the preloaded test to set the corresponding TestID
                                'Create the new Test
                                myGlobalDataTO = Create(dbConnection, pTestDS, testRow.PreloadedTest)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Create the list of needed Reagents
                                    Dim ReagentNum As Integer = 0
                                    For Each reagentsRow As ReagentsDS.tparReagentsRow In pReagentsDS.tparReagents.Rows
                                        ReagentNum += 1 'Used to control the Reagent Number

                                        tempReagentsDS.tparReagents.Clear()
                                        tempReagentsDS.tparReagents.ImportRow(reagentsRow)

                                        prevReagenID = reagentsRow.ReagentID
                                        myGlobalDataTO = myReagentsDelegate.Create(dbConnection, tempReagentsDS)
                                        If (Not myGlobalDataTO.HasError) Then
                                            'Verify if the created Reagent is linked to the Test
                                            Dim myReagentID As Integer = reagentsRow.ReagentID
                                            qTestReagent = (From a In pTestReagentesDS.tparTestReagents _
                                                           Where a.TestID = prevTestID _
                                                         AndAlso a.ReagentID = myReagentID _
                                                         AndAlso a.ReagentNumber = ReagentNum _
                                                          Select a).ToList()

                                            If (qTestReagent.Count > 0) Then
                                                For Each testReagentsRow As TestReagentsDS.tparTestReagentsRow In qTestReagent
                                                    testReagentsRow.TestID = testRow.TestID
                                                    testReagentsRow.ReagentID = tempReagentsDS.tparReagents(0).ReagentID
                                                    tempTestReagentsDS.tparTestReagents.ImportRow(testReagentsRow)

                                                    'Create the link betweent Test and the created Reagent
                                                    myGlobalDataTO = mytestReagentsDelegate.Create(dbConnection, tempTestReagentsDS)
                                                    If (Not myGlobalDataTO.HasError) Then
                                                        'Create data of the Test/SampleTypes and the Reagent volumes by each one of the linked Reagents 
                                                        myGlobalDataTO = CreateNewTestSample(dbConnection, pTestSamplesDS, pTestReagentsVolumesDS, prevTestID, _
                                                                                             testRow.TestID, prevReagenID, tempTestReagentsDS.tparTestReagents(0).ReagentID, _
                                                                                             ReagentNum)
                                                        If (Not myGlobalDataTO.HasError) Then
                                                            tempTestReagentsDS.tparTestReagents.Clear()
                                                            If (ReagentNum = 2) Then ReagentNum = 0
                                                        Else
                                                            'Error creating data of Test/SampleTypes
                                                            Exit Try
                                                        End If
                                                    Else
                                                        'Error creating links between Test and Reagents
                                                        Exit Try
                                                    End If
                                                Next
                                            End If
                                        Else
                                            'Error creating the needed Reagents
                                            Exit Try
                                        End If
                                    Next reagentsRow

                                    'Create the Reference Ranges by Test/SampleType
                                    newTestRefRangesDS.Clear()
                                    For Each testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In pTestRefRangesDS.tparTestRefRanges.Rows
                                        testRefRangesRow.TestID = testRow.TestID
                                        If (Not testRefRangesRow.IsDeleted) Then
                                            newTestRefRangesDS.tparTestRefRanges.ImportRow(testRefRangesRow)
                                        End If
                                    Next testRefRangesRow

                                    If (newTestRefRangesDS.tparTestRefRanges.Count > 0) Then
                                        myGlobalDataTO = myTestRefRangesDelegate.Create(dbConnection, newTestRefRangesDS)

                                        'Error creating the Reference Ranges by Test/SampleType
                                        If (myGlobalDataTO.HasError) Then Exit Try
                                    End If
                                Else
                                    'Error creating the new Test
                                    Exit For
                                End If
                            Else
                                'Update data of an existing Test
                                myGlobalDataTO = Update(dbConnection, tempTestsDS)

                                'AG 11/10/2012 - Move after update the testsamples table!!! (in this method but later)
                                'If Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode AndAlso pHistUpdateFlag Then
                                '    'Using myTestID I need to get the current test programming parameters inside a myHistTestSamplesDS - Method is empty
                                '    myGlobalDataTO = myTestSamplesDelegate.HIST_Update(dbConnection, myTestID, "")
                                'End If

                                'AG 08/10/2012 - Update the FormulaText when the TestID is part of a calculated test
                                If Not myGlobalDataTO.HasError Then
                                    Dim calcTests As New CalculatedTestsDelegate
                                    myGlobalDataTO = calcTests.UpdateFormulaText(dbConnection, "STD", myTestID)
                                End If
                                'AG 08/10/2012

                                If (Not myGlobalDataTO.HasError) Then
                                    prevReagenID = 0

                                    For Each reagentsRow As ReagentsDS.tparReagentsRow In pReagentsDS.tparReagents.Rows
                                        prevReagenID = reagentsRow.ReagentID
                                        tempReagentsDS.tparReagents.ImportRow(reagentsRow)

                                        'Validate if it is a new Reagent and if the Test is a preloaded one
                                        If (reagentsRow.IsNew AndAlso testRow.PreloadedTest) Then
                                            'Validate if the Reagent already exists
                                            myGlobalDataTO = myReagentsDelegate.GetReagentByReagentName(dbConnection, reagentsRow.ReagentName)
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                If (DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Count > 0) Then
                                                    tempReagentsDS.tparReagents.Clear()
                                                    tempReagentsDS.tparReagents.ImportRow(DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents(0))

                                                    'The Reagent exists, then IsNew is set to False
                                                    reagentsRow.IsNew = False
                                                End If
                                            Else
                                                'Error verifying if a Reagent exists
                                                Exit Try
                                            End If
                                        End If

                                        If (reagentsRow.IsNew) Then
                                            'Create the new Reagent
                                            myGlobalDataTO = myReagentsDelegate.Create(dbConnection, tempReagentsDS)
                                        Else
                                            'Update data of an existing Reagent
                                            myGlobalDataTO = myReagentsDelegate.Update(dbConnection, tempReagentsDS)
                                        End If

                                        If (Not myGlobalDataTO.HasError) Then
                                            qTestReagent = (From a In pTestReagentesDS.tparTestReagents _
                                                           Where a.TestID = myTestID _
                                                         AndAlso a.ReagentID = prevReagenID _
                                                          Select a).ToList

                                            For Each testReagentRow As TestReagentsDS.tparTestReagentsRow In qTestReagent
                                                testReagentRow.ReagentID = tempReagentsDS.tparReagents(0).ReagentID
                                                tempTestReagentsDS.tparTestReagents.ImportRow(testReagentRow)

                                                If (testReagentRow.IsNew) Then
                                                    'Create the link between the Test and the new Reagent
                                                    myGlobalDataTO = mytestReagentsDelegate.Create(dbConnection, tempTestReagentsDS)
                                                    'Else
                                                    'SA 28/10/2010 - The DAO function called by this delegate is bad written!!
                                                    'myGlobalDataTO = mytestReagentsDelegate.Update(dbConnection, tempTestReagentsDS)
                                                End If

                                                If (Not myGlobalDataTO.HasError) Then
                                                    'Update the needed Reagent Volumes 
                                                    qReagentsVolume = (From a In pTestReagentsVolumesDS.tparTestReagentsVolumes _
                                                                      Where a.TestID = myTestID _
                                                                    AndAlso a.ReagentID = prevReagenID).ToList()

                                                    If (qReagentsVolume.Count > 0) Then
                                                        'Change the reagent id for the new create reagent id.
                                                        qReagentsVolume.First().ReagentID = tempReagentsDS.tparReagents(0).ReagentID
                                                        qReagentsVolume.Last().ReagentID = tempReagentsDS.tparReagents(0).ReagentID
                                                    End If
                                                    tempReagentsDS.tparReagents.Clear()
                                                    tempTestReagentsDS.tparTestReagents.Clear()
                                                Else
                                                    'Error creating the link between Test and Reagent
                                                    Exit Try
                                                End If
                                            Next
                                        Else
                                            'Error creating/updating Reagent
                                            Exit Try
                                        End If
                                    Next

                                    'Update data of Test/Sample Types
                                    Dim tempTestSamplesDS As New TestSamplesDS
                                    For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In pTestSamplesDS.tparTestSamples.Rows
                                        'DL 11/10/2012. BEGIN
                                        If (String.Compare(testSampleRow.CalibratorType, "ALTERNATIV", False) = 0) Then
                                            Dim myTestSamples As List(Of TestSamplesDS.tparTestSamplesRow)
                                            myTestSamples = (From a In pTestSamplesDS.tparTestSamples _
                                                             Where String.Compare(a.CalibratorType, "FACTOR", False) = 0 _
                                                             AndAlso String.Compare(a.SampleType, testSampleRow.SampleTypeAlternative, False) = 0 _
                                                             Select a).ToList()

                                            If myTestSamples.Count > 0 Then
                                                testSampleRow.CalibrationFactor = myTestSamples(0).CalibrationFactor
                                            End If
                                        End If
                                        'DL 11/10/2012. END

                                        tempTestSamplesDS.tparTestSamples.ImportRow(testSampleRow)

                                        If (testSampleRow.IsNew) Then
                                            'Add a new SampleType for the Test
                                            myGlobalDataTO = myTestSamplesDelegate.Create(dbConnection, tempTestSamplesDS)
                                        Else
                                            'Update data of an existing SampleType for the Test
                                            myGlobalDataTO = myTestSamplesDelegate.Update(dbConnection, tempTestSamplesDS)

                                            'AG 11/10/2012
                                            If Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode AndAlso pHistUpdateFlag Then
                                                'Using myTestID I need to get the current test programming parameters inside a myHistTestSamplesDS - Method is empty
                                                myGlobalDataTO = myTestSamplesDelegate.HIST_Update(dbConnection, myTestID, testSampleRow.SampleType)
                                            End If

                                        End If

                                        If (myGlobalDataTO.HasError) Then
                                            'Error creating/updating a Test/SampleType
                                            Exit For
                                        Else
                                            If (String.Compare(testSampleRow.CalibratorType, "FACTOR", False) = 0) Then
                                                'Delete previous used Experimental Calibrator
                                                myGlobalDataTO = myTestSamplesDelegate.DeleteTestCalibratorDataByTestIDSampleType(dbConnection, testSampleRow.TestID, _
                                                                                                                                  testSampleRow.SampleType)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If
                                            tempTestSamplesDS.tparTestSamples.Clear()
                                        End If
                                    Next

                                    If (Not myGlobalDataTO.HasError) Then
                                        'Create/Update the needed Reagent Volumes 
                                        Dim TestReagVolDS As New TestReagentsVolumesDS

                                        pTestReagentsVolumesDS.AcceptChanges()
                                        For Each testReagVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In pTestReagentsVolumesDS.tparTestReagentsVolumes.Rows
                                            TestReagVolDS.tparTestReagentsVolumes.ImportRow(testReagVolRow)

                                            If (testReagVolRow.IsNew) Then
                                                myGlobalDataTO = myTestReagentsVolumesDelegate.Create(dbConnection, TestReagVolDS)
                                            Else
                                                myGlobalDataTO = myTestReagentsVolumesDelegate.Update(dbConnection, TestReagVolDS)
                                            End If

                                            If (myGlobalDataTO.HasError) Then
                                                Exit For
                                            Else
                                                TestReagVolDS = New TestReagentsVolumesDS
                                            End If
                                        Next

                                        If myGlobalDataTO.HasError Then
                                            Exit For
                                        End If
                                    Else
                                        Exit For
                                    End If

                                    'Update Reference Ranges (they can be NEW, UPDATED or DELETED)
                                    newTestRefRangesDS.Clear()
                                    updatedTestRefRangesDS.Clear()
                                    deletedTestRefRangesDS.Clear()

                                    If pTestRefRangesDS.tparTestRefRanges.Rows.Count > 0 Then
                                        'Go trhough each row to validate if a new row is inserted
                                        For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In pTestRefRangesDS.tparTestRefRanges.Rows
                                            If TestRefRangesRow.IsDeleted Then
                                                deletedTestRefRangesDS.tparTestRefRanges.ImportRow(TestRefRangesRow)
                                            ElseIf TestRefRangesRow.IsNew Then
                                                newTestRefRangesDS.tparTestRefRanges.ImportRow(TestRefRangesRow)
                                            Else 'Updated
                                                updatedTestRefRangesDS.tparTestRefRanges.ImportRow(TestRefRangesRow)
                                            End If
                                        Next

                                        'Deleted
                                        If deletedTestRefRangesDS.tparTestRefRanges.Count > 0 Then
                                            myGlobalDataTO = myTestRefRangesDelegate.Delete(dbConnection, deletedTestRefRangesDS)
                                            If myGlobalDataTO.HasError Then Exit Try
                                        End If

                                        'New
                                        If newTestRefRangesDS.tparTestRefRanges.Count > 0 Then
                                            myGlobalDataTO = myTestRefRangesDelegate.Create(dbConnection, newTestRefRangesDS)
                                            If myGlobalDataTO.HasError Then Exit Try
                                        End If

                                        'Updated
                                        If updatedTestRefRangesDS.tparTestRefRanges.Count > 0 Then
                                            myGlobalDataTO = myTestRefRangesDelegate.Update(dbConnection, updatedTestRefRangesDS)
                                            If myGlobalDataTO.HasError Then Exit Try
                                        End If

                                    Else
                                        If myGlobalDataTO.HasError Then
                                            Exit For
                                        Else
                                            newTestRefRangesDS.Clear()
                                            deletedTestRefRangesDS.Clear()
                                            updatedTestRefRangesDS.Clear()
                                        End If
                                    End If
                                Else
                                    'Error updating Test data
                                    Exit For
                                End If
                            End If

                            If Not myGlobalDataTO.HasError Then
                                tempTestsDS.tparTests.Clear()
                            Else
                                Exit For
                            End If
                        Next testRow
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.SaveTest", EventLogEntryType.Error, False)
            End Try

            If (Not myGlobalDataTO.HasError) Then
                'When the Database Connection was opened locally, then the Commit is executed
                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
            Else
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            End If
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Function that prepare all the element to be update, create or delete on the
        ''' Database.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestDS"></param>
        ''' <param name="pTestSamplesDS"></param>
        ''' <param name="pTestReagentsVolumesDS"></param>
        ''' <param name="pReagentsDS"></param>
        ''' <param name="pTestReagentesDS"></param>
        ''' <param name="pDelTestReagentsVolumeList"></param>
        ''' <param name="pDeletedTestProgramingList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: AG 22/03/2010 - Delete curve results when needed
        '''              TR 14/06/2010 - Add calibrators parammeters (pTestReagentesDS, pTestCalibratorDS, pTestCalibratorValuesDS)
        '''              SG 17/06/2010 - Add test reference ranges (pTestRefRangesDS)
        '''              TR            - Add parameters pTestSamplesMultiRulesDS, pTestControlsDS, and pUpdateSampleType (optional)
        '''              SA 27/08/2014 - Inform field TestLongName when preparing the HistoryTestSamplesDS that is passed to function UpdateByQCTestIDAndSampleTypeNEW
        '''                              in HistoryTestSamplesDelegate (added as part of BT #1865, in which field TestLongName is also exported to QC Module for ISE Tests; 
        '''                              this change is to export the field also for Standard Tests)  
        ''' </remarks>
        Public Function PrepareTestToSave(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pAnalyzerID As String, _
                                          ByVal pWorkSessionID As String, _
                                          ByVal pTestDS As TestsDS, _
                                          ByVal pTestSamplesDS As TestSamplesDS, _
                                          ByVal pTestReagentsVolumesDS As TestReagentsVolumesDS, _
                                          ByVal pReagentsDS As ReagentsDS, _
                                          ByVal pTestReagentesDS As TestReagentsDS, _
                                          ByVal pCalibratorDS As CalibratorsDS, _
                                          ByVal pTestCalibratorDS As TestCalibratorsDS, _
                                          ByVal pTestCalibratorValuesDS As TestCalibratorValuesDS, _
                                          ByVal pTestRefRangesDS As TestRefRangesDS, _
                                          ByVal pDeleteCalibratorList As List(Of DeletedCalibratorTO), _
                                          ByVal pDelTestReagentsVolumeList As List(Of DeletedTestReagentsVolumeTO), _
                                          ByVal pDeletedTestProgramingList As List(Of DeletedTestProgramingTO), _
                                          ByVal pTestSamplesMultiRulesDS As TestSamplesMultirulesDS, _
                                          ByVal pTestsControlsDS As TestControlsDS, _
                                          ByVal pDeleteControlTOList As List(Of DeletedControlTO), _
                                          Optional ByVal pUpdateSampleType As String = "") As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim tempTestsDS As New TestsDS()
                        Dim tempReagentsDS As New ReagentsDS
                        Dim tempTestReagentsDS As New TestReagentsDS()
                        Dim tempTestSampleDS As New TestSamplesDS
                        Dim tempTestReagentVolDS As New TestReagentsVolumesDS
                        Dim tempTestCalibratorDS As New TestCalibratorsDS
                        Dim tempCalibratorDS As New CalibratorsDS()
                        Dim tempTestRefRangesDS As New TestRefRangesDS 'SG 17/06/2010

                        Dim myTestSamplesDelegate As New TestSamplesDelegate()
                        Dim mytestReagentsDelegate As New TestReagentsDelegate()
                        Dim myReagentsDelegate As New ReagentsDelegate()
                        Dim myTestReagentsVolumesDelegate As New TestReagentsVolumeDelegate()
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate() 'SG 07/09/2010
                        Dim myTestSamplesMultiRulesDelegate As New TestSamplesMultirulesDelegate 'TR 06/04/2011
                        Dim myTestControlsDelegate As New TestControlsDelegate 'TR 08/04/2011

                        Dim qtestList As New List(Of TestsDS.tparTestsRow)
                        Dim qtestSample As New List(Of TestSamplesDS.tparTestSamplesRow)
                        Dim qReagents As New List(Of ReagentsDS.tparReagentsRow)
                        Dim qTestReagent As New List(Of TestReagentsDS.tparTestReagentsRow)
                        Dim qTestReagentsVolume As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                        Dim qCalibratorList As New List(Of CalibratorsDS.tparCalibratorsRow)
                        Dim qTestCalibratorList As New List(Of TestCalibratorsDS.tparTestCalibratorsRow)
                        Dim qTestRefRangesList As New List(Of TestRefRangesDS.tparTestRefRangesRow) 'SG 17/06/2010
                        Dim myTestID As Integer = 0
                        Dim mySampleType As String = "" 'TR 29/03/2011 add to use on filter

                        'TR 16/05/2011
                        Dim myQCTestSampleID As Integer = 0
                        Dim myHistoryControlLotDelegate As New HistoryControlLotsDelegate
                        Dim myHistoryControlLotDS As New HistoryControlLotsDS

                        Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
                        Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate

                        Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
                        Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
                        Dim myHistoryTestSamplesRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow

                        Dim myHistoryTestSamplesRulesDelegate As New HistoryTestSamplesRulesDelegate
                        'TR 16/05/2011 -END.

                        For Each testRow As TestsDS.tparTestsRow In pTestDS.tparTests.Rows
                            myTestID = testRow.TestID

                            'Search the test --TEST
                            qtestList = (From a In pTestDS.tparTests _
                                         Where a.TestID = myTestID _
                                         Select a).ToList()

                            'if found then add to the temporal TestDS
                            For Each tempTestRow As TestsDS.tparTestsRow In qtestList
                                tempTestsDS.tparTests.ImportRow(tempTestRow)
                            Next

                            'TEST SAMPLE
                            qtestSample = (From a In pTestSamplesDS.tparTestSamples _
                                           Where a.TestID = myTestID _
                                           Select a).ToList()

                            'SG 07/09/2010
                            Dim myDBSavedTestSample As New TestSamplesDS
                            myGlobalDataTO = myTestSamplesDelegate.GetSampleDataByTestID(pDBConnection, myTestID)
                            If Not myGlobalDataTO.HasError Then
                                myDBSavedTestSample = CType(myGlobalDataTO.SetDatos, Types.TestSamplesDS)
                                For Each savedTestSampleRow As TestSamplesDS.tparTestSamplesRow In myDBSavedTestSample.tparTestSamples.Rows
                                    Dim NotToDelete As Boolean = True
                                    For Each tempTestSampleRow As TestSamplesDS.tparTestSamplesRow In qtestSample
                                        NotToDelete = (savedTestSampleRow.SampleType = tempTestSampleRow.SampleType)
                                        If NotToDelete Then
                                            Exit For
                                        End If
                                    Next
                                    'Delete the testsample
                                    If Not NotToDelete Then
                                        myGlobalDataTO = myTestSamplesDelegate.DeleteCascadeByTestIDSampleType(pDBConnection, myTestID, savedTestSampleRow.SampleType, pAnalyzerID, pWorkSessionID)
                                        If myGlobalDataTO.HasError Then
                                            Exit For
                                        End If
                                    End If
                                Next

                            Else
                                Exit For
                            End If
                            'END 07/09/2010

                            For Each tempTestSampleRow As TestSamplesDS.tparTestSamplesRow In qtestSample
                                tempTestSampleDS.tparTestSamples.ImportRow(tempTestSampleRow)
                            Next

                            'TEST REAGENT
                            qTestReagent = (From a In pTestReagentesDS.tparTestReagents _
                                            Where a.TestID = myTestID _
                                            Select a).ToList()

                            Dim qValidateReagent As New List(Of ReagentsDS.tparReagentsRow)

                            For Each tempTestReagentRow As TestReagentsDS.tparTestReagentsRow In qTestReagent
                                Dim myReagentID As Integer = tempTestReagentRow.ReagentID

                                tempTestReagentsDS.tparTestReagents.ImportRow(tempTestReagentRow)

                                'REAGENT
                                qReagents = (From a In pReagentsDS.tparReagents _
                                             Where a.ReagentID = myReagentID _
                                             Select a).ToList()

                                For Each tempReagentRow As ReagentsDS.tparReagentsRow In qReagents
                                    Dim myReagentName As String = tempReagentRow.ReagentName

                                    qValidateReagent = (From a In tempReagentsDS.tparReagents _
                                                        Where a.ReagentName = myReagentName _
                                                        Select a).ToList()

                                    If qValidateReagent.Count = 0 Then
                                        tempReagentsDS.tparReagents.ImportRow(tempReagentRow)
                                    End If
                                Next tempReagentRow
                            Next tempTestReagentRow

                            'TEST REAGENT VOLUME
                            qTestReagentsVolume = (From a In pTestReagentsVolumesDS.tparTestReagentsVolumes _
                                                   Where a.TestID = myTestID _
                                                   Select a).ToList()

                            For Each tempTestReagentVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In qTestReagentsVolume
                                tempTestReagentVolDS.tparTestReagentsVolumes.ImportRow(tempTestReagentVolRow)
                            Next tempTestReagentVolRow

                            'SG 17/06/2010
                            'TEST REF RANGES
                            qTestRefRangesList = (From a In pTestRefRangesDS.tparTestRefRanges _
                                             Where a.TestID = myTestID _
                                             Select a).ToList()

                            For Each tempTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In qTestRefRangesList
                                tempTestRefRangesDS.tparTestRefRanges.ImportRow(tempTestRefRangesRow)
                            Next
                            'END SG 17/06/2010

                            'AG 08/10/2012 - Search if close historic test version is required or not. It not we have to update the history test programing
                            '                calling the HIST_Update method
                            Dim histUpdateFlag As Boolean = True
                            For Each resultsToDeleteRow As DeletedTestProgramingTO In pDeletedTestProgramingList
                                If resultsToDeleteRow.TestID = myTestID Then
                                    histUpdateFlag = False
                                    Exit For
                                End If
                            Next
                            'AG 08/10/2012

                            myGlobalDataTO = SaveTest(dbConnection, tempTestsDS, tempTestSampleDS, tempTestReagentVolDS, _
                                                      tempReagentsDS, tempTestReagentsDS, tempTestRefRangesDS, histUpdateFlag)

                            'TR 14/06/2010 CALIBRATOR AREA
                            If Not myGlobalDataTO.HasError Then
                                'TR 29/03/2011 set value to sample type and filter
                                'mySampleType = tempTestSampleDS.tparTestSamples(1).SampleType
                                mySampleType = pUpdateSampleType
                                'After saving the test then search the test id in case is new remove the temporar testid
                                ' and change by the new test id.
                                'TR 29/03/2011 -Add filter by sample type.
                                'qTestCalibratorList = (From a In pTestCalibratorDS.tparTestCalibrators _
                                '                       Where a.TestID = myTestID And a.SampleType = mySampleType Select a).ToList()
                                'TR 21/07/2011 -Filter by the TestID 
                                qTestCalibratorList = (From a In pTestCalibratorDS.tparTestCalibrators _
                                                       Where a.TestID = myTestID Select a).ToList()

                                If qTestCalibratorList.Count > 0 Then
                                    'TR 17/11/2010 -Change the testID for the new test id assigned for each testcalibraor on the list.
                                    For Each testCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In qTestCalibratorList
                                        testCalibRow.BeginEdit()
                                        testCalibRow.TestID = tempTestsDS.tparTests(0).TestID
                                        testCalibRow.EndEdit()
                                    Next
                                    'TR 17/11/2010 -END
                                End If
                                pTestCalibratorDS.AcceptChanges()

                            End If
                            'TR 14/06/2010 -END

                            'TR 06/04/2011 QC AREA
                            If (Not myGlobalDataTO.HasError) Then
                                'Go through each Multirule and set the TestID (in case it is a new Test)
                                For Each testSampleRule As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow In pTestSamplesMultiRulesDS.tparTestSamplesMultirules.Rows
                                    testSampleRule.BeginEdit()
                                    testSampleRule.TestID = tempTestsDS.tparTests(0).TestID
                                    testSampleRule.EndEdit()
                                Next
                                pTestSamplesMultiRulesDS.AcceptChanges()

                                'Save the group of Multirules 
                                'TR 14/06/2012 -Implement new function.
                                myGlobalDataTO = myTestSamplesMultiRulesDelegate.SaveMultiRulesNEW(dbConnection, pTestSamplesMultiRulesDS)

                                '25/05/2011 - Before saving Test Controls, validate if there are any delete Control
                                If (Not pDeleteControlTOList Is Nothing AndAlso pDeleteControlTOList.Count > 0) Then
                                    'If there are open QC Results, cumulate them before unlink the Control from the Test/SampleType
                                    For Each myDeleteControlTO As DeletedControlTO In pDeleteControlTOList
                                        myGlobalDataTO = CalculateQCCumulate(dbConnection, myDeleteControlTO.TestID, myDeleteControlTO.SampleType)
                                    Next
                                End If
                                '25/05/2011 -END.

                                'Save all linked Controls
                                If (Not myGlobalDataTO.HasError) Then
                                    'Go through each TestControl and set the TestID (in case it is a new Test)
                                    For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestsControlsDS.tparTestControls.Rows
                                        If (Not testControlRow.IsControlIDNull) Then
                                            testControlRow.BeginEdit()
                                            testControlRow.TestID = tempTestsDS.tparTests(0).TestID
                                            testControlRow.EndEdit()
                                        End If
                                    Next
                                    pTestsControlsDS.AcceptChanges()

                                    If (pTestsControlsDS.tparTestControls.Count > 0) Then
                                        'Call the delegate to save TestControls
                                        myGlobalDataTO = myTestControlsDelegate.SaveTestControlsNEW(dbConnection, pTestsControlsDS, Nothing, False)
                                    Else
                                        'If there are not linked Controls, then delete all the existing ones in the DataSet
                                        myGlobalDataTO = myTestControlsDelegate.DeleteTestControlsByTestIDAndTestTypeNEW(dbConnection, tempTestsDS.tparTests(0).TestID, _
                                                                                                                         "STD", pUpdateSampleType)
                                    End If
                                Else
                                    Exit For
                                End If
                            Else
                                Exit For
                            End If
                            'TR 06/04/2011 QC Area END

                            'TR 16/05/2011 -History QC
                            'After updating QC data for the Test, then update the corresponding data on the QC Module 
                            If (Not myGlobalDataTO.HasError) Then
                                myHistoryTestSamplesRow = myHistoryTestSamplesDS.tqcHistoryTestSamples.NewtqcHistoryTestSamplesRow()
                                myHistoryTestSamplesRow.TestType = "STD"
                                myHistoryTestSamplesRow.TestID = tempTestsDS.tparTests(0).TestID
                                myHistoryTestSamplesRow.TestName = testRow.TestName
                                myHistoryTestSamplesRow.TestShortName = testRow.ShortName
                                myHistoryTestSamplesRow.PreloadedTest = testRow.PreloadedTest
                                myHistoryTestSamplesRow.MeasureUnit = testRow.MeasureUnit
                                myHistoryTestSamplesRow.DecimalsAllowed = testRow.DecimalsAllowed
                                myHistoryTestSamplesDS.tqcHistoryTestSamples.AddtqcHistoryTestSamplesRow(myHistoryTestSamplesRow)

                                'UPDATE tqcHistoryTestSamples.
                                myGlobalDataTO = myHistoryTestSamplesDelegate.UpdateByTestIDNEW(dbConnection, myHistoryTestSamplesDS)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'UPDATE History QC Test/Sample data
                                For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In pTestSamplesDS.tparTestSamples.Rows
                                    myQCTestSampleID = 0
                                    'TR 14/06/2012 -Added the TestType new function
                                    myGlobalDataTO = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeNEW(dbConnection, "STD", tempTestsDS.tparTests(0).TestID, _
                                                                                                               TestSampleRow.SampleType)
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    myHistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                    If (myHistoryTestSamplesDS.tqcHistoryTestSamples.Count > 0) Then myQCTestSampleID = myHistoryTestSamplesDS.tqcHistoryTestSamples(0).QCTestSampleID

                                    If (Not myQCTestSampleID = 0) Then
                                        'Update fields of QC Calculation Criteria
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria = TestSampleRow.RejectionCriteria
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode = TestSampleRow.CalculationMode
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries = TestSampleRow.NumberOfSeries

                                        'Update field TestLongName (Report Name) 
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestLongName = TestSampleRow.TestLongName

                                        myGlobalDataTO = myHistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleTypeNEW(dbConnection, myHistoryTestSamplesDS)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'UPDATE THE MULTIRULES 
                                        '1- Delete existing Multirules
                                        myGlobalDataTO = myHistoryTestSamplesRulesDelegate.Delete(dbConnection, myQCTestSampleID)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        '2-Insert rules
                                        myGlobalDataTO = myHistoryTestSamplesRulesDelegate.InsertFromTestSampleMultiRulesNEW(dbConnection, "STD", tempTestsDS.tparTests(0).TestID, _
                                                                                                                             TestSampleRow.SampleType, myQCTestSampleID)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    End If

                                    'UPDATE THE HistoryTestControlLot
                                    For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestsControlsDS.tparTestControls.Rows
                                        myGlobalDataTO = myHistoryControlLotDelegate.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, testControlRow.ControlID, _
                                                                                                                              testControlRow.LotNumber)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        myHistoryControlLotDS = DirectCast(myGlobalDataTO.SetDatos, HistoryControlLotsDS)
                                        For Each histControlRow As HistoryControlLotsDS.tqcHistoryControlLotsRow In myHistoryControlLotDS.tqcHistoryControlLots.Rows
                                            myGlobalDataTO = myHistoryTestControlLotsDelegate.Update(dbConnection, myQCTestSampleID, histControlRow.QCControlLotID, _
                                                                                                     testControlRow.MinConcentration, testControlRow.MaxConcentration)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        Next
                                    Next
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next 'Test Sample
                            End If
                            'TR 16/05/2011 -END 

                            If (myGlobalDataTO.HasError) Then Exit For

                            tempTestsDS.tparTests.Clear()
                            tempTestSampleDS.tparTestSamples.Clear()
                            tempTestReagentVolDS.tparTestReagentsVolumes.Clear()
                            tempReagentsDS.tparReagents.Clear()
                            tempTestReagentsDS.tparTestReagents.Clear()
                            tempTestCalibratorDS.tparTestCalibrators.Clear()
                            tempCalibratorDS.tparCalibrators.Clear() 'TR 02/08/2010
                            tempTestRefRangesDS.tparTestRefRanges.Clear() 'SG 02/07/2010
                        Next 'TestDS

                        'TR 21/07/2011 -Save calibrator
                        If Not myGlobalDataTO.HasError Then
                            Dim myCalibratorDelegate As New CalibratorsDelegate

                            'If qTestCalibratorList.Count > 0 Then
                            '    tempTestCalibratorDS.tparTestCalibrators.ImportRow(qTestCalibratorList.First())
                            'End If
                            'Use the temptestcalibrator instead of ptestcalibrator
                            myGlobalDataTO = myCalibratorDelegate.Save(dbConnection, pCalibratorDS, pTestCalibratorDS, _
                                                                            pTestCalibratorValuesDS, pDeleteCalibratorList, True, , , pAnalyzerID, pWorkSessionID)

                        End If
                        'TR 21/07/2011 -END.

                        Dim myResultsDAO As New twksResultsDAO

                        'AG 22/03/2010
                        Dim myCurveDAO As New twksCurveResultsDAO   'NOTE: No se accede al delegado para evitar referencias circulares!!! Por eso se accede directamente al DAO
                        Dim deleteBlankCalibResults As Boolean = False
                        Dim deleteOnlyCalibResults As Boolean = False
                        'END AG 22/03/2010

                        'Delete element if there are some. 
                        If Not myGlobalDataTO.HasError Then
                            'delete Test Reagent and Test Reagent Volume
                            If Not pDelTestReagentsVolumeList Is Nothing AndAlso pDelTestReagentsVolumeList.Count > 0 Then
                                myGlobalDataTO = myReagentsDelegate.DeleteReagentCascade(dbConnection, pAnalyzerID, _
                                                                                            pDelTestReagentsVolumeList)
                            End If

                            If Not myGlobalDataTO.HasError Then
                                'Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
                                'AG TR 19/03/2010
                                'Delete Test if there are some
                                For Each DelTestSampRow As DeletedTestProgramingTO In pDeletedTestProgramingList
                                    'AG 22/03/2010
                                    deleteBlankCalibResults = DelTestSampRow.DeleteBlankCalibResults
                                    deleteOnlyCalibResults = DelTestSampRow.DeleteOnlyCalibrationResult
                                    'END AG 22/03/2010

                                    If DelTestSampRow.SampleType = "" AndAlso _
                                       Not DelTestSampRow.DeleteBlankCalibResults Then

                                        myGlobalDataTO = DeleteTestCascade(dbConnection, _
                                                                           pAnalyzerID, _
                                                                           DelTestSampRow.TestID, pWorkSessionID) 'TR 18/05/2010set the value to the global to

                                        If Not myGlobalDataTO.HasError Then
                                            deleteBlankCalibResults = True   'AG 22/03/2010

                                            'TR 25/05/2011 -Before deleting the control validate if there're QC cumulate result 
                                            myGlobalDataTO = CalculateQCCumulate(dbConnection, DelTestSampRow.TestID, "")

                                            If myGlobalDataTO.HasError Then Exit For
                                            'TR 25/05/2011 -END.

                                            'TR 11/04/2011 -Delete controls by Test ID
                                            myGlobalDataTO = myTestControlsDelegate.DeleteTestControlsByTestIDNEW(dbConnection, "STD", DelTestSampRow.TestID, pAnalyzerID, pWorkSessionID)
                                            'TR 25/01/2012 -Delete TestSampleMultirules
                                            If Not myGlobalDataTO.HasError Then
                                                Dim myTestSampleMultirulesDelegate As New TestSamplesMultirulesDelegate
                                                myGlobalDataTO = myTestSampleMultirulesDelegate.DeleMultiRulesByTestIDNEW(dbConnection, "STD", DelTestSampRow.TestID)
                                            End If

                                            'TR 11/05/2011 -Mark Test as delete on the history tables.
                                            If Not myGlobalDataTO.HasError Then
                                                myGlobalDataTO = myHistoryTestSamplesDelegate.MarkTestValuesAsDeleteNEW(dbConnection, "STD", DelTestSampRow.TestID)
                                            End If
                                            'TR 11/05/2011 -END.
                                        End If


                                    ElseIf String.Compare(DelTestSampRow.SampleType, "", False) <> 0 AndAlso Not DelTestSampRow.DeleteOnlyCalibrationResult Then

                                        'TR 25/05/2011 -Before deleting the control validate if there're QC cumulate result 
                                        myGlobalDataTO = CalculateQCCumulate(dbConnection, DelTestSampRow.TestID, DelTestSampRow.SampleType)
                                        If myGlobalDataTO.HasError Then Exit For
                                        'TR 25/05/2011 -END.

                                        myGlobalDataTO = myTestSamplesDelegate.DeleteCascadeByTestIDSampleType(dbConnection, _
                                                                                DelTestSampRow.TestID, DelTestSampRow.SampleType, pAnalyzerID, pWorkSessionID)
                                        If Not myGlobalDataTO.HasError Then
                                            deleteOnlyCalibResults = True   'AG 22/03/2010

                                            'TR 11/04/2011 -Delete controls by TestID and Sample Type 
                                            myGlobalDataTO = myTestControlsDelegate.DeleteTestControlsByTestIDNEW(dbConnection, "STD", DelTestSampRow.TestID, _
                                                                                                                  pAnalyzerID, pWorkSessionID, DelTestSampRow.SampleType)

                                            'TR 25/01/2012 -Delete TestSampleMultirules
                                            If Not myGlobalDataTO.HasError Then
                                                Dim myTestSampleMultirulesDelegate As New TestSamplesMultirulesDelegate
                                                myGlobalDataTO = myTestSampleMultirulesDelegate.DeleMultiRulesByTestIDNEW(dbConnection, "STD", DelTestSampRow.TestID, _
                                                                                                                          DelTestSampRow.SampleType)
                                            End If
                                            'TR 25/01/2012 -END

                                            'TR 11/05/2011 -Mark Test Sample as delete on the history tables.
                                            If Not myGlobalDataTO.HasError Then
                                                myGlobalDataTO = myHistoryTestSamplesDelegate.MarkSampleTypeValuesAsDeleteNEW(dbConnection, "STD", DelTestSampRow.TestID, _
                                                                                                                              DelTestSampRow.SampleType)
                                            End If
                                            'TR 11/05/2011 -END.
                                        End If
                                    End If
                                    'TR 18/05/2010 -Validate if there was an error
                                    If Not myGlobalDataTO.HasError Then
                                        'AG 22/03/2010
                                        'If DelTestSampRow.DeleteBlankCalibResults Then
                                        If deleteBlankCalibResults Then
                                            'AG 22/03/2010: Delete OLD calibrator curve results when necessary
                                            DelTestSampRow.CurveResultsID = Me.GetCurveResultID(dbConnection, _
                                                                    DelTestSampRow.TestID, DelTestSampRow.TestVersion, mySampleType) 'TR 29/07/2013 Set The sampleType to load
                                            If DelTestSampRow.CurveResultsID <> -1 Then
                                                myGlobalDataTO = myCurveDAO.DeleteCurve(dbConnection, _
                                                                                        DelTestSampRow.CurveResultsID)
                                            End If
                                            'END AG 22/03/2010

                                            'Delete all the blank and calib resuls.                                       
                                            myGlobalDataTO = myResultsDAO.DeleteResultsByTestID(dbConnection, _
                                                                    DelTestSampRow.TestID, DelTestSampRow.TestVersion)

                                            'AG 08/10/2012 - close all history blank & calib results
                                            If Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode Then
                                                myGlobalDataTO = myTestSamplesDelegate.HIST_CloseTestVersion(dbConnection, DelTestSampRow.TestID, "BLANK")
                                            End If


                                        ElseIf deleteOnlyCalibResults Then
                                            'AG 22/03/2010: Delete OLD calibrator curve results when necessary
                                            DelTestSampRow.CurveResultsID = Me.GetCurveResultID(dbConnection, _
                                                                                                DelTestSampRow.TestID, _
                                                                                                DelTestSampRow.TestVersion, _
                                                                                                DelTestSampRow.SampleType)
                                            If DelTestSampRow.CurveResultsID <> -1 Then
                                                myGlobalDataTO = myCurveDAO.DeleteCurve(dbConnection, _
                                                                                        DelTestSampRow.CurveResultsID)
                                            End If
                                            'END AG 22/03/2010

                                            'Delete only the sample type and calib. result. REMOVE COMMENT MARK WHEN IMPLEMETED.
                                            myGlobalDataTO = myResultsDAO.DeleteCalibrationResultsByTestIDSampleType(dbConnection, _
                                                                                                                     DelTestSampRow.TestID, _
                                                                                                                     DelTestSampRow.TestVersion, _
                                                                                                                     DelTestSampRow.SampleType)

                                            'AG 08/10/2012 - close all history blank & calib results
                                            'NOTE: the following call is redundant because the HIST_CloseTestVersion for "CALIB" has been also executed
                                            'when previously the myCalibratorDelegate.Save has been called 
                                            If Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode Then
                                                myGlobalDataTO = myTestSamplesDelegate.HIST_CloseTestVersion(dbConnection, DelTestSampRow.TestID, "CALIB", DelTestSampRow.SampleType)
                                            End If

                                        End If

                                        If myGlobalDataTO.HasError Then
                                            Exit For
                                        End If
                                        'END AG TR 19/03/2010

                                    Else
                                        Exit For
                                    End If 'TR 18/05/2010 END                                   
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
                If (pDBConnection Is Nothing) And _
                   (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.PrepareTestToSave", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Create data for new Sample Types for an Standard Test
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesDS"></param>
        ''' <param name="pTestReagentsVolumesDS"></param>
        ''' <param name="pPrevTestID"></param>
        ''' <param name="pNewTestId"></param>
        ''' <param name="pPrevReagentID"></param>
        ''' <param name="pnewReagentID"></param>
        ''' <param name="pReagentsNumber"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 16/03/2010
        ''' </remarks>
        Private Function CreateNewTestSample(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pTestSamplesDS As TestSamplesDS, _
                                             ByVal pTestReagentsVolumesDS As TestReagentsVolumesDS, ByVal pPrevTestID As Integer, ByVal pNewTestId As Integer, _
                                             ByVal pPrevReagentID As Integer, ByVal pnewReagentID As Integer, ByVal pReagentsNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestSamplesDelegate As New TestSamplesDelegate()
                Dim myTestReagentsVolumesDelegate As New TestReagentsVolumeDelegate()
                Dim qtestSample As New List(Of TestSamplesDS.tparTestSamplesRow)
                Dim qReagentsVolume As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)

                'CREATE NEW TestSample
                If (pTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                    'Get all new SampleTypes for the specified Test
                    qtestSample = (From a In pTestSamplesDS.tparTestSamples _
                                  Where a.TestID = pPrevTestID Select a).ToList()

                    If (qtestSample.Count > 0) Then
                        'Inform the new TestID for all SampleTypes and create them
                        For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In qtestSample
                            testSampleRow.TestID = pNewTestId
                        Next
                        If qtestSample.First().IsNew Then 'TR 20/02/2013 only if is new 
                            myGlobalDataTO = myTestSamplesDelegate.Create(pDbConnection, pTestSamplesDS)
                        End If
                    End If

                    If Not myGlobalDataTO.HasError Then
                        'CREATE NEW TEST REAGENTS VOLUME.
                        If pTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count > 0 Then
                            qReagentsVolume = (From a In pTestReagentsVolumesDS.tparTestReagentsVolumes _
                                              Where a.TestID = pPrevTestID And a.ReagentID = pPrevReagentID _
                                              And a.ReagentNumber = pReagentsNumber).ToList()
                            'set the test id
                            Dim tempReagentsVol As New TestReagentsVolumesDS
                            For Each testReagVol As TestReagentsVolumesDS.tparTestReagentsVolumesRow In qReagentsVolume
                                tempReagentsVol.tparTestReagentsVolumes.Clear()
                                testReagVol.TestID = pNewTestId
                                testReagVol.ReagentID = pnewReagentID 'tempTestReagentsDS.tparTestReagents(0).ReagentID
                                tempReagentsVol.tparTestReagentsVolumes.ImportRow(testReagVol)
                                myGlobalDataTO = myTestReagentsVolumesDelegate.Create(pDbConnection, tempReagentsVol)
                            Next
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.CreateNewTestSample", EventLogEntryType.Error, False)

            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Method incharge to delete a testID 
        ''' this method go throught all the tables in the DB 
        ''' where the test need to be deleted.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">TestID</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteTestCascade(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pAnalyzerID As String, _
                                           ByVal pTestID As Integer, _
                                           ByVal pWorksessionID As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        '<Function Logic>
                        Dim mytparTestsDAO As New tparTestsDAO()
                        Dim myTestReagentsDelegate As New TestReagentsDelegate()
                        Dim myReagentsDelegate As New ReagentsDelegate()
                        Dim myTestSampleDS As New TestSamplesDS()
                        Dim myTestReagensDS As New TestReagentsDS()
                        Dim myTestSampleDelegate As New TestSamplesDelegate()
                        Dim myTestReagentsVolume As New TestReagentsVolumeDelegate()
                        Dim myTestCalibratorDelegate As New TestCalibratorsDelegate()
                        Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate()
                        Dim myTestControlDelegate As New TestControlsDelegate()
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

                        'TR 17/04/2010 -Include the new tables to delete. 

                        'TESTCONTROL.
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myTestControlDelegate.DeleteTestControlsByTestIDNEW(dbConnection, "STD", pTestID, pAnalyzerID, pWorksessionID)
                        End If

                        'CALIBRATORS
                        If Not myGlobalDataTO.HasError Then
                            Dim myTestCalibratorDS As New TestCalibratorsDS
                            'Get the calibrators value 
                            myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(dbConnection, _
                                                                                                pTestID)

                            If Not myGlobalDataTO.HasError Then
                                myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                                'go throught each test calibrator to delete all values.
                                For Each testCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In myTestCalibratorDS.tparTestCalibrators.Rows
                                    'Delete calibrator value.
                                    myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, _
                                                                                                             testCalibRow.TestCalibratorID)

                                    If myGlobalDataTO.HasError Then
                                        Exit For
                                    End If
                                Next

                                If Not myGlobalDataTO.HasError Then
                                    'Delete the TestCalibrator
                                    myGlobalDataTO = myTestCalibratorDelegate.DeleteTestCaliByTestID(dbConnection, _
                                                                                                     pTestID)
                                End If
                            End If
                        End If

                        If Not myGlobalDataTO.HasError Then
                            'Get all the sample data by the TestID 
                            myGlobalDataTO = myTestSampleDelegate.GetSampleDataByTestID(dbConnection, _
                                                                                        pTestID)

                            If Not myGlobalDataTO.HasError Then
                                myTestSampleDS = CType(myGlobalDataTO.SetDatos, TestSamplesDS)

                                If myTestSampleDS.tparTestSamples.Rows.Count > 0 Then
                                    'DELETE Test Samples.
                                    For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In myTestSampleDS.tparTestSamples.Rows
                                        myGlobalDataTO = myTestSampleDelegate.DeleteCascadeByTestIDSampleType(dbConnection, _
                                                                                                              testSampleRow.TestID, _
                                                                                                              testSampleRow.SampleType, pAnalyzerID, pWorksessionID)
                                    Next
                                End If

                                'Delete Test Reagent
                                If Not myGlobalDataTO.HasError Then
                                    'get all the reagents Test
                                    myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(dbConnection, _
                                                                                            pTestID)

                                    If Not myGlobalDataTO.HasError Then
                                        myTestReagensDS = CType(myGlobalDataTO.SetDatos, TestReagentsDS)

                                        Dim myDeletedReagentsTO As New DeletedTestReagentsVolumeTO()
                                        Dim myDelReagentList As New List(Of DeletedTestReagentsVolumeTO)

                                        For Each testReagentRow As TestReagentsDS.tparTestReagentsRow In myTestReagensDS.tparTestReagents.Rows
                                            myDeletedReagentsTO.ReagentID = testReagentRow.ReagentID
                                            myDeletedReagentsTO.TestID = testReagentRow.TestID
                                            myDeletedReagentsTO.ReagentNumber = testReagentRow.ReagentNumber

                                            myDelReagentList.Add(myDeletedReagentsTO)

                                            'Delete Reagent and all related tables
                                            myGlobalDataTO = myReagentsDelegate.DeleteReagentCascade(dbConnection, _
                                                                                                     pAnalyzerID, _
                                                                                                     myDelReagentList)

                                            If Not myGlobalDataTO.HasError Then
                                                myDelReagentList.Clear()
                                                myDeletedReagentsTO = New DeletedTestReagentsVolumeTO
                                            Else
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If

                                'TESTREFRANGES.
                                If Not myGlobalDataTO.HasError Then
                                    myGlobalDataTO = myTestRefRangesDelegate.DeleteByTestID(dbConnection, pTestID)
                                End If

                                'Disable Calculated test.
                                If Not myGlobalDataTO.HasError Then
                                    Dim myCalculatedTestDelegate As New CalculatedTestsDelegate
                                    'TR 25/11/2010
                                    myGlobalDataTO = myCalculatedTestDelegate.DeleteCalculatedTestbyTestID(dbConnection, pTestID)
                                End If

                                'SEARCH THE TestProfiles and TestProfilesTest.
                                If Not myGlobalDataTO.HasError Then
                                    Dim myTestProfilesDelegate As New TestProfilesDelegate
                                    'TR 25/03/2011 -Fixed Bug: Set the correct connection control.
                                    myGlobalDataTO = myTestProfilesDelegate.DeleteByTestIDSampleType(dbConnection, pTestID)
                                    'TR 25/03/2011 -END.
                                End If

                                'AG 10/05/2013 - In those saved WS with FromLIS = 1 mark as deletedFlag but do not remove records 
                                'Mark as DeletedFlag if exists into a saved Worksession from LIS
                                'NOTE This code must be place before Delete the test in order to detect the LIS mapping value!!
                                If Not myGlobalDataTO.HasError Then
                                    Dim mySavedWSOTs As New SavedWSOrderTestsDelegate
                                    myGlobalDataTO = mySavedWSOTs.UpdateDeletedTestFlag(dbConnection, "STD", pTestID)
                                End If
                                'AG 10/05/2013

                                'TEST
                                If Not myGlobalDataTO.HasError Then
                                    'DELETE Test
                                    myGlobalDataTO = mytparTestsDAO.Delete(dbConnection, pTestID)
                                End If

                                'AG 10/05/2013 - Remove only in saved WS where FromLIS = 0 ('DL 08/05/2013)
                                'NOTE This code must be place after Delete the test in order to detect the deleted elements!!
                                Dim mySavedWSDelegate As New SavedWSDelegate

                                'Get all SavedWS created by Users (those created from processing of LIS messages or files) are excluded
                                myGlobalDataTO = mySavedWSDelegate.GetAll(dbConnection, False)   'SA 14/05/2013 - Changed the call due to the last parameter was removed
                                If Not myGlobalDataTO.HasError Then
                                    If Not myGlobalDataTO.SetDatos Is Nothing AndAlso DirectCast(myGlobalDataTO.SetDatos, SavedWSDS).tparSavedWS.Count > 0 Then
                                        Dim mySavedWS As SavedWSDS = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS)
                                        Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate

                                        For Each row As SavedWSDS.tparSavedWSRow In mySavedWS.tparSavedWS.Rows
                                            'Delete all elements included in the Saved WS that have been deleted
                                            myGlobalDataTO = mySavedWSOrderTests.ClearDeletedElements(dbConnection, row.SavedWSID)

                                            If Not myGlobalDataTO.HasError Then
                                                myGlobalDataTO = mySavedWSDelegate.DeleteEmptySavedWS(dbConnection, row.SavedWSID)
                                            End If

                                        Next
                                    End If
                                End If
                                'DL 08/05/2013
                            End If

                            ''AG 09/10/2012 - CloseSampleType in historic results and then close the test version
                            'If Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode Then
                            '    myGlobalDataTO = myTestSampleDelegate.HIST_CloseTestSample(dbConnection, pTestID) 'CloseSampleType (all sample type)
                            '    If Not myGlobalDataTO.HasError Then
                            '        myGlobalDataTO = myTestSampleDelegate.HIST_CloseTestVersion(dbConnection, pTestID, "BLANK") 'CloseTestVersion (blank and calib results for all sample type)
                            '    End If
                            'End If

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.DeleteTestCascade", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Search the curveresultid related to an existing result (if no curve then return -1)
        ''' </summary>
        ''' <param name="pdbConnection">Open DB Connection</param>
        ''' <param name="pTestID"></param>
        ''' <param name="pTestVersion"></param>
        ''' <param name="pSampleType"></param>
        ''' <returns>Integer indicating the last curve results ID belongs to the testid, testversion and optionally sample type</returns>
        ''' <remarks>Created by AG 22/03/2010 (Test Pending)</remarks>
        Private Function GetCurveResultID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                          ByVal pTestVersion As Integer, Optional ByVal pSampleType As String = "") As Integer
            Dim curveIDToReturn As Integer = -1
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim resultsDAO As New twksResultsDAO 'AG: Use the ResultsDAO instead using the ResultsDelegate due it isnt allowed (circular references)!!
                        Dim localres As New GlobalDataTO

                        'If no sampletype -> search blank results
                        If pSampleType.Trim = String.Empty Then
                            localres = resultsDAO.GetLastExecutedBlank(dbConnection, pTestID, pTestVersion)
                        Else    'If sampletype -> search calibrator results
                            localres = resultsDAO.GetLastExecutedCalibrator(dbConnection, pTestID, pSampleType, pTestVersion)
                        End If

                        If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                            Dim AdditionalElements As New WSAdditionalElementsDS
                            AdditionalElements = CType(localres.SetDatos, WSAdditionalElementsDS)

                            'Search the curve result ID
                            If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                                Dim foundOrderTestId As Integer = AdditionalElements.WSAdditionalElementsTable(0).PreviousOrderTestID

                                'Get blank results
                                localres = resultsDAO.GetAcceptedResults(dbConnection, foundOrderTestId)
                                If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                                    Dim lastResults As New ResultsDS
                                    lastResults = CType(localres.SetDatos, ResultsDS)

                                    If lastResults.twksResults.Rows.Count > 0 Then
                                        If Not lastResults.twksResults(0).IsCurveResultsIDNull Then curveIDToReturn = lastResults.twksResults(0).CurveResultsID
                                    End If

                                End If 'If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                            End If 'If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                        End If 'If Not localres.HasError And Not localres.SetDatos Is Nothing Then


                    End If 'If (Not dbConnection Is Nothing) Then
                End If 'If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestDelegate.GetCurveResultID", EventLogEntryType.Error, False)

                curveIDToReturn = -1    'If error dont delete curve results
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return curveIDToReturn
        End Function

        ''' <summary>
        ''' Get all Tests that are not using the specified Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of Tests do not linked to the specified
        '''          Calibrator</returns>
        ''' <remarks>
        ''' Created by: TR 04/05/2010
        ''' </remarks>
        Public Function GetTestToSetCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testsDAO As New tparTestsDAO
                        myGlobalDataTO = testsDAO.GetTestToSetCalibrator(dbConnection, pCalibratorID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetTestToSetCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Test/SampleTypes using an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier; optional parameter</param>
        ''' <param name="pSampleType">SampleType Code; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of Test/SampleTypes using 
        '''          an Experimental Calibrator</returns>
        ''' <remarks>
        ''' Created by:  TR 15/02/2011
        ''' Modified by: SA 15/11/2012 - Added optional parameters to allow get data of and specific TestID/SampleType
        ''' </remarks>
        Public Function GetCalibratorTestSampleList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pTestID As Integer = -1, _
                                                    Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testsDAO As New tparTestsDAO
                        myGlobalDataTO = testsDAO.GetCalibratorTestSampleList(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetCalibratorTestSampleList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For each deleted Test/SampleType, search the list of affected elements:
        '''   1-Test Profiles containing the Test/SampleType
        '''   2-Calculated Tests containing the Test/SampleType in their Formula
        '''     2.1-Test Profiles containing each affected Calculated Test
        '''     2.2-Calculated Tests containing each affected Calculated Test in their Formula
        '''   3-Linked Controls. When the linked Control have QC Results pending to cumulate for the Test/SampleType,
        '''     the warning message of Automatic Cumulation of QC Results will be shown
        ''' </summary>
        ''' <param name="pDeletedTestProgramingList">List of Test/SampleTypes selected for deletion</param>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected Elements</returns>
        ''' <remarks>
        ''' Created by:  TR 24/11/2010
        ''' Modified by: SA 06/05/2011 - The Icon for Calculated Tests was wrong 
        '''              SA 15/06/2012 - Called new implementation of QC functions (informing the TestType)
        '''              SA 13/09/2012 - Icons are obtained outside the loops and set to nothing at the end of the function
        ''' </remarks>
        Public Function ValidatedDependencies(ByVal pDeletedTestProgramingList As List(Of DeletedTestProgramingTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDependeciesElementsDS As New DependenciesElementsDS
                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                Dim imageBytesTPROF As Byte()
                Dim imageBytesTCALC As Byte()
                Dim imageBytesCTRL As Byte()
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate()

                Dim myTestProfileTestDS As New TestProfileTestsDS
                Dim myTestProfilesTestDelegate As New TestProfileTestsDelegate()

                Dim myFormulasDS As New FormulasDS
                Dim myRelatedElements As New FormulasDS
                Dim myFormulasDelegate As New FormulasDelegate

                Dim myTestControlsDS As New TestControlsDS
                Dim myTestControlsDelegate As New TestControlsDelegate

                Dim myQCResultDelegate As New QCResultsDelegate
                Dim myHistoryQCInfoDS As New HistoryQCInformationDS
                Dim myHistoryQCInfoList As New List(Of HistoryQCInformationDS.HistoryQCInfoTableRow)

                'Get text of Warning Message for QC Results cumulation in the current language
                Dim myWarningQCMSG As String = String.Empty
                Dim currentLanguageGlobal As New GlobalBase
                Dim myMessageDelegate As New MessageDelegate()

                myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString, _
                                                                         currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then myWarningQCMSG = myMessagesDS.tfmwMessages(0).MessageText

                    imageBytesTPROF = preloadedDataConfig.GetIconImage("PROFILES")
                    imageBytesTCALC = preloadedDataConfig.GetIconImage("TCALC")
                    imageBytesCTRL = preloadedDataConfig.GetIconImage("CTRL")

                    For Each myTestProgList As DeletedTestProgramingTO In pDeletedTestProgramingList
                        '1 - Verify affected Test Profiles (Test Profiles in which the deleted Test is included)
                        myTestProfileTestDS.tparTestProfileTests.Clear()

                        myGlobalDataTO = myTestProfilesTestDelegate.ReadByTestID(Nothing, myTestProgList.TestID, myTestProgList.SampleType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                            For Each testProfTest As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                                myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                                myDependenciesElementsRow.Type = imageBytesTPROF
                                myDependenciesElementsRow.Name = testProfTest.TestProfileName
                                myDependenciesElementsRow.FormProfileMember = myTestProgList.TestName

                                myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                            Next
                        Else
                            'Error verifying affected Test Profiles
                            Exit For
                        End If

                        '2 - Verify affected Calculated Tests (Calculated Tests having the deleted Test included in the Formula) 
                        myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(Nothing, myTestProgList.TestID, myTestProgList.SampleType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myFormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                            For Each formRow As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas.Rows
                                myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                                myDependenciesElementsRow.Type = imageBytesTCALC
                                myDependenciesElementsRow.Name = formRow.TestName
                                myDependenciesElementsRow.FormProfileMember = "=" & formRow.FormulaText
                                myDependenciesElementsRow.Related = False

                                myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)

                                'For the affected Calculated Test: 
                                '2.1 - Search if it is included in the Formula of other Calculated Tests 
                                myGlobalDataTO = myFormulasDelegate.ReadFormulaByTestID(Nothing, formRow.CalcTestID, formRow.SampleType, "CALC")
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myRelatedElements = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                                    For Each relatedElementRow As FormulasDS.tparFormulasRow In myRelatedElements.tparFormulas.Rows
                                        myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                                        myDependenciesElementsRow.Type = imageBytesTCALC
                                        myDependenciesElementsRow.Name = relatedElementRow.TestName
                                        myDependenciesElementsRow.FormProfileMember = "=" & relatedElementRow.FormulaText
                                        myDependenciesElementsRow.Related = True

                                        myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)

                                        '2.2 - Search if it is included in Test Profiles
                                        myGlobalDataTO = myTestProfilesTestDelegate.ReadByTestID(Nothing, relatedElementRow.CalcTestID, relatedElementRow.SampleType, "CALC")
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                                            For Each testProfTest As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                                                myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                                                myDependenciesElementsRow.Type = imageBytesTPROF
                                                myDependenciesElementsRow.Name = testProfTest.TestProfileName
                                                myDependenciesElementsRow.FormProfileMember = myTestProgList.TestName

                                                myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                            Next
                                        Else
                                            'Error verifying affected Test Profiles - 2nd level
                                            Exit For
                                        End If
                                    Next
                                Else
                                    'Error verifying affected Calculated Tests - 2nd level
                                    Exit For
                                End If
                            Next
                        Else
                            'Error verifying affected Calculated Tests
                            Exit For
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            '3 - Verify affected Controls
                            myGlobalDataTO = myTestControlsDelegate.GetControlsNEW(Nothing, "STD", myTestProgList.TestID, myTestProgList.SampleType)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestControlsDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                                If (myTestControlsDS.tparTestControls.Rows.Count > 0) Then
                                    'Search all Control/Lots having QC Results pending to cumulate for the specified Test/SampleType
                                    myGlobalDataTO = myQCResultDelegate.SearchPendingResultsByTestIDSampleTypeNEW(Nothing, "STD", myTestProgList.TestID, myTestProgList.SampleType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myHistoryQCInfoDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                                        For Each testControlRow As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls.Rows
                                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                            myDependenciesElementsRow.Type = imageBytesCTRL
                                            myDependenciesElementsRow.Name = testControlRow.ControlName
                                            myDependenciesElementsRow.FormProfileMember = myTestProgList.TestName & " [" & testControlRow.SampleType & "]"

                                            'Are there QC Results pending to cumulate for this Control/Lot?
                                            myHistoryQCInfoList = (From a In myHistoryQCInfoDS.HistoryQCInfoTable _
                                                                  Where a.ControlID = testControlRow.ControlID _
                                                                AndAlso String.Compare(a.SampleType, testControlRow.SampleType, False) = 0 _
                                                                 Select a).ToList()

                                            If (myHistoryQCInfoList.Count > 0) Then
                                                myDependenciesElementsRow.FormProfileMember &= " " & myWarningQCMSG
                                            End If
                                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                        Next

                                    Else
                                        'Error searching QC Results pending to cumulate
                                        Exit For
                                    End If
                                End If
                            Else
                                'Error getting the list of affected Controls
                                Exit For
                            End If
                        End If
                    Next
                    imageBytesTPROF = Nothing
                    imageBytesTCALC = Nothing
                    imageBytesCTRL = Nothing

                    If (Not myGlobalDataTO.HasError) Then
                        'Return the list of affected Elements
                        myGlobalDataTO.HasError = False
                        myGlobalDataTO.SetDatos = myDependeciesElementsDS
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.ValidatedDependencies", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Standard Tests/SampleTypes using Quality Control (those having QCActive = TRUE). If a Control Identifier 
        ''' is specified, then it also get the Standard Tests/SampleTypes with QCActive = FALSE but that are linked to the 
        ''' informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with all Tests/SampleTypes to show in the screen of 
        '''          Tests Selection when it is opened from Control Programming Screen</returns>
        ''' <remarks>
        ''' Created by:  DL 06/04/2011
        ''' Modified by: SA 10/05/2011 - Changed the LINQ to search the Test/SampleType linked to the specified ControlID
        '''                              to update field ActiveControl. Searching has to be done by TestID and SampleType, 
        '''                              not by ControlID and TestID 
        ''' </remarks>
        Public Function GetForControlsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pControlID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Tests/Sample Types having QCActive=TRUE
                        Dim mytparTestsDAO As New tparTestsDAO
                        resultData = mytparTestsDAO.GetAllWithQCActive(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myAllTestsDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                            If (pControlID > 0) Then
                                'A Control has been informed, get all Tests/SampleTypes linked to it
                                resultData = mytparTestsDAO.GetAllByControl(dbConnection, pControlID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myCtrlTestsDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                    For Each ctrlTestRow As TestsDS.tparTestsRow In myCtrlTestsDS.tparTests.Rows
                                        'The Test/SampleType has QCActive; inform in the DS to return, if it is Active for the Control
                                        If (ctrlTestRow.QCActive) Then
                                            Dim qControlTestList As List(Of TestsDS.tparTestsRow)
                                            qControlTestList = (From a In myAllTestsDS.tparTests _
                                                               Where a.TestID = ctrlTestRow.TestID _
                                                             AndAlso String.Compare(a.SampleType, ctrlTestRow.SampleType, False) = 0 _
                                                              Select a).ToList()

                                            If (qControlTestList.Count = 1) Then
                                                qControlTestList.First.ActiveControl = ctrlTestRow.ActiveControl
                                            End If
                                        Else
                                            'Move the Test/SampleType to the DS to return
                                            myAllTestsDS.tparTests.ImportRow(ctrlTestRow)
                                        End If
                                        myAllTestsDS.AcceptChanges()
                                    Next ctrlTestRow
                                End If
                            End If

                            resultData.SetDatos = myAllTestsDS
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.GetForControlsProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified Test/SampleType and in this case, create
        ''' a new cumulate for each linked Control/Lot
        ''' </summary>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 25/05/2011
        ''' Modified by: SA 20/12/2011 - After executing the Cumulation process, if the CalculationMode defined for the 
        '''                              Test/SampleType is STATISTIC, delete also the group of QC Results used to calculate
        '''                              the statistics values for each linked Control/Lot
        ''' </remarks>
        Private Function CalculateQCCumulate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                             ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Search if there are QC Results pending to cumulate for the Test/Sample Type
                Dim myQCResultsDelegate As New QCResultsDelegate

                myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByTestIDSampleTypeNEW(pDBConnection, "STD", pTestID, pSampleType)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myHistoryQCInfoDS As HistoryQCInformationDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                    Dim myCumulateResultDelegate As New CumulatedResultsDelegate
                    For Each historyQCInfoRow As HistoryQCInformationDS.HistoryQCInfoTableRow In myHistoryQCInfoDS.HistoryQCInfoTable.Rows
                        myGlobalDataTO = myCumulateResultDelegate.SaveCumulateResultNEW(pDBConnection, historyQCInfoRow.QCTestSampleID, _
                                                                                        historyQCInfoRow.QCControlLotID, historyQCInfoRow.AnalyzerID)
                        If (myGlobalDataTO.HasError) Then Exit For

                        If (String.Compare(historyQCInfoRow.CalculationMode, "STATISTIC", False) = 0) Then
                            'Delete the group of results used to calculate the statistic values
                            myGlobalDataTO = myQCResultsDelegate.DeleteStatisticResultsNEW(pDBConnection, historyQCInfoRow.QCTestSampleID, historyQCInfoRow.QCControlLotID, _
                                                                                           historyQCInfoRow.AnalyzerID)
                            If (myGlobalDataTO.HasError) Then Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.CalculateQCCumulate", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the LISValue by the testID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, pTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestsDAO As New tparTestsDAO
                        myGlobalDataTO = myTestsDAO.UpdateLISValueByTestID(dbConnection, pTestID, pLISValue)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateLISValueByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the sort of the Tests as alphabetically order by the name of the test (except User tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function UpdatePreloadedTestSortByTestName(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO

                        myGlobalDataTO = mytparTestsDAO.UpdatePreloadedTestSortByTestName(dbConnection)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdatePreloadedTestSortByTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the sort of the User Tests depending on Preloaded Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function UpdateUserTestPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestsDAO As New tparTestsDAO
                        Dim myLastPreloadedTestPosition As Integer

                        myGlobalDataTO = mytparTestsDAO.GetLastPreloadedTestPosition(dbConnection)
                        If (Not myGlobalDataTO.HasError) Then
                            If (myGlobalDataTO.SetDatos Is Nothing OrElse myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                myLastPreloadedTestPosition = 0
                            Else
                                myLastPreloadedTestPosition = CType(myGlobalDataTO.SetDatos, Integer)
                            End If

                            myGlobalDataTO = mytparTestsDAO.UpdateUserTestPosition(dbConnection, myLastPreloadedTestPosition)

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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateUserTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets all STD tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
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
                        Dim myDAO As New tparTestsDAO
                        resultData = myDAO.GetCustomizedSortedTestSelectionList(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TestsDelegate.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for STD tests
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
                        Dim myDAO As New tparTestsDAO
                        myGlobalDataTO = myDAO.UpdateCustomPositionAndAvailable(dbConnection, pTestsSortingDS)

                        'Get the not Available TestID and look for all CALC test or profiles affected -> Set them also as not available
                        Dim notAvailableItemList As List(Of ReportsTestsSortingDS.tcfgReportsTestsSortingRow)
                        notAvailableItemList = (From a As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.Available = False Select a).ToList
                        If notAvailableItemList.Count > 0 Then
                            'Look for other calculated tests that uses it in their formula and set available = False
                            Dim myCalcTestDlg As New CalculatedTestsDelegate
                            myGlobalDataTO = myCalcTestDlg.ResetAvailableCascade(dbConnection, notAvailableItemList, "STD")

                            If Not myGlobalDataTO.HasError Then
                                Dim myTestProfileDlg As New TestProfilesDelegate
                                myGlobalDataTO = myTestProfileDlg.ResetAvailableCascade(dbConnection, notAvailableItemList, "STD")
                            End If

                        End If
                        notAvailableItemList = Nothing

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestsDelegate.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function


#End Region
    End Class
End Namespace
