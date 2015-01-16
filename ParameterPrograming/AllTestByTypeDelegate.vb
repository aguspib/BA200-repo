
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO


Namespace Biosystems.Ax00.BL

    Public Class AllTestByTypeDelegate
        ''' <summary>
        ''' Search in the DS containing all LIS Mappings defined for Tests (all Test Types), the LIS Value for the specified TestType/TestID
        ''' </summary>
        ''' <param name="pTestMappingDS">Typed DS AllTestsByTypeDS containing all LIS Mappings defined for Tests (all Test Types)</param>
        ''' <param name="pTestId">Test Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <returns>GlobalDataTO containing an string value with the LIS Value for the TestType/TestID or an Error indicating the mapping is not defined</returns>
        ''' <remarks>
        ''' Created by: SGM 22/03/2013
        ''' </remarks>
        Public Function GetLISTestID(ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pTestId As Integer, ByVal pTestType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim myMapTestIdRows As List(Of AllTestsByTypeDS.vparAllTestsByTypeRow) = (From r As AllTestsByTypeDS.vparAllTestsByTypeRow In pTestMappingDS.vparAllTestsByType _
                                                                                         Where r.TestID = pTestId _
                                                                                       AndAlso r.TestType.ToUpperBS = pTestType.ToUpperBS _
                                                                                        Select r).ToList()
                If (myMapTestIdRows.Count > 0) Then
                    If (Not myMapTestIdRows(0).IsLISValueNull AndAlso myMapTestIdRows(0).LISValue.Length > 0) Then
                        resultData.SetDatos = myMapTestIdRows(0).LISValue
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                    End If
                End If
                myMapTestIdRows = Nothing

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LISMappingsDelegate.GetLISTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in view vparAllTestsByType the LIS Mapping Value defined for the informed TestType/TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing an string value with the LIS Value for the TestType/TestID</returns>
        ''' <remarks>
        ''' Created by: TR 02/05/2013
        ''' </remarks>
        Public Function GetLISValue(ByVal pDBConnection As SqlClient.SqlConnection, pTestType As String, pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myvparAllTestByTypeDAO As New vparAllTestByTypeDAO
                        myGlobalDataTO = myvparAllTestByTypeDAO.GetLISValue(dbConnection, pTestType, pTestID)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AllTestByTypeDelegate.GetLISValue", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the full content of view vparAllTestsByType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AllTestsByTypeDS with all LIS mappings defined for all Test Types</returns>
        ''' <remarks>
        ''' Created by:  TR 04/03/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myvparAllTestByTypeDAO As New vparAllTestByTypeDAO
                        myGlobalDataTO = myvparAllTestByTypeDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AllTestByTypeDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the LIS Value on the diferents tables (Test, ISETest, OffSystemsTest, CalculatedTest)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAllTestByTypeDS">AllTestByTypeDS.</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValues(ByVal pDBConnection As SqlClient.SqlConnection, pAllTestByTypeDS As AllTestsByTypeDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myTestsDelegate As New TestsDelegate
                        Dim myISETestsDelegate As New ISETestsDelegate
                        Dim myOffSystemsDelegate As New OffSystemTestsDelegate
                        Dim myCalculatedTestDelegate As New CalculatedTestsDelegate
                        'Go thought each test Type and update LISValue
                        For Each AllTestByTypeRow As AllTestsByTypeDS.vparAllTestsByTypeRow In pAllTestByTypeDS.vparAllTestsByType.Rows
                            'validate the testtype and call the corresponding delegate
                            Select Case AllTestByTypeRow.TestType
                                Case "STD"
                                    'Update the Test.
                                    myGlobalDataTO = myTestsDelegate.UpdateLISValueByTestID(dbConnection, _
                                                                     AllTestByTypeRow.TestID, AllTestByTypeRow.LISValue)
                                    Exit Select
                                Case "ISE"
                                    'Update the ISE Test.
                                    myGlobalDataTO = myISETestsDelegate.UpdateLISValueByTestID(dbConnection, _
                                                                        AllTestByTypeRow.TestID, AllTestByTypeRow.LISValue)
                                    Exit Select
                                Case "OFFS"
                                    'Update Off System Test.
                                    myGlobalDataTO = myOffSystemsDelegate.UpdateLISValueByTestID(dbConnection, _
                                                                          AllTestByTypeRow.TestID, AllTestByTypeRow.LISValue)
                                    Exit Select
                                Case "CALC"
                                    'Update Calcualated Test.
                                    myGlobalDataTO = myCalculatedTestDelegate.UpdateLISValueByTestID(dbConnection, _
                                                                              AllTestByTypeRow.TestID, AllTestByTypeRow.LISValue)
                                    Exit Select
                            End Select

                            If myGlobalDataTO.HasError Then Exit For
                        Next


                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
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
                GlobalBase.CreateLogActivity(ex.Message, "LISMappingsDelegate.UpdateLISValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

    End Class

End Namespace
