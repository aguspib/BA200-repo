Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparTestSamplesMultirulesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a Multirule for an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSampleMultiRules">Typed DataSet TestSamplesMultirulesDS containing the Multirule to add for an 
        '''                                     specific TestID/SampleType</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 05/04/2011
        ''' Modified by: SA 10/05/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleMultiRules As TestSamplesMultirulesDS) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO tparTestSamplesMultirules (TestID, SampleType, RuleID, SelectedRule) " & vbCrLf

                    cmdText &= " VALUES " & vbCrLf
                    cmdText &= String.Format(" ({0}{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).TestID, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).SampleType, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).RuleID, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}) ", pTestSampleMultiRules.tparTestSamplesMultirules(0).SelectedRule.ToString(), vbCrLf)


                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Multirules for the specified Test and optionally for an specific SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: SA 10/05/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tparTestSamplesMultirules " & vbCrLf & _
                                            " WHERE  TestID = " & pTestID.ToString & vbCrLf

                    If (Not pSampleType.Trim = String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all Multirules defined for the specified TestID/SampleType. Optionally, only the Multirules selected to be applied for
        ''' the TestID/SampleType are returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pOnlyActiveRules">Optional parameter. When TRUE, it indicates only the Multirules selected to be applied for 
        '''                                the TestID/SampleType will be returned</param>
        ''' <returns>GlobalDataTO containing a typed Dataset with the list of Multirules</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: TR 17/05/2011 - Added optional parammeter pOnlyActiveRules to get only the Multirules selected to be applied for
        '''                              the TestID/SampleType
        '''              SA 10/05/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                                  Optional ByVal pOnlyActiveRules As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTestSamplesMultirules " & vbCrLf & _
                                                " WHERE  TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        If (pOnlyActiveRules) Then cmdText &= " AND SelectedRule = 1 " & vbCrLf

                        Dim mytTestSamplesMultirulesDS As New TestSamplesMultirulesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytTestSamplesMultirulesDS.tparTestSamplesMultirules)
                            End Using
                        End Using

                        resultData.SetDatos = mytTestSamplesMultirulesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NEW QC FUNCTION"
        ''' <summary>
        ''' Add a Multirule for an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSampleMultiRules">Typed DataSet TestSamplesMultirulesDS containing the Multirule to add for an 
        '''                                     specific TestID/SampleType</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 05/04/2011
        ''' Modified by: SA 10/05/2012 - Changed the function template
        '''              SA 05/06/2012 - Insert also field TestType
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleMultiRules As TestSamplesMultirulesDS) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO tparTestSamplesMultirules (TestType, TestID, SampleType, RuleID, SelectedRule) " & vbCrLf

                    cmdText &= " VALUES " & vbCrLf
                    cmdText &= String.Format(" ('{0}'{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).TestType, vbCrLf)
                    cmdText &= String.Format(" ,{0}{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).TestID, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).SampleType, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}", pTestSampleMultiRules.tparTestSamplesMultirules(0).RuleID, vbCrLf)
                    cmdText &= String.Format(" ,'{0}'{1}) ", pTestSampleMultiRules.tparTestSamplesMultirules(0).SelectedRule.ToString(), vbCrLf)


                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Multirules for the specified Test and optionally for an specific SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: SA 10/05/2012 - Changed the function template
        '''              SA 05/06/2012 - Added parameter for TestType and filter the query for this field
        ''' </remarks>
        Public Function DeleteByTestIDSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                    Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tparTestSamplesMultirules " & vbCrLf & _
                                            " WHERE  TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                            " AND    TestID = " & pTestID.ToString & vbCrLf

                    If (Not pSampleType.Trim = String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Multirules defined for the specified TestID/SampleType. Optionally, only the Multirules selected to be applied for
        ''' the TestID/SampleType are returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pOnlyActiveRules">Optional parameter. When TRUE, it indicates only the Multirules selected to be applied for 
        '''                                the TestID/SampleType will be returned</param>
        ''' <returns>GlobalDataTO containing a typed Dataset with the list of Multirules</returns>
        ''' <remarks>
        ''' Created by:  TR 04/04/2011
        ''' Modified by: TR 17/05/2011 - Added optional parammeter pOnlyActiveRules to get only the Multirules selected to be applied for
        '''                              the TestID/SampleType
        '''              SA 10/05/2012 - Changed the function template
        '''              SA 05/06/2012 - Added parameter for TestType and filter the query for this field
        ''' </remarks>
        Public Function ReadByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, Optional ByVal pOnlyActiveRules As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTestSamplesMultirules " & vbCrLf & _
                                                " WHERE  TestType   = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        If (pOnlyActiveRules) Then cmdText &= " AND SelectedRule = 1 " & vbCrLf

                        Dim mytTestSamplesMultirulesDS As New TestSamplesMultirulesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytTestSamplesMultirulesDS.tparTestSamplesMultirules)
                            End Using
                        End Using

                        resultData.SetDatos = mytTestSamplesMultirulesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesMultirulesDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace

