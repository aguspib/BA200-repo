Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksOrdersDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pNewOrder">DataSet containing the data of the Order to create</param>
        ''' <returns>Global object containing error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 23/02/2010 - Added new field SampleID to the insert
        '''              SA 28/10/2010 - Added N preffix for multilanguage of fields SampleID and TS_User; if audit fields
        '''                              are not informed in the parameter DS, use the current logged User and the current date
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pNewOrder As OrdersDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                ElseIf (Not pNewOrder Is Nothing) Then
                    Dim cmdText As String = " INSERT INTO twksOrders (OrderID, SampleClass, StatFlag, PatientID, SampleID, OrderDateTime, OrderStatus, " & vbCrLf & _
                                                                    " ExternalOID, ExternalDateTime, ExportDateTime, TS_User, TS_DateTime) " & vbCrLf & _
                                            " VALUES ('" & pNewOrder.twksOrders(0).OrderID & "', " & vbCrLf & _
                                                    " '" & pNewOrder.twksOrders(0).SampleClass & "', " & vbCrLf & _
                                                    IIf(pNewOrder.twksOrders(0).StatFlag, 1, 0).ToString & ", " & vbCrLf
                    '" '" & pNewOrder.twksOrders(0).SampleClass.ToUpper & "', " & vbCrLf & _

                    'Verify if PatientID is informed
                    If (pNewOrder.twksOrders(0).IsPatientIDNull) Then
                        cmdText = cmdText & " NULL, " & vbCrLf
                    Else
                        cmdText = cmdText & " N'" & pNewOrder.twksOrders(0).PatientID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Verify if SampleID is informed
                    If (pNewOrder.twksOrders(0).IsSampleIDNull) Then
                        cmdText = cmdText & " NULL, " & vbCrLf
                    Else
                        cmdText = cmdText & " N'" & pNewOrder.twksOrders(0).SampleID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Order Date Time and Order Status are always informed
                    cmdText = cmdText & " '" & pNewOrder.twksOrders(0).OrderDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                        " '" & pNewOrder.twksOrders(0).OrderStatus & "', " & vbCrLf

                    'Verify if External Order ID and External Date Time are informed (only for Orders sent from an external LIS 
                    If (pNewOrder.twksOrders(0).IsExternalOIDNull) Then
                        cmdText = cmdText & " NULL, NULL, " & vbCrLf
                    Else
                        cmdText = cmdText & " '" & pNewOrder.twksOrders(0).ExternalOID.Trim.Replace("'", "''") & "', " & vbCrLf
                        If (pNewOrder.twksOrders(0).IsExternalDateTimeNull) Then
                            'If the LIS does not send the Order Date Time, then the current day is used
                            cmdText = cmdText & " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                        Else
                            'The Date and Time sent by the external LIS is used
                            cmdText = cmdText & " '" & pNewOrder.twksOrders(0).ExternalDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                        End If
                    End If

                    'Export Date Time is always NULL when the Order is created
                    cmdText = cmdText & " NULL, " & vbCrLf

                    'Audit fields 
                    If (pNewOrder.twksOrders(0).IsTS_UserNull) Then
                        Dim myGlobalBase As New GlobalBase
                        cmdText = cmdText & " N'" & myGlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText = cmdText & " N'" & pNewOrder.twksOrders(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If
                    If (pNewOrder.twksOrders(0).IsTS_DateTimeNull) Then
                        cmdText = cmdText & " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    Else
                        cmdText = cmdText & " '" & pNewOrder.twksOrders(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    End If

                    'Execute the SQL Sentence
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrders " & _
                                            " WHERE  OrderID = '" & pOrderID & "' "

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read data of the specified Order 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the specified Order</returns>
        ''' <remarks>
        ''' Created by:  SA 18/03/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksOrders " & vbCrLf & _
                                                " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Orders created in the specified Date
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderDateTime">Order Date</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with the list of Orders created in the 
        '''          specified Date</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 18/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''                              Changed the returned value for a GlobalDataTO with DataSet OrdersDS inside
        ''' </remarks>
        Public Function ReadByOrderDateTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderDateTime As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderID, SampleClass, StatFlag, PatientID, OrderDateTime, SequenceNumber, OrderStatus, " & vbCrLf & _
                                                       " ExternalOID, ExternalDateTime, ExportDateTime, TS_User, TS_DateTime " & vbCrLf & _
                                                " FROM   twksOrders " & vbCrLf & _
                                                " WHERE  CONVERT(VARCHAR, OrderDateTime, 112) = '" & pOrderDateTime.ToString("yyyyMMdd") & "' " & vbCrLf

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadByOrderDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Orders defined for the specified SampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleID">Sample Identifier to search</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the Orders for the specified SampleID </returns>
        ''' <remarks>
        ''' Created by:  SA 04/06/2013
        ''' </remarks>
        Public Function ReadBySampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksOrders " & vbCrLf & _
                                                " WHERE  UPPER(SampleID) = UPPER(N'" & pSampleID.Trim.Replace("'", "''") & "') " & vbCrLf

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadBySampleID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Modify data of an existing Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderDS">DataSet containing the data of the Order to update</param>
        ''' <returns>Global object containing error information</returns>
        ''' <remarks>
        ''' Created by:  SA 05/03/2010 
        ''' Modified by: SA 13/10/2010 - If field Order Status is not informed, do not update it
        '''              SA 28/10/2010 - Added N preffix for multilanguage of fields SampleID and TS_User; if audit fields
        '''                              are not informed in the parameter DS, use the current logged User and the current date.
        '''                              Removed error raising based in number of AffectedRecords
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pOrderDS As OrdersDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                            " SET    StatFlag = " & IIf(pOrderDS.twksOrders(0).StatFlag, 1, 0).ToString & ", " & vbCrLf

                    'Verify if PatientID is informed
                    If (pOrderDS.twksOrders(0).PatientID = "") Then
                        cmdText &= " PatientID = NULL, " & vbCrLf
                    Else
                        cmdText &= " PatientID = N'" & pOrderDS.twksOrders(0).PatientID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Verify if SampleID is informed
                    If (pOrderDS.twksOrders(0).SampleID = "") Then
                        cmdText &= " SampleID = NULL, " & vbCrLf
                    Else
                        cmdText &= " SampleID = N'" & pOrderDS.twksOrders(0).SampleID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Order Status is updated only if it has been informed
                    If (Not pOrderDS.twksOrders(0).IsOrderStatusNull) Then
                        cmdText &= " OrderStatus = '" & pOrderDS.twksOrders(0).OrderStatus & "', " & vbCrLf
                    End If

                    'Audit fields
                    If (pOrderDS.twksOrders(0).IsTS_UserNull) Then
                        Dim myGlobalBase As New GlobalBase
                        cmdText &= " TS_User = N'" & myGlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " TS_User = N'" & pOrderDS.twksOrders(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If
                    If (pOrderDS.twksOrders(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    Else
                        cmdText &= " TS_DateTime = '" & pOrderDS.twksOrders(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    End If

                    cmdText += " WHERE OrderID = '" & pOrderDS.twksOrders(0).OrderID & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Deletes the informed Order if it has not Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteEmptyOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksOrders " & vbCrLf & _
                                            " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf & _
                                            " AND    OrderID NOT IN (SELECT OrderID FROM twksOrderTests) " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.DeleteEmptyOrder", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete all Orders of the specified SampleClass that have not Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  SA 25/02/2010
        ''' </remarks>
        Public Function DeleteEmptyOrdersBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksOrders " & vbCrLf & _
                                            " WHERE  SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                            " AND    OrderID NOT IN (SELECT OrderID FROM twksOrderTests) " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.DeleteEmptyOrdersBySampleClass", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the number of Blanks, Calibrators, Controls and Patient's Samples requested for aN specific TestID/SampleType inside the 
        ''' list of Order Tests included in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a Work Session</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SampleClassesByTestDS with the number of Orders requested of each different 
        '''          Sample Class for a specific TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/06/2010 - Changed the way of opening the DB Connection to fulfill the new template; return a GlobalDataTO 
        '''                              instead of a typed Dataset; added optional parameter for the Work Session ID, needed to improve 
        '''                              the performance; query changed to use the WorkSessionID instead of the list of Order Tests
        '''              SA 16/01/2012 - Changed the query to get the number of replicates requested for each different SampleClass
        '''                              instead the number of Orders; changed the function template 
        '''              SA 11/04/2012 - Changed the subquery used when parameter WorkSessionID is informed to get only OrderTests 
        '''                              having ToSendFlag=TRUE 
        ''' </remarks>
        Public Function GetSampleClassesByTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, ByVal pTestID As Integer, _
                                               ByVal pSampleType As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.SampleClass, SUM(OT.ReplicatesNumber) AS NumReplicates " & vbCrLf & _
                                                " FROM   twksOrders O INNER JOIN twksOrderTests OT ON O.OrderID = OT.OrderID " & vbCrLf & _
                                                " WHERE  OT.TestID     =  " & pTestID.ToString & vbCrLf & _
                                                " AND    UPPER(OT.SampleType) = UPPER('" & pSampleType.Trim & "') " & vbCrLf

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.TestType IN ('STD', 'ISE') " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE WorkSessionID = '" & pWorkSessionID.ToString & "' " & vbCrLf & _
                                                              " AND   OpenOTFlag = 0 " & vbCrLf & _
                                                              " AND   ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        cmdText &= " GROUP BY O.SampleClass " & vbCrLf & _
                                   " ORDER BY O.SampleClass " & vbCrLf

                        Dim resultData As New SampleClassesByTestDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.SampleClassesByTest)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.GetSampleClassesByTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read Orders by ExternalOID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExternalOID"></param>
        ''' <param name="pSampleType" ></param>
        ''' <returns>WSRequiredElementsDS</returns>
        ''' <remarks>
        ''' Created by:  AG 29/08/2011
        ''' </remarks>
        Public Function ReadByExternalOID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExternalOID As String, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT RE.ElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN twksWSRequiredElemByOrderTest REOT ON REOT.ElementID = RE.ElementID " & vbCrLf & _
                                                                                                                              " AND RE.TubeContent = 'PATIENT' " & vbCrLf & _
                                                                                 " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                 " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE O.ExternalOID = N'" & pExternalOID.Trim.Replace("'", "''") & "' " & vbCrLf

                        If (pSampleType <> "") Then cmdText &= " AND OT.SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myDataSet As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSRequiredElements)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadByExternalOID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read data of the Order to which the specified OrderTest belongs 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the Order to which the specified OrderTest belongs</returns>
        ''' <remarks>
        ''' Created by:  SA 04/07/2012
        ''' </remarks>
        Public Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.* FROM twksOrders O INNER JOIN twksOrderTests OT ON O.OrderID = OT.OrderID " & vbCrLf & _
                                                " WHERE  OT.OrderTestID = " & pOrderTestID.ToString & vbCrLf

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the Order has to be re-opened (when an Order is closed but a new 
        ''' OrderTest is added to it, the Order is re-opened)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Identifier of the Order</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/06/2010
        ''' </remarks>
        Public Function ReOpenClosedOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                            " SET    OrderStatus = 'OPEN' " & vbCrLf & _
                                            " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf & _
                                            " AND    OrderStatus = 'CLOSED' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReOpenClosedOrder", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Orders included not included in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrders " & vbCrLf & _
                                            " WHERE  OrderID NOT IN (SELECT OrderID FROM twksOrderTests) " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields PatientID and SampleID for the specified Order, depending on value of parameter pSampleIDType:
        ''' ** If MAN, update SampleID = pSampleID and PatientID = NULL
        ''' ** If DB,  update PatientID = pSampleID and SampleID = NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleID">Sample or Patient Identifier to update</param>
        ''' <param name="pSampleIDType">Type of Sample ID: DB or MAN</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/08/2013 
        ''' </remarks>
        Public Function UpdatePatientSampleFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pSampleID As String, ByVal pSampleIDType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                            " SET    PatientID = " & IIf(pSampleIDType = "DB", "N'" & pSampleID.Trim.Replace("'", "''") & "', ", "NULL,").ToString & vbCrLf & _
                                            "        SampleID  = " & IIf(pSampleIDType = "MAN", "N'" & pSampleID.Trim.Replace("'", "''") & "' ", "NULL").ToString & vbCrLf & _
                                            " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.UpdatePatientSampleFields", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the status of the specified Order with the informed value
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pNewStatus">New Order Status</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/06/2010
        ''' Modified by: SA 13/09/2010 - Moved here from twksOrdersGeneratedDAO 
        '''              XB 20/03/2014 - Add parameter HResult into Try Catch section - #1548
        ''' </remarks>
        Public Function UpdateOrderStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pNewStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrders SET OrderStatus = '" & pNewStatus & "' " & vbCrLf & _
                                            " WHERE OrderID = '" & pOrderID & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksOrdersDAO.UpdateOrderStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update flags indicating if an Order has been printed and/or exported to an external LIMS system
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleID">Sample or Patient Identifier</param>
        ''' <param name="pPrintState">New value to print field</param>
        ''' <param name="pExportState">New value to Export field</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 16/03/2011
        ''' Modified by: SA 01/02/2012 - Added N prefix and replace
        ''' </remarks>
        Public Function UpdateOutputBySampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleID As String, ByVal pPrintState As Boolean, _
                                               ByVal pExportState As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                            " SET    OrderToPrint  = " & Convert.ToInt32(IIf(pPrintState, 1, 0)) & ", " & vbCrLf & _
                                            "        OrderToExport = " & Convert.ToInt32(IIf(pExportState, 1, 0)) & vbCrLf & _
                                            " WHERE  SampleClass   = 'PATIENT' " & vbCrLf & _
                                            " AND   (SampleID = N'" & pSampleID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " OR     PatientID = N'" & pSampleID.Trim.Replace("'", "''") & "') " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.UpdateOutputBySampleID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Orders having Tests of the specified Test Type and, when informed, with the specified Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pOrderStatus">Order Status (optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS</returns>
        ''' <remarks>
        ''' Created by: JV + AG 02/10/2013
        ''' </remarks>
        Public Function GetOrdersByTestType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, Optional ByVal pOrderStatus As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksOrders O INNER JOIN twksOrderTests OT ON O.OrderID = OT.OrderID " & vbCrLf & _
                                                " WHERE  OT.TestType = '" & pTestType & "' "
                        If (pOrderStatus <> "") Then
                            cmdText &= " AND O.OrderStatus = '" & pOrderStatus & "' "
                        End If

                        Dim myDataSet As New OrdersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrders)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.GetOrdersByTestType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the identifier of all Orders of the specified Sample Class and having Executions with the informed Execution Status
        ''' (AG - Improve the query, we can query orders (Patient) by status closed - it will be better)
        ''' (I don't do it because we are closing version v2.1.1)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pStatus">Execution Status Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSOrderTestsDS when the identifier of the Orders that fulfill the specified conditions</returns>
        ''' <remarks>
        ''' Created by: JV 01/09/2013
        ''' </remarks>
        Public Function GetOrdersPatientByAnalyzerWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                     ByVal pAnalyzerID As String, ByVal pSampleClass As String, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT wsOT.OrderID " & vbCrLf & _
                                                " FROM   twksWSExecutions wsExec INNER JOIN twksOrderTests wsOT ON wsExec.OrderTestID = wsOT.OrderTestID " & vbCrLf & _
                                                " WHERE  wsExec.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    wsExec.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    wsExec.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    wsExec.ExecutionStatus = '" & pStatus.Trim & "' " & vbCrLf

                        Dim orderTestDS As New WSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(orderTestDS.twksWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = orderTestDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.GetOrderPatientByAnalyzerWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns all the Orders for the specified Work Session, Analyzer, Sample Class and Order Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pStatus">Order Status Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the Orders that fulfill the specified conditions</returns>
        ''' <remarks>
        ''' Created by:  JV 03/10/13 
        ''' </remarks> 
        Public Function GetOrdersPatientByAnalyzerAndWSAllTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                                ByVal pAnalyzerID As String, ByVal pSampleClass As String, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.* FROM twksOrders O INNER JOIN twksOrderTests OT ON O.OrderID = OT.OrderID " & vbCrLf & _
                                                                             " INNER JOIN twksWSOrderTests otWS ON OT.OrderTestID = otWS.OrderTestID " & vbCrLf & _
                                                " WHERE  O.OrderStatus = '" & pStatus.Trim & "' " & vbCrLf & _
                                                " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    O.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    otWS.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        Dim orderDS As New OrdersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(orderDS.twksOrders)
                            End Using
                        End Using

                        resultData.SetDatos = orderDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.GetOrdersPatientByAnalyzerAndWSAllTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the column OrderToExport to pNewValue by the informed filter parameters
        ''' Filter1: by orderTest + Rerun
        ''' Filter2: by LISMEssageID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderID"></param>
        ''' <param name="pNewValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 30/07/2014 - #1887 OrderToExport management
        ''' AG 17/10/2014 BA-2011 change pOrderID parameter from String to List(Of String) due to patients with OFFS tests have more than 1 orderID</remarks>
        Public Function UpdateOrderToExport(ByVal pDBConnection As SqlClient.SqlConnection, pOrderID As List(Of String), pNewValue As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                           " SET    OrderToExport = " & Convert.ToInt32(IIf(pNewValue, 1, 0)) & vbCrLf & _
                                           " WHERE  ( SampleClass   = 'PATIENT' )" & vbCrLf

                    'AG 17/10/2014 BA-2011
                    'If pOrderID <> "" Then
                    '    cmdText &= " AND   OrderID = N'" & pOrderID.Trim.Replace("'", "''") & "' "
                    'End If
                    If pOrderID.Count > 0 Then
                        cmdText &= " AND ( "
                        For item As Integer = 0 To pOrderID.Count - 1
                            cmdText &= " OrderID = N'" & pOrderID(item).Trim.Replace("'", "''") & "' "
                            If item < pOrderID.Count - 1 Then
                                cmdText &= " OR "
                            End If
                        Next
                        cmdText &= " ) "
                    End If
                    'AG 17/10/2014 BA-2011

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.UpdateOrderToExport", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read data of the Order to which the specified LISMessageID belongs 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID">LIS message Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the Order to which the specified LIS message identifier belongs</returns>
        ''' <remarks>
        ''' Created by:  AG 30/07/2014 #1887 OrderToExport management
        ''' </remarks>
        Public Function ReadByLISMessageID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISMessageID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT O.* FROM twksOrders O INNER JOIN twksOrderTests OT ON O.OrderID = OT.OrderID " & vbCrLf & _
                                                " INNER JOIN twksResults R ON OT.OrderTestID = R.OrderTestID " & vbCrLf & _
                                                " WHERE  R.LISMessageID = N'" & pLISMessageID.Trim.Replace("'", "''") & "' "

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadByLISMessageID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Updates the column OrderToPrint to pNewValue by the informed filter parameters
        ''' Filter1: by orderTest + Rerun
        ''' Filter2: by LISMEssageID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderID"></param>
        ''' <param name="pNewValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 30/07/2014 - #1887 OrderToPrint management</remarks>
        Public Function UpdateOrderToPrint(ByVal pDBConnection As SqlClient.SqlConnection, pOrderID As String, pNewValue As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksOrders " & vbCrLf & _
                                           " SET    OrderToPrint = " & Convert.ToInt32(IIf(pNewValue, 1, 0)) & vbCrLf & _
                                           " WHERE  ( SampleClass   = 'PATIENT' )" & vbCrLf

                    If pOrderID <> "" Then
                        cmdText &= " AND   OrderID = N'" & pOrderID.Trim.Replace("'", "''") & "' "
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.UpdateOrderToPrint", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Look for different orderID belonging the same patient/control sample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with all data of the Order to which the specified OrderTest belongs</returns>
        ''' <remarks>
        ''' Created by:  AG 16/10/2014 BA-2011
        ''' </remarks>
        Public Function ReadRelatedOrdersByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT DISTINCT O2.* FROM twksOrders O INNER JOIN twksOrders O2 ON O.OrderID <> O2.OrderID AND (O.PatientID = O2.PatientID OR O.SampleID = O2.SampleID) " & vbCrLf & _
                                  " WHERE O.OrderID = N'" & pOrderID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                  "AND (O.SampleClass = 'PATIENT' ) "

                        Dim resultData As New OrdersDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(dbCmd)
                                da.Fill(resultData.twksOrders)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrdersDAO.ReadRelatedOrdersByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


#End Region

    End Class
End Namespace
