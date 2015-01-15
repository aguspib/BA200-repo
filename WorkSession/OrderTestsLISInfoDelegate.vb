Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Partial Public Class OrderTestsLISInfoDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Search the AwosID in the active Work Session
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAwosID"></param>
        ''' <returns>GlobalDataTo (setData as OrderTestsLISInfoDS)</returns>
        ''' <remarks>
        ''' Created  by SG 18/03/2013
        ''' </remarks>
        Public Function GetOrderTestInfoByAwosID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.GetOrderTestInfoByAwosID(dbConnection, pAwosID)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.GetOrderTestInfoByAwosID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData


        End Function

        ''' <summary>
        ''' Read by OK
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>GlobalDataTo (setData as OrderTestsLISInfoDS)</returns>
        ''' <remarks>
        ''' Created  by AG 06/03/2013
        ''' Modified by XB 08/03/2013 - Add pRerunNumber parameter
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.Read(dbConnection, pOrderTestID, pRerunNumber)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myLISInfoDS As New OrderTestsLISInfoDS
                            myLISInfoDS = CType(resultData.SetDatos, OrderTestsLISInfoDS)
                            If myLISInfoDS.twksOrderTestsLISInfo.Rows.Count = 0 And pRerunNumber <> 1 Then
                                ' If rerun's not found, try the search with value by default 1
                                resultData = myDAO.Read(dbConnection, pOrderTestID, 1)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.Read", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData


        End Function

        ''' <summary>
        ''' Delete all Tests by Order Test Id
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Identifier</param>
        ''' <param name="pUpdateLISRequest">Indicate if want to update the </param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 18/03/2013
        ''' Modified by: TR 20/03/2013 -Add the parameter pUpdateLISRequest. In case UpdateLISRequest = true then
        '''                             Update the LISStatus field by OrderTestID.
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                                                 Optional pUpdateLISRequest As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If (Not resultData.HasError) Then
                            Dim myOrderTestLISInfoDAO As New twksOrderTestsLISInfoDAO
                            resultData = myOrderTestLISInfoDAO.Delete(dbConnection, pOrderTestID, pRerunNumber)
                        End If
                        If (Not resultData.HasError) Then
                            If pUpdateLISRequest Then
                                Dim myOrderTestDelegate As New OrderTestsDelegate
                                'Update the LISRequest.
                                resultData = myOrderTestDelegate.UpdateLISRequestByOrderTestID(dbConnection, pOrderTestID, pUpdateLISRequest)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of AwosID that has been received from LIS for the existing Order Test; sort them by RerunNumber DESC
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <returns>GlobalDataTo (setData as OrderTestsLISInfoDS)</returns>
        ''' <remarks>
        ''' Created  by XB 21/03/2013
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.GetByOrderTestID(dbConnection, pOrderTestID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.GetByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update information of the specified Order Tests LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS containing the list of Order Tests LIS to update</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsLISInfoDS As OrderTestsLISInfoDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.Update(dbConnection, pOrderTestsLISInfoDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a group of OrderTests with LIS information in table twksOrderTestsLISInfo. When parameter pUpdateOrderTest is TRUE, the flag LISRequest of the 
        ''' OrderTests is updated to TRUE in table twksOrderTests
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestsLISInfoDS"></param>
        ''' <param name="pUpdateOrderTest">When TRUE, the flag LISRequest of the OrderTests in the DS is updated to TRUE in table twksOrderTests</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  TR 26/03/2013
        ''' Modified by: SA 31/05/2013 - Added parameter pUpdateOrderTest
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsLISInfoDS As OrderTestsLISInfoDS, ByVal pUpdateOrderTest As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.Create(dbConnection, pOrderTestsLISInfoDS)

                        If (Not resultData.HasError) Then
                            If (pUpdateOrderTest) Then
                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                For Each pRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pOrderTestsLISInfoDS.twksOrderTestsLISInfo
                                    resultData = myOrderTestsDelegate.UpdateLISRequestByOrderTestID(dbConnection, pRow.OrderTestID, True)
                                Next
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all records in table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo (OrderTestsLISInfoDS)</returns>
        ''' <remarks>AG 24/04/2013</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.ReadAll(dbConnection)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' For an specific read Barcode, verify if it corresponds to an SpecimenID sent by LIS, and in this case, get the OrderTestID, Patient/Sample ID,
        ''' Sample Type and the flag indicating if the required tube has been sent to positioning or not
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSpecimenID">Specimen Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with data of all OrderTests requested by LIS for the specified SpecimenID</returns>
        ''' <remarks>
        ''' Created by: SA 09/04/2013
        ''' </remarks>
        Public Function GetOrderTestBySpecimenID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSpecimenID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.GetOrderTestBySpecimenID(dbConnection, pSpecimenID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.GetOrderTestBySpecimenID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 22/04/2013
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.ResetWS(dbConnection)

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
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests with status OPEN  belonging to Orders of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pSampleClass"></param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created  by XB 26/04/2013
        ''' </remarks>
        Public Function DeleteOpenOrderTestsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If (Not resultData.HasError) Then
                            Dim myOrderTestLISInfoDAO As New twksOrderTestsLISInfoDAO
                            resultData = myOrderTestLISInfoDAO.DeleteOpenOrderTestsBySampleClass(dbConnection, pSampleClass)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.DeleteOpenOrderTestsBySampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For each coupple PatientID-SampleType return the related SpecimenID's list
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 29/03/2013
        ''' </remarks>
        Public Function GetSpecimensByPatientSampleType(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.GetSpecimensByPatientSampleType(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.GetSpecimensByPatientSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Patient Identifier, get all Order Tests requested by LIS (filtering data by field LISPatientID). The SampleType of each 
        ''' Order Test is also returned. Function created for BT #1453 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier sent by LIS</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with the list of LIS Order Tests received for the informed Patient</returns>
        ''' <remarks>
        ''' Created by:  SA 14/01/2014 
        ''' </remarks>
        Public Function GetLISInfoByLISPatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderTestsLISInfoDAO
                        resultData = myDAO.GetLISInfoByLISPatient(dbConnection, pPatientID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsLISInfoDelegate.GetLISInfoByLISPatient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace
