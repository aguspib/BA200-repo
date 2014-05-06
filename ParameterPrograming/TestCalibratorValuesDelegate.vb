Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class TestCalibratorValuesDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Add values for one point of an experimental Calibrator when it is used for an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorValuesDS">Typed DataSet TestCalibratorValuesDS containing the Test Calibrator Values to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 09/06/2010
        ''' Modified by: AG 12/11/2010 - Due to continous PK errors, before add the Test Calibrator Values, verify the TestCalibratorID already exists in DB
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorValuesDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the informed TestCalibratorID exist in table tparTestCalibrators
                        Dim myTCDelegate As New TestCalibratorsDelegate
                        myGlobalDataTO = myTCDelegate.Exists(dbConnection, pTestCalibratorValuesDS.tparTestCalibratorValues(0).TestCalibratorID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myDS As TestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                            If (myDS.tparTestCalibrators.Rows.Count > 0) Then
                                'Add the Test Calibrator Values
                                Dim myTestCalibratorValuesDAO As New tparTestCalibratorValuesDAO
                                myGlobalDataTO = myTestCalibratorValuesDAO.Create(dbConnection, pTestCalibratorValuesDS)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Calibrator values for the specified TestCalibratorID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByTestCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestCalibratorValuesDAO As New tparTestCalibratorValuesDAO
                        myGlobalDataTO = myTestCalibratorValuesDAO.DeleteByTestsCalibratorID(dbConnection, pTestCalibratorID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the number of points of the Experimental Calibrator used for the Test/SampleType by searching the number of points with defined values
        ''' (theorical concentration and factor). Considerations:
        ''' ** If the Test/SampleType uses a Factor or Alternative Calibrator, this function will return zero (due to the Calibrator Type is not experimental)
        ''' ** For Multipoint Calibrators, the definition for every point
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of points of the Experimental Calibrator used for the informed Test/SampleType. 
        '''          Value 0 indicates the Test/SampleType uses a Factor or Alternative Calibrator</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: BK 23/12/2009 - Change datatype Int16 to Integer
        '''              SA 20/07/2010 - Returns a GlobalDataTO instead of an Integer value
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetNumberOfCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testCalibratorValuesData As New tparTestCalibratorValuesDAO
                        resultData = testCalibratorValuesData.GetNumberOfPointsDefined(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.GetNumberOfCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get values of all points of a Calibrator searching by TestCalibratorID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorValuesDS with the values of the Calibrator points</returns>
        ''' <remarks>
        ''' Created by:  TR 03/06/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetTestCalibratorValuesByTestCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testCalibratorValuesData As New tparTestCalibratorValuesDAO
                        myGlobalDataTO = testCalibratorValuesData.ReadByTestCalibratorID(dbConnection, pTestCalibratorID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.GetTestCalibratorValuesByTestCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get values of all points of a Calibrator searching by TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorValuesDS with the values of the Calibrator points</returns>
        ''' <remarks>
        ''' Created by:  TR 14/06/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetTestCalibratorValuesByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testCalibratorValuesData As New tparTestCalibratorValuesDAO
                        myGlobalDataTO = testCalibratorValuesData.ReadByTestIDSampleType(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.GetTestCalibratorValuesByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values of one point of an experimental Calibrator when it is used for an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorValuesDS">Typed DataSet TestCalibratorValuesDS containing the Test Calibrator Value to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 09/06/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorValuesDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestCalibratorValuesDAO As New tparTestCalibratorValuesDAO
                        myGlobalDataTO = myTestCalibratorValuesDAO.Update(dbConnection, pTestCalibratorValuesDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "TO REVIEW!!"
        ''' <summary>
        ''' Get Test Calibrator Values by the Test Calibrator ID and Test ID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  DL 07/09/2010
        ''' </remarks>
        Public Function GetTestCalibratorValuesByTestCalibratorIDAndTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer, _
                                                                           ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testCalibratorValuesData As New tparTestCalibratorValuesDAO
                        myGlobalDataTO = testCalibratorValuesData.ReadByTestCalibratorIDAndTestID(dbConnection, pTestCalibratorID, pTestID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorValuesDelegate.GetTestCalibratorValuesByTestCalibratorIDAndTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function
#End Region

    End Class
End Namespace