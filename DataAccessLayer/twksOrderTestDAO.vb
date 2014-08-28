Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksOrderTestsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a list of Order Tests to an existing Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pNewOrderTests">List of Order Tests to add</param>
        ''' <returns>Global object containing error information and the updated DataSet</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: VR 10/12/2009 - Included field ReplicatesNumber in insert command (TESTED: OK)
        '''              VR 05/02/2010 - Included field TestProfileID in Insert Command (Tested: OK)
        '''              SA 23/02/2010 - Included fields PreviousOrderTestID, AlternativeOrderTestID and ControlNumber in Insert Command
        '''              SA 26/03/2010 - Included field CreationOrder in Insert Command
        '''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_User; if audit fields are not informed
        '''                              in the parameter DS, use the current logged User and the current date
        '''              TR 06/08/2011 - Do not declare variables inside loops 
        '''              SA 26/03/2013 - When fields LISRequest and/or ExternalQC have NULL as value in the entry DS, insert them as FALSE
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewOrderTests As OrderTestsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                ElseIf (Not IsNothing(pNewOrderTests)) Then
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection

                    Dim i As Integer = 0
                    Dim cmdText As String = ""
                    Dim noError As Boolean = True
                    Dim nextOrderTestID As Integer = 0
                    Do While (i < pNewOrderTests.twksOrderTests.Rows.Count) AndAlso (noError)
                        'SQL Sentence to insert the new Order Test
                        cmdText = " INSERT INTO twksOrderTests(OrderID, TestType, TestID, SampleType, OrderTestStatus, TubeType, AnalyzerID, " & _
                                                             " TestProfileID, ExportDateTime, ReplicatesNumber, PreviousOrderTestID, AlternativeOrderTestID, " & _
                                                             " ControlID, CreationOrder, LISRequest, ExternalQC, TS_User, TS_DateTime, Selected) " & _
                                  " VALUES('" & pNewOrderTests.twksOrderTests(i).OrderID & "', " & _
                                         " '" & pNewOrderTests.twksOrderTests(i).TestType & "', " & _
                                                pNewOrderTests.twksOrderTests(i).TestID & ", " & _
                                         " '" & pNewOrderTests.twksOrderTests(i).SampleType & "', " & _
                                         " '" & pNewOrderTests.twksOrderTests(i).OrderTestStatus & "', "

                        If (pNewOrderTests.twksOrderTests(i).IsTubeTypeNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " '" & pNewOrderTests.twksOrderTests(i).TubeType & "', "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsAnalyzerIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " '" & pNewOrderTests.twksOrderTests(i).AnalyzerID & "', "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsTestProfileIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).TestProfileID & ", "
                        End If

                        'Field ExportDateTime is always Null when creation
                        cmdText = cmdText & " NULL, "

                        If (pNewOrderTests.twksOrderTests(i).IsReplicatesNumberNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).ReplicatesNumber & ", "  'VR - Modified - 18/12/2009
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsPreviousOrderTestIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).PreviousOrderTestID & ", "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsAlternativeOrderTestIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).AlternativeOrderTestID & ", "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsControlIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).ControlID & ", "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsCreationOrderNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= pNewOrderTests.twksOrderTests(i).CreationOrder & ", "
                        End If

                        'SA 26/03/2013 - Fields LISRequest and ExternalQC are set to False when not informed
                        If (pNewOrderTests.twksOrderTests(i).IsLISRequestNull) Then
                            cmdText &= " 0, "
                        Else
                            cmdText &= IIf(pNewOrderTests.twksOrderTests(i).LISRequest, 1, 0).ToString & ", "
                        End If

                        If (pNewOrderTests.twksOrderTests(i).IsExternalQCNull) Then
                            cmdText &= " 0, "
                        Else
                            cmdText &= IIf(pNewOrderTests.twksOrderTests(i).ExternalQC, 1, 0).ToString & ", "
                        End If


                        'Audit fields 
                        If (pNewOrderTests.twksOrderTests(i).IsTS_UserNull) Then
                            Dim myGlobalBase As New GlobalBase
                            cmdText &= " N'" & myGlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
                        Else
                            cmdText &= " N'" & pNewOrderTests.twksOrderTests(i).TS_User.Replace("'", "''") & "', "
                        End If
                        If (pNewOrderTests.twksOrderTests(i).IsTS_DateTimeNull) Then
                            cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                        Else
                            cmdText += " '" & pNewOrderTests.twksOrderTests(i).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        ' XB 27/08/2014 - BT #1868
                        If (pNewOrderTests.twksOrderTests(i).IsSelectedNull) Then
                            cmdText &= " 0) "
                        Else
                            cmdText &= IIf(pNewOrderTests.twksOrderTests(i).Selected, 1, 0).ToString & ") "
                        End If

                        'Execute the SQL Sentence
                        dbCmd.CommandText = cmdText
                        dbCmd.CommandText &= " SELECT SCOPE_IDENTITY()"

                        nextOrderTestID = 0
                        nextOrderTestID = CType(dbCmd.ExecuteScalar(), Integer)

                        If (nextOrderTestID > 0) Then
                            'Get the generated Order Test ID and update the correspondent field in the DataSet
                            pNewOrderTests.twksOrderTests(i).BeginEdit()
                            pNewOrderTests.twksOrderTests(i).OrderTestID = nextOrderTestID
                            pNewOrderTests.twksOrderTests(i).EndEdit()
                        Else
                            'If the OrderTestID was not returned, then it was an uncontrolled error
                            noError = False
                        End If
                        i += 1
                    Loop

                    If (noError) Then
                        dataToReturn.HasError = False
                        dataToReturn.AffectedRecords = i
                        dataToReturn.SetDatos = pNewOrderTests
                    Else
                        dataToReturn.HasError = True
                        dataToReturn.AffectedRecords = 0
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Deletes the informed Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTests WHERE OrderTestID = '" & pOrderTestID.ToString().Trim & "' "

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.DeleteByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all existing Order Tests with status OPEN and belonging to Orders of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 25/02/2010
        ''' Modified by: SA 27/04/2010 - Delete only OPEN Order Tests that are not linked to any Work Session
        '''              SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteOpenOrderTestsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTests " & vbCrLf & _
                                            " WHERE  OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    OrderTestID NOT IN (SELECT OrderTestID FROM twksWSOrderTests) " & vbCrLf & _
                                            " AND    OrderID IN (SELECT OrderID FROM twksOrders WHERE  SampleClass = '" & pSampleClass & "') " & vbCrLf

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.DeleteOpenOrderTestsBySampleClass", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the specified Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderDetailsDS with details of the specified Order Test</returns>
        ''' <remarks>
        ''' Created by:  DL 08/02/2010
        ''' Modified by: SA 11/05/2010 - Name changed from GetOrderTestsByOrdertestID to Read
        '''              AG 03/12/2010 - Get also the OrderStatus using INNER JOIN with twksOrders table
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 18/06/2012 - Changed the query to get also the SampleClass of the Order 
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.*, O.SampleClass, O.OrderStatus " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  OT.OrderTestID = " & pOrderTestID

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myOrderTestsDS
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = False
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update information of the specified Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsDS">Typed DataSet OrderTestsDS containing the list of Order Tests to update</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 17/03/2010 - Added updation of new field ControlNumber
        '''              SA 26/03/2010 - Added updation of new field CreationOrder
        '''              SA 18/05/2010 - Change the SQL Sentence: field ReplicatesNumber is Null for Calculated Tests
        '''              SA 28/10/2010 - Added N preffix for multilanguage of fields TS_User; if audit fields are not informed
        '''                              in the parameter DS, use the current logged User and the current date. Remove error
        '''                              raising based in number of affected records
        '''              SA 21/02/2011 - Changed implementation to send several Updates as a script to improve the process performance
        '''              SA 31/01/2012 - Use the Using clause to manage the SQL Commands defined in the function
        '''              SA 26/03/2013 - When fields LISRequest and ExternalQC are informed in the entry DS, update the corresponding value. 
        '''                              When value is NULL, do not change the current value.
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsDS As OrderTestsDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Const maxUpdates As Integer = 500
                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests
                        cmdText.Append(" UPDATE twksOrderTests SET ")

                        cmdText.Append(" ReplicatesNumber = ")
                        If (orderTestRow.IsReplicatesNumberNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", orderTestRow.ReplicatesNumber)
                        End If

                        cmdText.Append(", TubeType = ")
                        If (orderTestRow.IsTubeTypeNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", orderTestRow.TubeType)
                        End If

                        'SA 26/03/2013 - Update field LISRequest only if it is informed in the entry DS
                        If (Not orderTestRow.IsLISRequestNull) Then
                            cmdText.Append(", LISRequest = ")
                            cmdText.AppendFormat("{0}", IIf(orderTestRow.LISRequest, 1, 0).ToString)
                        End If

                        'SA 26/03/2013 - Update field ExternalQC only if it is informed in the entry DS
                        If (Not orderTestRow.IsExternalQCNull) Then
                            cmdText.Append(", ExternalQC = ")
                            cmdText.AppendFormat("{0}", IIf(orderTestRow.ExternalQC, 1, 0).ToString)
                        End If

                        cmdText.Append(", AnalyzerID = ")
                        If (orderTestRow.IsAnalyzerIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", orderTestRow.AnalyzerID)
                        End If

                        cmdText.Append(", TestProfileID = ")
                        If (orderTestRow.IsTestProfileIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", orderTestRow.TestProfileID)
                        End If

                        cmdText.Append(", ExportDateTime = ")
                        If (orderTestRow.IsExportDateTimeNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", orderTestRow.ExportDateTime)
                        End If

                        cmdText.Append(", PreviousOrderTestID = ")
                        If (orderTestRow.IsPreviousOrderTestIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", orderTestRow.PreviousOrderTestID)
                        End If

                        cmdText.Append(", AlternativeOrderTestID = ")
                        If (orderTestRow.IsAlternativeOrderTestIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", orderTestRow.AlternativeOrderTestID)
                        End If

                        cmdText.Append(", ControlID = ")
                        If (orderTestRow.IsControlIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", orderTestRow.ControlID)
                        End If

                        cmdText.Append(", CreationOrder = ")
                        If (orderTestRow.IsCreationOrderNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", orderTestRow.CreationOrder)
                        End If

                        cmdText.Append(", TS_User = ")
                        If (orderTestRow.IsTS_UserNull) Then
                            Dim myGlobalBase As New GlobalBase
                            cmdText.AppendFormat("N'{0}'", myGlobalBase.GetSessionInfo().UserName().Replace("'", "''"))
                        Else
                            cmdText.AppendFormat("N'{0}'", orderTestRow.TS_User.Replace("'", "''"))
                        End If

                        cmdText.Append(", TS_DateTime = ")
                        If (orderTestRow.IsTS_DateTimeNull) Then
                            cmdText.AppendFormat("'{0}'", Now.ToString("yyyyMMdd HH:mm:ss"))
                        Else
                            cmdText.AppendFormat("'{0}'", orderTestRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss"))
                        End If

                        ' XB 27/08/2014 - BT #1868
                        If (Not orderTestRow.IsSelectedNull) Then
                            cmdText.Append(", Selected = ")
                            cmdText.AppendFormat("{0}", IIf(orderTestRow.Selected, 1, 0).ToString)
                        End If

                        cmdText.Append(" WHERE  OrderTestID = ")
                        cmdText.AppendFormat("{0}", orderTestRow.OrderTestID)
                        cmdText.Append(vbCrLf)

                        i += 1
                        If (i = maxUpdates) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not myGlobalDataTO.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Updates...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update field AlternativeOrderTestID for the specified OrderTestID - This function is used only for OTs with SampleClass 
        ''' CALIB when for the Test/SampleType the type of Calibrator needed is an Alternative one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pAlternativeOT">OrderTestID for the Test/SampleType used as Alternative Calibrator</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 26/06/2012
        ''' </remarks>
        Public Function UpdateAlternativeOTByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                         ByVal pAlternativeOT As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET AlternativeOrderTestID = " & pAlternativeOT.ToString & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateAlternativeOTByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update the LISStatus field by OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pLISStatusValue">Value to assign to LISStatus field</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/03/2013
        ''' </remarks>
        Public Function UpdateLISRequestByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                      ByVal pLISStatusValue As Boolean) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET LISRequest = " & IIf(pLISStatusValue, 1, 0).ToString & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateLISRequestByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Change the Status of the Order Tests included in the informed Work Session from the indicated current status to the 
        ''' specified new status. For instance:
        '''    ** When a Work Session is created, the status of all Order Tests included in it should
        '''       change from OPEN to PENDING
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pNewStatus">New Order Test Status</param>
        ''' <param name="pPreviousStatus">Current Status of the Order Tests to update</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLoggedUser">Current logged User</param>
        ''' <returns>Global object containing sucess/error information</returns>
        ''' <remarks>
        ''' Created  by: SA 
        ''' Modified by: SA 26/04/2010 - The Status will be changed only for Order Tests that have been selected 
        '''                              to be positioned in the Work Session 
        '''              SA 28/10/2010 - Added N preffix for multilanguage of fields TS_User
        '''              SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewStatus As String, ByVal pPreviousStatus As String, _
                                     ByVal pWorkSessionID As String, ByVal pLoggedUser As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests " & vbCrLf & _
                                            " SET    OrderTestStatus = '" & pNewStatus.Trim & "', " & vbCrLf & _
                                                   " TS_User = N'" & pLoggedUser.Trim & "', " & vbCrLf & _
                                                   " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                   " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                   " AND    OpenOTFlag = 0) " & vbCrLf & _
                                            " AND    OrderTestStatus = '" & pPreviousStatus.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateStatus", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update the OrderTestStatus field by OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pStatusValue">New Status to assign to the informed OrderTestID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateStatusByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                  ByVal pStatusValue As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET OrderTestStatus = '" & pStatusValue.Trim & "' " & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateStatusByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update the TubeType of all Order Tests with SampleClass = CALIB that use the informed Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Identifier of the active WorkSession</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateTubeTypeByCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pCalibratorID As Integer, _
                                                   ByVal pNewTubeType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET TubeType = '" & pNewTubeType & "' " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT WSOT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                                                " INNER JOIN tparTestCalibrators TC ON OT.TestID = TC.TestID AND OT.SampleType = TC.SampleType " & vbCrLf & _
                                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                   " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                                   " AND    O.SampleClass      = 'CALIB' " & vbCrLf & _
                                                                   " AND    TC.CalibratorID    = " & pCalibratorID.ToString & ") "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateTubeTypeByCalibrator", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the TubeType of all Order Tests with SampleClass = CTRL that use the informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Identifier of the active WorkSession</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateTubeTypeByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pControlID As Integer, _
                                                ByVal pNewTubeType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET TubeType = '" & pNewTubeType & "' " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT WSOT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                   " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                                   " AND    O.SampleClass      = 'CTRL' " & vbCrLf & _
                                                                   " AND    OT.ControlID       = " & pControlID.ToString & ") "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateTubeTypeByControl", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the TubeType of the specified SampleType for all Order Tests with SampleClass = PATIENT and belonging to
        ''' the informed PatientID or SampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Identifier of the active WorkSession</param>
        ''' <param name="pPatientID">PatientID or SampleID</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateTubeTypeByPatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pPatientID As String, _
                                                ByVal pSampleType As String, ByVal pNewTubeType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET TubeType = '" & pNewTubeType & "' " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT WSOT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                   " AND    O.SampleClass      = 'PATIENT' " & vbCrLf & _
                                                                   " AND   (O.PatientID IS NOT NULL AND O.PatientID = N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf & _
                                                                   " OR    (O.OrderID   IS NOT NULL AND O.OrderID   = N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf & _
                                                                   " OR    (O.SampleID  IS NOT NULL AND O.SampleID  = N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf & _
                                                                   " AND    OT.SampleType = '" & pSampleType & "') "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateTubeTypeByPatient", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the TubeType all Order Tests with SampleClass = BLANK that correspond to Standard Tests using the specified
        ''' Special Solution to execute the Blank
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Identifier of the active WorkSession</param>
        ''' <param name="pSpecSolution">Code of the Special Solution</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateTubeTypeBySpecialSolution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pSpecSolution As String, _
                                                        ByVal pNewTubeType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests SET TubeType = '" & pNewTubeType & "' " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT WSOT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                                                " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
                                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                   " AND    O.SampleClass      = 'BLANK' " & vbCrLf & _
                                                                   " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                                   " AND    T.BlankMode        = '" & pSpecSolution & "') "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateTubeTypeBySpecialSolution", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed WorkSession Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerIDNew">current connected Analyzer Identifier</param>
        ''' <param name="pAnalyzerIDOld">old connected Analyzer Identifier</param>
        ''' <param name="pOrderTestStatus">status of which is NOT change</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerIDNew As String, ByVal pAnalyzerIDOld As String, _
                                           ByVal pOrderTestStatus As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksOrderTests " & vbCrLf & _
                                            " SET    AnalyzerID = '" & pAnalyzerIDNew.Trim & "' " & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerIDOld.Trim & "' " & vbCrLf & _
                                            " AND    OrderTestStatus <> '" & pOrderTestStatus.Trim & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Count the number of Order Tests for the infomed Standard TestID  and, optionally the informed Sample Types:
        ''' ** For SampleClass=CALIB, counts the number of Patient and Control Order Tests for the specified TestID/SampleType
        ''' ** For SampleClass=BLANK, counts the number of Patient, Control and Calibrator Order Tests for the specified TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Sample Class Code: BLANK or CALIB</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Code of the Sample Type. Optional parameter, informed only when the informed SampleClass is CALIB</param>
        ''' <param name="pOrderTestID">Order Test Identifier. Optional parameter, informed only when the informed SampleClass is CALIB</param>
        ''' <returns>GlobalDataTO containing an integer value with:
        '''          ** For SampleClass=CALIB, the number of Patient and Control Order Tests for the specified TestID/SampleType
        '''          ** For SampleClass=BLANK, the number of Patient, Control and Calibrator Order Tests for the specified TestID</returns>
        ''' <remarks>
        ''' Created by:  SA 15/05/2013
        ''' </remarks>
        Public Function CountBlankOrCalibDependencies(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String, ByVal pTestID As Integer, _
                                                      Optional ByVal pSampleType As String = "", Optional ByVal pOrderTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'The list of Sample Classes to consult depends on the received SampleClass
                        Dim mySampleClassList As String = "'PATIENT', 'CTRL'"
                        If (pSampleClass = "BLANK") Then mySampleClassList &= ", 'CALIB'"

                        Dim cmdText As String = " SELECT COUNT(*) AS NumOTs FROM twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass IN (" & mySampleClassList.Trim & ") " & vbCrLf & _
                                                " AND    OT.TestType = 'STD' " & vbCrLf & _
                                                " AND    OT.TestID   = " & pTestID.ToString & vbCrLf




                        'For Calibrators, verify it is not used as Alternative Order Test for other SampleType
                        If (pSampleClass = "CALIB") Then
                            'Add the SampleType filter if the optional parameter is informed
                            If (pSampleType.Trim <> String.Empty AndAlso pOrderTestID > 0) Then
                                cmdText &= " AND (OT.SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                    "OR EXISTS (SELECT OrderTestID FROM twksOrderTests WHERE AlternativeOrderTestID = " & pOrderTestID & ")) " & vbCrLf
                            End If
                        End If

                        Dim myNumOTs As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    myNumOTs = CInt(dbDataReader.Item("NumOTs"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = myNumOTs
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountBlankOrCalibDependencies", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count all OrderTests (or only the Calculated/OffSystem ones) in current WorkSession that have the specified OrderTestStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pStatus">OrderTest Status to search</param>
        ''' <param name="pOnlyTestsWithNoExecutions">When TRUE, function count only CALC and OFFS Order Tests having the specified Status
        '''                                          When FALSE, function count all Order Tests having the specified Status</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests with the specified Status</returns>
        ''' <remarks>
        ''' Created by: AG 03/03/2014 - BT #1524 (integrated in source control version 11/03/2014)
        ''' </remarks>
        Public Function CountByOrderTestStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                               ByVal pStatus As String, ByVal pOnlyTestsWithNoExecutions As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS ResultCount " & vbCrLf & _
                                                " FROM   twksOrderTests OT " & vbCrLf & _
                                                " WHERE  OT.AnalyzerID    = N'" & pAnalyzerID.Trim & "' " & vbCrLf

                        If (Not pOnlyTestsWithNoExecutions AndAlso pStatus.Length > 0) Then
                            'All test types by orderTEstStatus
                            cmdText &= " AND    OT.OrderTestStatus = '" & pStatus & "' "

                        ElseIf (pStatus.Length > 0) Then  'Only tests with no executions (CALC / OFFS)
                            'CALC + OFFS test by ordertest status
                            If (pStatus <> "PENDING") Then
                                cmdText &= " AND ( OT.TestType = 'CALC' OR OT.TestType = 'OFFS' ) AND    OT.OrderTestStatus = '" & pStatus & "' "
                            Else
                                cmdText &= " AND ((OT.TestType = 'CALC' AND OT.OrderTestStatus = '" & pStatus & "') " & vbCrLf
                                cmdText &= " OR (OT.TestType = 'OFFS' AND OT.OrderTestStatus = 'OPEN')) " & vbCrLf
                            End If
                        Else
                            'CALC + OFFS (all) status
                            cmdText &= " AND ( OT.TestType = 'CALC' OR OT.TestType = 'OFFS' ) "
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountByOrderTestStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests included in the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' Modified by: SA 31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTests WHERE OrderID = '" & pOrderID.Trim & "' "

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.DeleteByOrderID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Order, delete all Order Tests with Status OPEN that are not included in the 
        ''' list of informed Order Test IDs 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the Order</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 31/01/2012 - Changed the function template
        '''              SA 05/02/2014 - BT #1491 ==> Changed the SQL sentence to exclude from the DELETE the Order Tests requested by LIS
        ''' </remarks>
        Public Function DeleteNotInList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pOrderTestsList As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTests " & vbCrLf & _
                                            " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf & _
                                            " AND    OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    OrderTestID NOT IN (" & pOrderTestsList.Trim & ") " & vbCrLf & _
                                            " AND   (LISRequest IS NULL OR LISRequest = 0) " & vbCrLf

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.DeleteNotInList", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search if for the specified TestID/SampleType the required Experimental Calibrator is linked to a different SampleType for
        ''' the same Test.  This will be true only when the CalibratorType for the Test/SampleType is defined as Alternative
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a  string value with the SampleType for which the Experimental Calibrator for the informed 
        '''          Test is defined (if any)</returns>
        ''' <remarks>
        ''' Created by:  SA 02/06/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetAlternativeSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                 ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SampleType FROM twksOrderTests " & vbCrLf & _
                                                " WHERE  OrderTestID = (SELECT AlternativeOrderTestID FROM vwksWSOrderTests " & vbCrLf & _
                                                                      " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                      " AND    AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                                      " AND    SampleClass   = 'CALIB' " & vbCrLf & _
                                                                      " AND    TestType      = 'STD' " & vbCrLf & _
                                                                      " AND    TestID        = " & pTestID & vbCrLf & _
                                                                      " AND    SampleType    = '" & pSampleType & "' " & vbCrLf & _
                                                                      " AND    AlternativeOrderTestID IS NOT NULL) " & vbCrLf

                        resultData.SetDatos = String.Empty
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = dbDataReader.Item("SampleType").ToString
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestDAO.GetAlternativeSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Blanks and Calibrators that have been not included in a WorkSession (those
        ''' with Status OPEN) and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReturnOpenAsSelected">Optional parameter to indicate if OPEN Blanks and Calibrators have to be returned as SELECTED
        '''                                     (when its value is TRUE) or UNSELECTED (when its value is FALSE)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Blanks or Calibrators
        '''          obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 17/03/2010 - For Blank and Calibrator Order Tests included in a WorkSession, return always "INPROCESS" as
        '''                              OrderTestStatus instead of the current value of field OrderTestStatus in twksOrderTests
        '''                              (needed to show Blank and Calibrator Order Tests properly sorted in the screen of WS Preparation);
        '''                              Queries changed to ANSI.
        '''              SA 26/03/2010 - Get also field CreationOrder from table twksOrderTests. Sort results by SampleClass and CreationOrder    
        '''              SA 26/04/2010 - Query changed to get Open and InProcess Blanks and Calibrators linked to the informed WorkSession;
        '''                              parameter pWorkSessionID changed from optional to fixed
        '''              SA 01/09/2010 - Query changed to allow get value of twksWSOrderTests.ToSendFlag. Table twksWSOrderTests is now included
        '''                              in the JOIN instead of be part of a subquery in the WHERE clause         
        '''              SA 01/09/2011 - Changed the function template 
        '''              SA 04/04/2013 - Added new optional parameter to indicate if OPEN Blanks and Calibrators have to be returned as SELECTED.
        '''                              Optional parameter with TRUE as default value. Changed the subQuery for OPEN OrderTests to return them
        '''                              selected or not according value of this new parameter  
        '''              XB 28/08/2014 - Get the new field Selected from twksOrderTests table - BT #1868
        ''' </remarks>
        Public Function GetBlankCalibOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                Optional ByVal pReturnOpenAsSelected As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        ' XB 28/08/2014 - this value is not used from this moment - BT #1868
                        'Dim selectedValue As String = IIf(pReturnOpenAsSelected, 1, 0).ToString

                        'Get Open Blanks and Calibrators linked to the informed WorkSession
                        Dim cmdText As String = " SELECT OT.Selected AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, " & vbCrLf & _
                                                       " OT.TubeType,  OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates, " & vbCrLf & _
                                                       " 1 AS NewCheck,  OT.PreviousOrderTestID, OT.OrderTestStatus AS OTStatus, " & vbCrLf & _
                                                       " OT.CreationOrder, WSOT.ToSendFlag, T.BlankMode " & vbCrLf & _
                                                 " FROM  twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                         " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
                                                 " WHERE OT.TestType = 'STD' " & vbCrLf & _
                                                 " AND   WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                 " AND   WSOT.OpenOTFlag = 1 " & vbCrLf & _
                                                 " AND   OT.AlternativeOrderTestID IS NULL " & vbCrLf & _
                                                 " AND   O.SampleClass IN ('BLANK', 'CALIB') " & vbCrLf

                        '... and get In Process Blanks and Calibrators linked to the informed WorkSession
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT OT.Selected AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, OT.TubeType, " & vbCrLf & _
                                          " OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates, 1 AS NewCheck, " & vbCrLf & _
                                          " OT.PreviousOrderTestID, 'INPROCESS' AS OTStatus, OT.CreationOrder, WSOT.ToSendFlag, T.BlankMode " & vbCrLf & _
                                   " FROM   twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                            " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                            " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
                                   " WHERE  OT.TestType = 'STD' " & vbCrLf & _
                                   " AND    WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                   " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
                                   " AND    OT.AlternativeOrderTestID IS NULL " & vbCrLf & _
                                   " AND    O.SampleClass IN ('BLANK', 'CALIB') " & vbCrLf

                        '... sort all obtained Blanks and Calibrators by SampleClass and CreationOrder
                        cmdText &= " ORDER BY O.SampleClass, OT.CreationOrder " & vbCrLf

                        Dim blankCalibsDS As New WorkSessionResultDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(blankCalibsDS.BlankCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = blankCalibsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetBlankCalibOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different Blank Modes (different of Only Reagent) for a group of Order Tests included in a WorkSession, 
        ''' having ToSendFlag=True and belonging to Orders with SampleClass BLANK. 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Transaction</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Blank Modes that are required in a 
        '''          WorkSession for the informed list of Order Tests</returns>
        ''' <remarks>
        ''' Created by:  RH 08/06/2011
        ''' Modified by: SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetBlanksData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                      Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT T.BlankMode As ControlName,  OT.TubeType " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN twksOrderTests OT ON T.TestID = OT.TestID " & vbCrLf & _
                                                                   " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'BLANK' " & vbCrLf & _
                                                " AND    OT.TestType   = 'STD' " & vbCrLf & _
                                                " AND    T.BlankMode  <> 'REAGENT' " & vbCrLf

                        If (Not String.IsNullOrEmpty(pWorkSessionID)) Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & _
                                                              " AND    OpenOTFlag    = 0 " & _
                                                              " AND    ToSendFlag    = 1) "
                        Else
                            cmdText &= " AND  OT.OrderTestID IN (" & pOrderTestsList & ") "
                        End If

                        Dim myTestControlsDS As New TestControlsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsDS.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetBlanksData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Experimental Calibrators required for a group of Order Tests included in a WorkSession having 
        ''' ToSendFlag=True and belonging to Orders with SampleClass Calibrator. If several Test/SampleType uses the same 
        ''' Calibrator it will be returned only once
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS with the list of Experimental 
        '''          Calibrators that are required in a WorkSession for the informed list of Order Tests. For each 
        '''          Calibrator, the number of Points is also returned</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changes to update function to the new template for DB Connection and 
        '''                              data to return
        '''              SA 17/03/2010 - Return also field TubeType from twksOrderTests Table
        '''              SA 03/05/2010 - Added optional parameter WorkSessionID (needed to improve performance). Query changed to 
        '''                              ANSI format. When WorkSessionID is informed, filter also by OrderTestStatus = OPEN
        '''              SA 27/08/2010 - Added filter to subquery to get only the OrderTestID that has to be positioned
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetCalibrationData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                           Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT C.CalibratorID, C.NumberOfCalibrators, OT.TubeType " & vbCrLf & _
                                                " FROM   tparCalibrators C INNER JOIN tparTestCalibrators TC ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                                         " INNER JOIN twksOrderTests OT ON TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType " & vbCrLf & _
                                                                         " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'CALIB' " & vbCrLf & _
                                                " AND    OT.TestType   = 'STD' " & vbCrLf

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND    OpenOTFlag    = 0 " & vbCrLf & _
                                                              " AND    ToSendFlag    = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        Dim myTestSampleCalibratorDS As New TestSampleCalibratorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestSampleCalibratorDS.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = myTestSampleCalibratorDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetCalibrationData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Controls that have been not included in a WorkSession (those with Status OPEN) 
        ''' and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Controls obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 03/03/2010
        ''' Modified by: DL 16/03/2010 - Get the Control Number from table twksOrderTests and also use this field in the join
        '''              SA 17/03/2010 - For Control Order Tests included in a WorkSession, return always "INPROCESS" as
        '''                              OrderTestStatus instead of the current value of field OrderTestStatus in twksOrderTests
        '''                              (needed to show Control Order Tests properly sorted in the screen of WS Preparation);
        '''                              Queries changed to ANSI
        '''              SA 26/03/2010 - Get also field CreationOrder from table twksOrderTests. Sort results by CreationOrder
        '''              SA 26/04/2010 - Query changed to get Open and InProcess Controls linked to the informed WorkSession;
        '''                              parameter pWorkSessionID changed from optional to fixed  
        '''              SA 01/09/2011 - Changed the function template      
        '''              SA 18/06/2012 - Changed the query to get the Test Name from the proper table depending on the TestType: for 
        '''                              Standard Tests, field TestName in tparTests, but for ISE Tests, field Name in tparISETests 
        '''              TR 12/03/2013 - Changed the query to get also field LISRequest from twksOrderTests table
        '''              XB 28/08/2014 - Get the new field Selected from twksOrderTests table - BT #1868
        ''' </remarks>
        Public Function GetControlOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get Open Controls linked to the informed WorkSession
                        Dim cmdText As String = " SELECT OT.Selected AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, OT.TubeType, OT.TestType, OT.TestID, OT.LISRequest, " & vbCrLf & _
                                                      " (CASE WHEN OT.TestType = 'STD' THEN (SELECT TestName FROM tparTests WHERE TestID = OT.TestID) " & vbCrLf & _
                                                            " WHEN OT.TestType = 'ISE' THEN (SELECT Name FROM tparISETests WHERE ISETestID = OT.TestID) END) AS TestName, " & vbCrLf & _
                                                       " OT.SampleType, OT.ReplicatesNumber AS NumReplicates, OT.OrderTestStatus AS OTStatus, C.ControlID, " & vbCrLf & _
                                                       " C.ControlName, C.LotNumber, C.ExpirationDate, OT.CreationOrder " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN tparControls C ON C.ControlID = OT.ControlID " & vbCrLf & _
                                                " WHERE OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                          " WHERE WorkSessionID = '" & pWorkSessionID & "' AND OpenOTFlag = 1) " & vbCrLf & _
                                                " AND O.SampleClass = 'CTRL' " & vbCrLf
                        '... and get In Process Controls linked to the informed WorkSession
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT OT.Selected AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, OT.TubeType, OT.TestType, OT.TestID, OT.LISRequest, " & vbCrLf & _
                                         " (CASE WHEN OT.TestType = 'STD' THEN (SELECT TestName FROM tparTests WHERE TestID = OT.TestID) " & vbCrLf & _
                                               " WHEN OT.TestType = 'ISE' THEN (SELECT Name FROM tparISETests WHERE ISETestID = OT.TestID) END) AS TestName, " & vbCrLf & _
                                          " OT.SampleType, OT.ReplicatesNumber AS NumReplicates, 'INPROCESS' AS OTStatus, C.ControlID, " & vbCrLf & _
                                          " C.ControlName, C.LotNumber, C.ExpirationDate, OT.CreationOrder " & vbCrLf & _
                                   " FROM twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                          " INNER JOIN tparControls C ON C.ControlID = OT.ControlID " & vbCrLf & _
                                   " WHERE OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' AND   OpenOTFlag = 0) " & vbCrLf & _
                                   " AND   O.SampleClass = 'CTRL'" & vbCrLf & _
                                   " ORDER BY OT.CreationOrder " & vbCrLf

                        Dim controlsDS As New WorkSessionResultDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(controlsDS.Controls)
                            End Using
                        End Using

                        resultData.SetDatos = controlsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetControlOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Controls required for a group of Order Tests included in a WorkSession having ToSendFlag=True 
        ''' and belonging to Orders with SampleClass Control. If several Test/SampleType uses the same Control, it will be 
        ''' returned only once
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Transaction</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Controls that are required in a 
        '''          WorkSession for the informed list of Order Tests</returns>
        ''' <remarks>
        ''' Modified by: VR 16/12/2009 - Changed the Return Type TestControlsDS to GlobalDataTO
        '''              SA 03/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 17/03/2010 - Changes to filter data using also the ControlNumber
        '''              SA 17/03/2010 - Return also field TubeType from twksOrderTests Table
        '''              SA 03/05/2010 - Added optional parameter WorkSessionID (needed to improve performance). Query changed to 
        '''                              ANSI format. When WorkSessionID is informed, filter also by OrderTestStatus = OPEN
        '''              SA 27/08/2010 - Added filter to subquery to get only the OrderTestID that have to be positioned
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        '''              SA 18/06/2012 - Changed the query: filter by Standard Tests has been replaced by including TestType field 
        '''                              in the InnerJoin between tparTestControls and twksOrderTests
        ''' </remarks>
        Public Function GetControlsData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                        Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT TQCC.ControlID, OT.TubeType " & vbCrLf & _
                                                " FROM   tparTestControls TQCC INNER JOIN twksOrderTests OT ON TQCC.TestType   = OT.TestType " & vbCrLf & _
                                                                                                         " AND TQCC.TestID     = OT.TestID " & vbCrLf & _
                                                                                                         " AND TQCC.SampleType = OT.SampleType " & vbCrLf & _
                                                                                                         " AND TQCC.ControlID  = OT.ControlID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'CTRL' " & vbCrLf

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND    OpenOTFlag    = 0 " & vbCrLf & _
                                                              " AND    ToSendFlag    = 1) " & vbCrLf
                        Else
                            cmdText &= " AND  OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        Dim myTestControlsDS As New TestControlsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsDS.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetControlData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified OrderTestID, get the identifier of its related required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the related required Element</returns>
        ''' <remarks>
        ''' Created by:  SA 31/01/2012
        ''' </remarks>
        Public Function GetElementByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the required Element depending on if the OrderTest corresponds to a Calibrator, Control or Patient Sample or if it corresponds to a Blank 
                        Dim cmdText As String = " SELECT RE.ElementID, RE.ElementFinished " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID AND O.SampleClass <> 'BLANK' " & vbCrLf & _
                                                                             " INNER JOIN twksWSRequiredElemByOrderTest REOT ON WSOT.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksWSRequiredElements RE ON REOT.ElementID = RE.ElementID AND RE.TubeContent = O.SampleClass " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.OrderTestID   = " & pOrderTestID & vbCrLf & _
                                                " AND    OT.AnalyzerID      = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT RE.ElementID, RE.ElementFinished " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID AND O.SampleClass = 'BLANK' " & vbCrLf & _
                                                                             " INNER JOIN twksWSRequiredElemByOrderTest REOT ON WSOT.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksWSRequiredElements RE ON REOT.ElementID = RE.ElementID AND RE.TubeContent = 'TUBE_SPEC_SOL' " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.OrderTestID   = " & pOrderTestID & vbCrLf & _
                                                " AND    OT.AnalyzerID      = '" & pAnalyzerID & "' " & vbCrLf

                        Dim myReqElementDS As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReqElementDS.twksWSRequiredElements)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myReqElementDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetElementByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all not CLOSED Order Tests that need the specified required Element for their execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Identifier of a WS Required Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of not CLOSED Order Tests that need the specified required 
        '''          Element for their execution</returns>
        ''' <remarks>
        ''' Created by: TR 21/11/2103 - BT #1388
        ''' </remarks>
        Public Function GetNotClosedOrderTestByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksOrderTests " & vbCrLf & _
                                                " WHERE  OrderTestID IN (SELECT OrderTestID from twksWSRequiredElemByOrderTest " & vbCrLf & _
                                                                       " WHERE ElementID = " & pElementID.ToString() & ") " & vbCrLf & _
                                                " AND    OrderTestStatus <> 'CLOSED' "

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestDAO.GetNotClosedOrderTestByElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of PatientID/SampleID in the active WS and having at least an OrderTest with an accepted Result. This function is used for Reports
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with the list of PatientID/SampleID with at least an accepted Result in the active WS</returns>
        ''' <remarks>
        ''' Created by:  JV 31/10/2013 - BT #1226
        ''' Modified by: TR 28/11/2013 - BT #1409 - Changed the query to get the PatientID/SampleID in the same column (the PatientID Column).
        ''' </remarks>
        Public Function GetOrdersOKByUser(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT (CASE WHEN O.PatientID IS NULL THEN O.SampleID ELSE O.PatientID END) AS PatientID " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksResults R ON OT.OrderTestID = R.OrderTestID " & vbCrLf & _
                                                                         " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  R.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    R.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    R.AcceptedResultFlag = 1 "

                        Dim myOrdersDS As New OrdersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrdersDS.twksOrders)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myOrdersDS
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrdersOKByUser", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Order Tests included in the specified Order and having the specified Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestStatus">Status of the OrderTests to get</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of Order Tests included in the specified Order
        '''          and having the specified Status</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2010
        ''' Modified by: AG 22/05/2012 - Added parameter for the Status of the OrderTests 
        '''              SA 18/06/2012 - Changed the query: inner joins are not needed
        ''' </remarks>
        Public Function GetOrderTestByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pOrderTestStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.* " & vbCrLf & _
                                                " FROM   twksOrderTests OT " & vbCrLf & _
                                                " WHERE  OT.OrderID          = '" & pOrderID.Trim & "' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus = '" & pOrderTestStatus.Trim & "' " & vbCrLf

                        Dim mytwksOrderTestDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytwksOrderTestDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mytwksOrderTestDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestByOrderIDAndTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if for an specific Calibrator Order, there is an Order Test for the informed Test and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pTestType">Code of the Test Type</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Code of the Sample Type</param>
        ''' <returns>GlobalDataTO containing an integer value with the identifier of the Order Test for the
        '''          Test and requested Sample Type; if the Order Test does not exist then return Null</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2010
        ''' Modified by: SA 31/01/2012 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetOrderTestByTestAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, _
                                                        ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String) _
                                                        As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID FROM twksOrderTests " & vbCrLf & _
                                                " WHERE  OrderID    = '" & pOrderID.Trim & "' " & vbCrLf & _
                                                " AND    TestType   = 'STD' " & vbCrLf & _
                                                " AND    TestID     = " & pTestID.ToString.Trim & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType.Trim & "' "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestByTestAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For an specific OrderTest requested in a WorkSession, corresponding to a Standard Test and with status Closed, get all information that is needed
        ''' to update data of the Test/SampleType in history tables of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the information of the informed OrderTest that is needed
        '''          to update data of the Test/SampleType in history tables of QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 20/07/2011 - Changed the SQL to get data only if the current status of the Order Test is CLOSED; changed the template
        ''' </remarks>
        Public Function GetOrderTestInfoByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.TestID, OT.SampleType, OT.ControlID, C.ControlName, C.LotNumber, T.TestName, " & vbCrLf & _
                                                       " T.ShortName AS TestShortName, T.PreloadedTest, T.MeasureUnit,T.DecimalsAllowed, " & vbCrLf & _
                                                       " TS.RejectionCriteria, TS.CalculationMode, TS.NumberOfSeries " & vbCrLf & _
                                                " FROM  twksOrderTests OT INNER JOIN tparControls C ON OT.ControlID = C.ControlID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
                                                                        " INNER JOIN tparTestSamples TS ON OT.TestID = TS.TestID AND OT.SampleType = TS.SampleType " & vbCrLf & _
                                                " WHERE  OT.OrderTestID     = " & pOrderTestID.ToString() & vbCrLf & _
                                                " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus = 'CLOSED' "

                        Dim mytwksOrderTestDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytwksOrderTestDS.twksOrderTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = mytwksOrderTestDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestInfoByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if in the active WorkSession there are requested Order Tests (for whatever SampleClass) for the informed TestType and
        ''' TestID. Closed Blanks and Calibrators that remain in table twksOrderTests are automatically excluded from the searching by 
        ''' adding the INNER JOIN with table twksWSOrderTests  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with all the Order Tests for the informed TestType and TestID in
        '''          the active Work Session</returns>
        ''' <remarks>
        ''' Created by:  AG 08/05/2013
        ''' Modified by: SA 09/05/2013 - Added parameters to inform TestType and TestID and filter the query also by these values 
        ''' </remarks>
        Public Function GetOrderTestsByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pTestType As String, _
                                                   ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.OrderTestID " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OT.TestType        = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    OT.TestID          = " & pTestID

                        Dim myDataSet As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTests)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestsByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Patients/Controls that have been not performed into the current WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the LIS Order Tests not processed</returns>
        ''' <remarks>
        ''' Created by:  XB 22/04/2013
        ''' </remarks>
        Public Function GetOrderTestsForLISReset(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'PATIENTS
                        'Get information for STD Tests
                        Dim cmdText As String = " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag, " & vbCrLf & _
                                                "       (CASE WHEN O.PatientID IS NOT NULL THEN 'DB' ELSE 'MAN' END) AS SampleIDType, " & vbCrLf & _
                                                "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, OT.CreationOrder, " & vbCrLf & _
                                                "       T.TestName, NULL AS CalcTestFormula, OT.ControlID, " & vbCrLf & _
                                                "       OT.OrderTestID, OTL.AwosID , OTL.ESOrderID , OTL.ESPatientID , OTL.LISOrderID , OTL.LISPatientID , OTL.RerunNumber , OTL.SpecimenID , " & vbCrLf & _
                                                "       O.OrderID, OT.LISRequest, OT.OrderTestStatus AS OTStatus, OT.ExternalQC " & vbCrLf & _
                                                " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                                                "                           INNER JOIN tparTests AS T ON OT.TestID = T.TestID  " & vbCrLf & _
                                                "                           LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID   " & vbCrLf & _
                                                " WHERE OT.TestType = 'STD'  " & vbCrLf & _
                                                " AND   OT.LISRequest = 1 " & vbCrLf & _
                                                " AND   O.SampleClass IN ('PATIENT') " & vbCrLf & _
                                                " AND   ((OT.OrderTestStatus <> 'CLOSED' AND OT.OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions WHERE LockedByLIS = 1)) " & vbCrLf & _
                                                " OR " & vbCrLf & _
                                                " (OrderTestStatus = 'CLOSED' AND OT.OrderTestID IN (SELECT OCT.OrderTestID  " & vbCrLf & _
                                                "   FROM   twksOrderCalculatedTests OCT INNER JOIN twksOrderTests OT ON OCT.CalcOrderTestID = OT.OrderTestID " & vbCrLf & _
                                                "  WHERE  OT.OrderTestStatus <> 'CLOSED'))) "

                        '...get also for ISE Tests
                        cmdText &= " UNION " & vbCrLf & _
                                    " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag, " & vbCrLf & _
                                    "       (CASE WHEN O.PatientID IS NOT NULL THEN 'DB' ELSE 'MAN' END) AS SampleIDType, " & vbCrLf & _
                                    "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, OT.CreationOrder, " & vbCrLf & _
                                    "       IT.[Name] AS TestName,  NULL AS CalcTestFormula, OT.ControlID, " & vbCrLf & _
                                    "       OT.OrderTestID, OTL.AwosID , OTL.ESOrderID , OTL.ESPatientID , OTL.LISOrderID , OTL.LISPatientID , OTL.RerunNumber , OTL.SpecimenID , " & vbCrLf & _
                                    "       O.OrderID, OT.LISRequest, OT.OrderTestStatus AS OTStatus, OT.ExternalQC " & vbCrLf & _
                                    " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                                    "                           INNER JOIN tparISETests AS IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                    "                           INNER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID " & vbCrLf & _
                                    " WHERE OT.TestType = 'ISE'  " & vbCrLf & _
                                    " AND   OT.LISRequest = 1" & vbCrLf & _
                                    " AND   O.SampleClass IN ('PATIENT') " & vbCrLf & _
                                    " AND   OT.OrderTestStatus <> 'CLOSED' " & vbCrLf & _
                                    " AND   OT.OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions WHERE LockedByLIS = 1) "

                        '...get also for Calculated Tests
                        cmdText &= " UNION " & vbCrLf & _
                                    " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag,  " & vbCrLf & _
                                    "       (CASE WHEN O.PatientID IS NOT NULL THEN 'DB' ELSE 'MAN' END) AS SampleIDType, " & vbCrLf & _
                                    "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, OT.CreationOrder, " & vbCrLf & _
                                    "       CT.CalcTestLongName AS TestName, CT.FormulaText AS CalcTestFormula, OT.ControlID, " & vbCrLf & _
                                    "       OT.OrderTestID, OTL.AwosID , OTL.ESOrderID , OTL.ESPatientID , OTL.LISOrderID , OTL.LISPatientID , OTL.RerunNumber , OTL.SpecimenID , " & vbCrLf & _
                                    "       O.OrderID, OT.LISRequest, OT.OrderTestStatus AS OTStatus, OT.ExternalQC " & vbCrLf & _
                                    " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                                    "                           INNER JOIN tparCalculatedTests AS CT ON OT.TestID = CT.CalcTestID " & vbCrLf & _
                                    "                           LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID  " & vbCrLf & _
                                    " WHERE OT.TestType = 'CALC'  " & vbCrLf & _
                                    " AND   OT.LISRequest = 1 " & vbCrLf & _
                                    " AND   O.SampleClass = 'PATIENT' " & vbCrLf & _
                                    " AND   OT.OrderTestStatus <> 'CLOSED' "

                        '...get also for Off-System Tests
                        cmdText &= " UNION " & vbCrLf & _
                                    " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag, " & vbCrLf & _
                                    "       (CASE WHEN O.PatientID IS NOT NULL THEN 'DB' ELSE 'MAN' END) AS SampleIDType, " & vbCrLf & _
                                    "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, OT.CreationOrder, " & vbCrLf & _
                                    "       OST.[Name] AS TestName, NULL AS CalcTestFormula, OT.ControlID, " & vbCrLf & _
                                    "       OT.OrderTestID, OTL.AwosID , OTL.ESOrderID , OTL.ESPatientID , OTL.LISOrderID , OTL.LISPatientID , OTL.RerunNumber , OTL.SpecimenID , " & vbCrLf & _
                                    "       O.OrderID, OT.LISRequest, OT.OrderTestStatus AS OTStatus, OT.ExternalQC " & vbCrLf & _
                                    " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                                    "                           INNER JOIN tparOffSystemTests AS OST ON OT.TestID = OST.OfFSystemTestID " & vbCrLf & _
                                    "                           LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID  " & vbCrLf & _
                                    " WHERE OT.TestType = 'OFFS'  " & vbCrLf & _
                                    " AND   OT.LISRequest = 1 " & vbCrLf & _
                                    " AND   O.SampleClass = 'PATIENT' " & vbCrLf & _
                                    " AND   OT.OrderTestStatus <> 'CLOSED' "

                        Dim resultsDS As New WorkSessionResultDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultsDS.Patients)
                            End Using
                        End Using

                        'CONTROLS 
                        'Get information for STD Tests
                        cmdText = " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag,  " & vbCrLf & _
                        "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, " & vbCrLf & _
                        "       T.TestName, NULL AS CalcTestFormula, " & vbCrLf & _
                        "       OTL.AwosID, OTL.SpecimenID, OTL.ESOrderID, OTL.LISOrderID, OTL.ESPatientID, OTL.LISPatientID,  " & vbCrLf & _
                        "       OT.LISRequest, OT.ExternalQC " & vbCrLf & _
                        " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                        "                           INNER JOIN tparTests AS T ON OT.TestID = T.TestID  " & vbCrLf & _
                        "                           INNER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID   " & vbCrLf & _
                        " WHERE OT.TestType = 'STD'  " & vbCrLf & _
                        " AND   OT.LISRequest = 1 " & vbCrLf & _
                        " AND   O.SampleClass IN ('CTRL') " & vbCrLf & _
                        " AND   OT.OrderTestStatus <> 'CLOSED' AND OT.OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions WHERE LockedByLIS = 1) "

                        '...get also for ISE Tests
                        cmdText &= " UNION " & vbCrLf & _
                                    " SELECT DISTINCT O.SampleClass AS SampleClass, (CASE WHEN O.PatientID IS NOT NULL THEN O.PatientID ELSE O.SampleID END) AS SampleID, O.StatFlag, " & vbCrLf & _
                                    "       OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.ReplicatesNumber as NumReplicates, " & vbCrLf & _
                                    "       IT.[Name] AS TestName,  NULL AS CalcTestFormula, " & vbCrLf & _
                                    "       OTL.AwosID, OTL.SpecimenID, OTL.ESOrderID, OTL.LISOrderID, OTL.ESPatientID, OTL.LISPatientID,  " & vbCrLf & _
                                    "       OT.LISRequest, OT.ExternalQC " & vbCrLf & _
                                    " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID  " & vbCrLf & _
                                    "                           INNER JOIN tparISETests AS IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                    "                           INNER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID " & vbCrLf & _
                                    " WHERE OT.TestType = 'ISE'  " & vbCrLf & _
                                    " AND   OT.LISRequest = 1" & vbCrLf & _
                                    " AND   O.SampleClass IN ('CTRL') " & vbCrLf & _
                                    " AND   OT.OrderTestStatus <> 'CLOSED' AND OT.OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions WHERE LockedByLIS = 1) "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultsDS.Controls)
                            End Using
                        End Using

                        resultData.SetDatos = resultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestsForLISReset", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of OPEN OrderTests that have to be added to a new WorkSession when the connected Analyzer has been changed 
        ''' and there was a WorkSession created for the previous one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing the list of OPEN OrderTests that have to be added to a new WorkSession when the 
        '''          connected Analyzer has been changed and there was a WorkSession created for the previous one</returns>
        ''' <remarks>
        ''' Created by:  SA 14/06/2012
        ''' </remarks>
        Public Function GetOrderTestsToChangeAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT '' AS WorkSessionID, OrderTestID, 1 AS OpenOTFlag, " & vbCrLf & _
                                                       " (CASE WHEN (TestType = 'STD' OR TestType = 'ISE') AND AlternativeOrderTestID IS NULL AND PreviousOrderTestID IS NULL " & vbCrLf & _
                                                             " THEN 1 ELSE 0 END) AS ToSendFlag " & vbCrLf & _
                                                " FROM   twksOrderTests " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestStatus = 'OPEN' " & vbCrLf

                        Dim myWSOrderTestsDS As New WSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSOrderTestsDS.twksWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = False
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOrderTestsToChangeAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For an OrderTest requested for a Patient that is stored in the DB, get the Patient data needed to validate
        ''' detailed Reference Ranges: Gender and/or Age and DateOfBirth
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with the Patient data</returns>
        ''' <remarks>
        ''' Created by:  SA 24/01/2011
        ''' Modified by: SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetOTPatientData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.PatientID, P.Gender, P.Age, P.DateOfBirth " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN tparPatients P ON O.PatientID = P.PatientID " & vbCrLf & _
                                                " WHERE  OT.OrderTestID = " & pOrderTestID

                        Dim patientDataDS As New PatientsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(patientDataDS.tparPatients)

                            End Using
                        End Using

                        resultData.SetDatos = patientDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = False
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetOTPatientData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Patients that have been not included in a WorkSession (those with Status OPEN) 
        ''' and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReturnOpenAsSelected">Optional parameter to indicate if OPEN Patient Samples have to be returned as SELECTED
        '''                                     (when its value is TRUE) or UNSELECTED (when its value is FALSE)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Controls obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 05/03/2010
        ''' Modified by: SA 17/03/2010 - For Patient Order Tests included in a WorkSession, return always "INPROCESS" as
        '''                              OrderTestStatus instead of the current value of field OrderTestStatus in twksOrderTests
        '''                              (needed to show Patient Order Tests properly sorted in the screen of WS Preparation)
        '''              SA 06/04/2010 - Get also field CreationOrder from table twksOrderTests. Sort results by CreationOrder
        '''              SA 26/04/2010 - Query changed to get Open and InProcess Patient Samples linked to the informed WorkSession;
        '''                              parameter pWorkSessionID changed from optional to fixed 
        '''              SA 18/05/2010 - Query changed to get also Open and InProcess Patient Samples linked to the informed WorkSession but
        '''                              containing Calculated Tests 
        '''              SA 21/10/2010 - Query changed to get also Open and InProcess Patient Samples for ISE Tests; besides, query changed
        '''                              to get Profile Information also for Calculated Tests
        '''              SA 02/12/2010 - Query changed to get also Open and InProcess Patient Samples for OFF System Tests
        '''              SA 18/01/2011 - Subqueries for OffSystem Tests changed: all Order Tests for this type of Tests are get as unselected
        '''                              and OPEN due to they can be removed from the WorkSession in whatever moment (or their result can be 
        '''                              changed)
        '''              SA 01/09/2011 - Changed the function template
        '''              TR 12/03/2013 - Changed all subQueries to get also field LISRequest from table twksOrderTests
        '''              SA 04/04/2013 - Added new optional parameter to indicate if OPEN Patient Samples have to be returned as SELECTED.
        '''                              Optional parameter with TRUE as default value. Changed all subQueries for OPEN OrderTests to return 
        '''                              them selected or not according value of this new parameter  
        '''              SA 09/04/2013 - Changed all subqueries to get value of SpecimenID for OrderTests that were requested by LIS
        '''              SA 22/05/2013 - Changed all subqueries to get value of AwosID for OrderTests that were requested by LIS
        '''              SA 01/04/2014 - Changed all subqueries to filter by RerunNumber = 1 all LEFT OUTER JOINs over table twksOrderTestsLISInfo, 
        '''                              to prevent Order Tests with Reruns requested by LIS are returned as many times as requested LIS Rerun they have 
        '''                              (error detected when testing changes due to BT #1564)
        '''              XB 28/08/2014 - Get the new field Selected from twksOrderTests table when value of entry parameter pReturnOpenAsSelected is True - BT #1868
        '''</remarks>
        Public Function GetPatientOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                             Optional ByVal pReturnOpenAsSelected As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        ' XB 27/08/2014 - BT #1868
                        'Dim selectedValue As String = IIf(pReturnOpenAsSelected, 1, 0).ToString
                        Dim selectedValue As String = "0"
                        If pReturnOpenAsSelected Then
                            selectedValue = "OT.Selected"
                        End If

                        'Get Open Patient Samples linked to the informed WorkSession for Standard Tests
                        Dim cmdText As String = " SELECT DISTINCT " & selectedValue & " AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                                       " OT.OrderTestID, OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                                       " OT.OrderTestStatus AS OTStatus, OT.TestProfileID, TP.TestProfileName, NULL AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                                       " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                                 " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                           " INNER JOIN tparTests AS T ON OT.TestID = T.TestID " & vbCrLf & _
                                                                           " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID AND OT.SampleType = TP.SampleType " & vbCrLf & _
                                                                           " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID AND OTL.RerunNumber = 1 " & vbCrLf & _
                                                 " WHERE OT.TestType = 'STD' " & vbCrLf & _
                                                 " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                          " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                          " AND   OpenOTFlag = 1) " & vbCrLf & _
                                                 " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '...get also Open Patient Samples linked to the informed WorkSession for Calculated Tests
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT " & selectedValue & " AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, CT.CalcTestLongName AS TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest," & vbCrLf & _
                                          " OT.OrderTestStatus AS OTStatus, OT.TestProfileID, TP.TestProfileName, '=' + CT.FormulaText AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM   twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                               " INNER JOIN tparCalculatedTests AS CT ON OT.TestID = CT.CalcTestID " & vbCrLf & _
                                                               " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID " & vbCrLf & _
                                                               " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'CALC' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                             " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                             " AND OpenOTFlag = 1) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '...get also Open Patient Samples linked to the informed WorkSession for ISE Tests
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT " & selectedValue & " AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, IT.[Name] AS TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                          " OT.OrderTestStatus AS OTStatus, OT.TestProfileID AS TestProfileID, TP.TestProfileName, NULL AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM   twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                               " INNER JOIN tparISETests AS IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                                               " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID AND OT.SampleType = TP.SampleType " & vbCrLf & _
                                                               " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'ISE' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                            " AND OpenOTFlag = 1) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '... get also In Process Patient Samples linked to the informed WorkSession for Standard Tests
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT OT.Selected AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                          " 'INPROCESS' AS OTStatus, OT.TestProfileID, TP.TestProfileName, NULL AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                             " INNER JOIN tparTests AS T ON OT.TestID = T.TestID " & vbCrLf & _
                                                             " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID AND OT.SampleType = TP.SampleType " & vbCrLf & _
                                                             " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'STD' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                            " AND   OpenOTFlag = 0) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '... get also In Process Patient Samples linked to the informed WorkSession for Calculated Tests
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT OT.Selected AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, CT.CalcTestLongName AS TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                          " 'INPROCESS' AS OTStatus, OT.TestProfileID, TP.TestProfileName, '=' + CT.FormulaText AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                             " INNER JOIN tparCalculatedTests AS CT ON OT.TestID = CT.CalcTestID " & vbCrLf & _
                                                             " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID " & vbCrLf & _
                                                             " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID  AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'CALC' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                            " AND   OpenOTFlag = 0) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '...get also In Process Patient Samples linked to the informed WorkSession for ISE Tests
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT OT.Selected AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, IT.[Name] AS TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                          " OT.OrderTestStatus AS OTStatus, OT.TestProfileID AS TestProfileID, TP.TestProfileName, NULL AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM   twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                               " INNER JOIN tparISETests AS IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                                               " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID AND OT.SampleType = TP.SampleType " & vbCrLf & _
                                                               " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'ISE' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                            " AND OpenOTFlag = 0) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        '...finally, get all Patient Samples linked to the informed WorkSession for Off-System Tests as unselected and Open
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT OT.Selected AS Selected, O.SampleClass AS SampleClass, O.StatFlag, O.PatientID, O.SampleID, O.OrderID, OT.TubeType, " & vbCrLf & _
                                          " OT.OrderTestID, OT.TestType, OT.TestID, OST.[Name] AS TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates,  OT.LISRequest, " & vbCrLf & _
                                          " 'OPEN' AS OTStatus, OT.TestProfileID AS TestProfileID, TP.TestProfileName, NULL AS CalcTestFormula, OT.CreationOrder, OT.ExternalQC, " & vbCrLf & _
                                          " OTL.SpecimenID, OTL.AwosID " & vbCrLf & _
                                   " FROM   twksOrderTests AS OT INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                               " INNER JOIN tparOffSystemTests AS OST ON OT.TestID = OST.OfFSystemTestID " & vbCrLf & _
                                                               " LEFT OUTER JOIN tparTestProfiles AS TP ON OT.TestProfileID = TP.TestProfileID AND OT.SampleType = TP.SampleType " & vbCrLf & _
                                                               " LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OT.OrderTestID = OTL.OrderTestID  AND OTL.RerunNumber = 1 " & vbCrLf & _
                                   " WHERE OT.TestType = 'OFFS' " & vbCrLf & _
                                   " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                            " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                            " AND OpenOTFlag = 1) " & vbCrLf & _
                                   " AND   O.SampleClass = 'PATIENT' " & vbCrLf

                        cmdText &= " ORDER BY OT.CreationOrder " & vbCrLf

                        Dim patientsDS As New WorkSessionResultDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(patientsDS.Patients)
                            End Using
                        End Using

                        resultData.SetDatos = patientsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetPatientOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of different Patient's Sample Tubes of the WorkSession in the order they have been requested
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TubesBySampleTypeDS with the list of different Patient's Sample Tubes
        '''          in the order they have been requested</returns>
        ''' <remarks>
        ''' Created by:  SA 09/11/2011
        ''' </remarks>
        Public Function GetPatientSamplesCreationOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.OrderID, O.PatientID, O.SampleID, OT.SampleType, MIN(OT.CreationOrder) AS CreationOrder " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                " AND    WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag = 1 " & vbCrLf & _
                                                " GROUP BY O.OrderID, O.PatientID, O.SampleID, OT.SampleType " & vbCrLf & _
                                                " ORDER BY CreationOrder "

                        Dim resultData As New TubesBySampleTypeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.TubesBySampleTypeTable)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetPatientSamplesCreationOrder", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of different pairs of TestID/SampleType in the specified list of Order Tests
        ''' (which have been included in a Work Session and have to be executed in an Analyzer).  For
        ''' each pair of TestID/SampleType, the number of replicates for the different possible Sample 
        ''' Classes (Blanks, Calibrators, Controls and/or Patient Samples), the Reagent and the  
        ''' the required volume of it are also obtained 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReplicatesDS with the list of pairs TestID/SampleType 
        '''          that have to be executed in a WorkSession and for each one of them, the number of replicates defined 
        '''          for the different Sample Classes and information about the Reagent (identifier and required volume 
        '''          expressed in ml)</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changed the Return Type TestReplicatesDS to GlobalDataTO; changed the way of 
        '''                              opening the DB Connection to fulfill the new template
        '''              SA 30/04/2010 - Added optional parameter WorkSessionID (needed to improve performance). Query changed to 
        '''                              ANSI format. When WorkSessionID is informed, filter also by OrderTestStatus = OPEN
        '''              SA 03/07/2010 - When pWorkSessionID is informed, query was bad built: WHERE was missing, then the 
        '''                              correct data was not returned in all cases
        '''              SA 27/08/2010 - Added filter to subquery to get only the OrderTestID that have to be positioned
        '''              SA 10/11/2010 - Added filter to subqueries to execute the searching only for Standard Tests
        '''              SA 01/09/2011 - Changed the function template; function name changed to GetReagentsByTest; query changed 
        '''                              due to this function now get only the Reagents data, not the number of Replicates nor the
        '''                              number of Controls
        ''' </remarks>
        Public Function GetReagentsByTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                          Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OT.TestID, OT.SampleType, TV.ReagentID, TV.ReagentNumber, " & vbCrLf & _
                                                                " TV.ReagentVolume/1000 AS ReagentVolume " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN tparTestReagentsVolumes TV ON OT.TestID = TV.TestID " & vbCrLf & _
                                                                                                              " AND OT.SampleType = TV.SampleType " & vbCrLf

                        If (pWorkSessionID <> "") Then
                            cmdText &= " WHERE OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND   OT.TestType = 'STD' " & vbCrLf & _
                                       " AND   OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                " AND    OpenOTFlag = 0 " & vbCrLf & _
                                                                " AND    ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdText &= " WHERE  OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf & _
                                       " AND   OT.TestType = 'STD' " & vbCrLf
                        End If

                        Dim myTestReplicatesDS As New TestReplicatesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestReplicatesDS.TestReplicates)
                            End Using
                        End Using

                        resultData.SetDatos = myTestReplicatesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetReagentsByTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search the SampleType for the specified OrderTestID, which corresponds to an Experimental Calibrator used as Alternative for the
        ''' same Test but a different Sample Type 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of a Calibrator Order Test used as Alternative for the same Test and another SampleType</param>
        ''' <returns>GlobalDataTO containing a  string value with the SampleType for which the Experimental Calibrator is defined</returns>
        ''' <remarks>
        ''' Created by:  SA 15/05/2013
        ''' </remarks>
        Public Function GetSampleTypeForAlternativeOT(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SampleType AS CalibSType FROM twksOrderTests " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID & vbCrLf

                        resultData.SetDatos = String.Empty
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = dbDataReader.Item("CalibSType").ToString
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestDAO.GetampleTypeForAlternativeOT", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of different Sample Types needed for the specified PatientID/SampleID according the list of Tests that have been 
        ''' requested in the active WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">PatientID or SampleID</param>
        ''' <param name="pOnlySentToPos">When TRUE, it indicates only SampleTypes of Order Tests selected to be positioned (OpenOTFlag = FALSE) will be returned
        '''                              When FALSE, SampleTypes of all Order Tests will be returned. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of different Sample Types needed for the specified
        '''          Patient/Sample according the requested Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 01/09/2011
        ''' Modified by: SA 10/04/2013 - Added optional parameter pOnlySentToPos to indicate if only OrderTests with OpenOTFlag=FALSE have to be returned 
        '''              SA 17/04/2013 - Deleted parameter pPatientExists. Changed the SQL Query to filter twksOrders table by PatientID OR SampleID equal
        '''                              to the value specified by parameter pPatientID
        ''' </remarks>
        Public Function GetSampleTypesByPatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                 ByVal pPatientID As String, Optional ByVal pOnlySentToPos As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OT.SampleType " & vbCrLf & _
                                                " FROM twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                       " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND   O.SampleClass      = 'PATIENT' " & vbCrLf & _
                                                " AND  (O.SampleID         = N'" & pPatientID.Replace("'", "''") & "' " & vbCrLf & _
                                                " OR    O.PatientID        = N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf

                        If (pOnlySentToPos) Then cmdText &= " AND   WSOT.OpenOTFlag    = 0 " & vbCrLf

                        Dim myDataSet As New OrderTestsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTests)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetSampleTypesByPatient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of required elements for Patient's Sample Types for a group of Order Tests having ToSendFlag=True and
        ''' belonging to Orders with SampleClass Patient. For each Patient, only one element will be created for each different 
        ''' SampleType (no matter how many Orders are defined for she/he). Only for OrderTests of requested ISE Tests       
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TubesBySampleTypeDS with the list of different Patient's Sample Tubes 
        '''          that are required in a WorkSession for the informed list of Order Tests (for ISE Tests)</returns>
        ''' <remarks>
        ''' Created by:  SA 21/10/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetSampleTypesTubesISE(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                               Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Orders belonging to Patients saved in the DB
                        Dim cmdText As String = " SELECT DISTINCT NULL AS OrderID, O.PatientID AS PatientID, NULL AS SampleID, ITS.SampleType, NULL AS PredilutionUseFlag, " & vbCrLf & _
                                                                " ITS.ISE_DilutionFactor AS PredilutionFactor, NULL AS PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                                " FROM   tparISETestSamples ITS INNER JOIN  twksOrderTests OT ON ITS.ISETestID = OT.TestID AND ITS.SampleType = OT.SampleType " & vbCrLf & _
                                                                              " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    O.PatientID IS NOT NULL " & vbCrLf & _
                                                " AND    OT.TestType = 'ISE' " & vbCrLf

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND    OpenOTFlag = 0 " & vbCrLf & _
                                                              " AND    ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        'Get also all Orders belonging to anonymus or manually informed Patients 
                        cmdText += " UNION " & vbCrLf & _
                                   " SELECT DISTINCT NULL AS OrderID, NULL AS PatientID, O.SampleID AS SampleID, ITS.SampleType, NULL AS PredilutionUseFlag, " & vbCrLf & _
                                          " ITS.ISE_DilutionFactor AS PredilutionFactor, NULL AS PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                   " FROM   tparISETestSamples ITS INNER JOIN  twksOrderTests OT ON ITS.ISETestID = OT.TestID AND ITS.SampleType = OT.SampleType " & vbCrLf & _
                                                                 " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                   " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                   " AND    O.SampleID IS NOT NULL " & vbCrLf & _
                                   " AND    OT.TestType = 'ISE' " & vbCrLf

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND    OpenOTFlag = 0 " & vbCrLf & _
                                                              " AND    ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If
                        cmdText += " ORDER BY PatientID, SampleID, ITS.SampleType, ITS.ISE_DilutionFactor " & vbCrLf

                        Dim resultData As New TubesBySampleTypeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.TubesBySampleTypeTable)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetSampleTypesTubesISE", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of required elements for Patient's Sample Types for a group of Order Tests having ToSendFlag=True and
        ''' belonging to Orders with SampleClass Patient. For each Patient, only one element will be created for each different 
        ''' SampleType (no matter how many Orders are defined for she/he). Only for OrderTests of requested Standard Tests    
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TubesBySampleTypeDS with the list of different Patient's Sample Types 
        '''          that are required in a WorkSession for the informed list of Order Tests (for Standard Tests)</returns>
        ''' <remarks>
        ''' Created by:  SA 
        ''' Modified by: VR 10/12/2009 - Change the Return Type TubesBySampleTypeDS to GlobalDataTO and added TS.PredilutionMode in 
        '''                              both parts of UNION - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template 
        '''              SA 09/03/2010 - Changes needed to shown the manual SampleID instead of PatientID or OrderID
        '''              SA 17/03/2010 - Return also field TubeType from twksOrderTests Table
        '''              SA 03/05/2010 - Added optional parameter WorkSessionID (needed to improve performance). Query changed to 
        '''                              ANSI format. When WorkSessionID is informed, filter also by OrderTestStatus = OPEN
        '''              SA 14/07/2010 - Filter the subquery used when WorkSessionID is informed by ToSendFlag=1 to avoid the return
        '''                              of SampleTypes defined for Calculated Tests
        '''              SA 21/10/2010 - Name changed from GetSampleTypesTubes to GetSampleTypesTubesSTD. Get OnlyForISE field as False 
        '''              SA 20/04/2011 - Changed the query to avoid getting more than one record when for the same Patient there are 
        '''                              requests for Tests without Predilution or with Predilution by Analyzer
        '''              TR 05/08/2011 - Implemented the Using Sentence to execute the SQL Query
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetSampleTypesTubesSTD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                               Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdWSConditions As String = ""
                        If (pWorkSessionID.Trim <> "") Then
                            cmdWSConditions = " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                              " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                     " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                     " AND    OpenOTFlag = 0 " & vbCrLf & _
                                                                     " AND    ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdWSConditions = " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        'Get all Orders belonging to Patients saved in the DB
                        Dim cmdText As String = " SELECT DISTINCT NULL AS OrderID, O.PatientID AS PatientID, NULL AS SampleID, TS.SampleType, 0 AS PredilutionUseFlag, " & vbCrLf & _
                                                                " NULL AS PredilutionFactor, NULL AS PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                                " FROM   tparTestSamples TS INNER JOIN  twksOrderTests OT ON TS.TestID = OT.TestID AND TS.SampleType = OT.SampleType " & vbCrLf & _
                                                                          " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    O.PatientID IS NOT NULL " & vbCrLf & _
                                                " AND    OT.TestType = 'STD' " & vbCrLf & _
                                                " AND   (TS.PredilutionUseFlag = 0 " & vbCrLf & _
                                                " OR    (TS.PredilutionUseFlag = 1 AND TS.PredilutionMode = 'INST')) " & vbCrLf & _
                                                cmdWSConditions & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT DISTINCT NULL AS OrderID, O.PatientID AS PatientID, NULL AS SampleID, TS.SampleType, TS.PredilutionUseFlag, " & vbCrLf & _
                                                                " TS.PredilutionFactor, TS.PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                                " FROM   tparTestSamples TS INNER JOIN  twksOrderTests OT ON TS.TestID = OT.TestID AND TS.SampleType = OT.SampleType " & vbCrLf & _
                                                                          " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    O.PatientID IS NOT NULL " & vbCrLf & _
                                                " AND    OT.TestType = 'STD' " & vbCrLf & _
                                                " AND   (TS.PredilutionUseFlag = 1 AND TS.PredilutionMode = 'USER') " & vbCrLf & _
                                                cmdWSConditions & vbCrLf

                        'Get also all Orders belonging to anonymus or manually informed Patients 
                        cmdText += " UNION " & vbCrLf & _
                                   " SELECT DISTINCT NULL AS OrderID, NULL AS PatientID, O.SampleID AS SampleID, TS.SampleType, 0 AS PredilutionUseFlag, " & vbCrLf & _
                                         " NULL AS PredilutionFactor, NULL AS PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                   " FROM   tparTestSamples TS INNER JOIN  twksOrderTests OT ON TS.TestID = OT.TestID AND TS.SampleType = OT.SampleType " & vbCrLf & _
                                                             " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                   " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                   " AND    O.SampleID IS NOT NULL " & vbCrLf & _
                                   " AND    OT.TestType = 'STD' " & vbCrLf & _
                                   " AND   (TS.PredilutionUseFlag = 0 " & vbCrLf & _
                                   " OR    (TS.PredilutionUseFlag = 1 AND TS.PredilutionMode = 'INST')) " & vbCrLf & _
                                   cmdWSConditions & vbCrLf & _
                                   " UNION " & vbCrLf & _
                                   " SELECT DISTINCT NULL AS OrderID, NULL AS PatientID, O.SampleID AS SampleID, TS.SampleType, TS.PredilutionUseFlag, " & vbCrLf & _
                                          " TS.PredilutionFactor, TS.PredilutionMode, 0 AS OnlyForISE, OT.TubeType, OT.CreationOrder " & vbCrLf & _
                                   " FROM   tparTestSamples TS INNER JOIN  twksOrderTests OT ON TS.TestID = OT.TestID AND TS.SampleType = OT.SampleType " & vbCrLf & _
                                                             " INNER JOIN  twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                   " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                   " AND    O.SampleID IS NOT NULL " & vbCrLf & _
                                   " AND    OT.TestType = 'STD' " & vbCrLf & _
                                   " AND   (TS.PredilutionUseFlag = 1 AND TS.PredilutionMode = 'USER') " & vbCrLf & _
                                   cmdWSConditions & vbCrLf

                        'Finally, set the sort fields
                        cmdText += " ORDER BY PatientID, SampleID, TS.SampleType, PredilutionUseFlag, PredilutionFactor " & vbCrLf

                        Dim resultData As New TubesBySampleTypeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.TubesBySampleTypeTable)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetSampleTypesTubesSTD", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the total volume needed for Blanks executed using an specific Special Solution according the list of 
        ''' Order Tests included in a Work Session. The formula used to calculate the required volume is:
        '''    (Sample Volume + Constant Volumes)/1000 * Blank Replicates
        ''' Where:
        '''    * Sample Volume: value defined for each Test/SampleType in table tparTests
        '''    * Blank Replicates: number of Blank replicates defined for each Test in table tparTests
        '''    * Constant Volumes: additional volume that have to be aspired for mechanical reasons (depend on the Analyzer)
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pSpecialSolutionCode">Code of the Special Solution</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing the needed total volume of the specified Solution Code</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changed the Return Type Single to GlobalDataTO; changed the way of 
        '''                              opening the DB Connection to fulfill the new template
        '''              SA 30/04/2010 - Added optional parameter WorkSessionID (needed to improve performance). Query changed to 
        '''                              ANSI format. When WorkSessionID is informed, filter also by OrderTestStatus = OPEN
        '''              SA 27/08/2010 - Added filter to subquery to get only the OrderTestID that have to be positioned
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 16/02/2012 - Removed parameter pConstantVolumes; it is not needed for Ax00 Analyzers
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetSpecialSolutionVolumeForBlanks(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                                          ByVal pSpecialSolutionCode As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SUM((TS.SampleVolume)/1000 * T.BlankReplicates) AS VolumeSpecialSol " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN tparTestSamples TS ON OT.TestID = TS.TestID AND OT.SampleType = TS.SampleType " & vbCrLf & _
                                                                         " INNER JOIN tparTests T ON OT.TestID= T.TestID " & vbCrLf & _
                                                " WHERE O.SampleClass = 'BLANK' " & vbCrLf & _
                                                " AND   OT.TestType   = 'STD' " & vbCrLf & _
                                                " AND   T.BlankMode   = '" & pSpecialSolutionCode.Trim & "' " & vbCrLf     '--> BlankMode Codes are the same as SpecialSolution Codes

                        If (pWorkSessionID <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND    OpenOTFlag    = 0 " & vbCrLf & _
                                                              " AND    ToSendFlag    = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        Dim solutionVolume As Single = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    solutionVolume = CSng(dbDataReader.Item("VolumeSpecialSol"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = solutionVolume
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetSpecialSolutionVolumeForBlanks", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is at least a Test of the specified TestType requested in the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestType">Test Type code: STD or ISE</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if there are Tests of the specified type requested in the WorkSession</returns>
        ''' <remarks>
        ''' Created by:  RH 15/06/2011
        ''' Modified by: SA 20/03/2012 - Added parameter for the WorkSessionID; changed the query to filter OrderTests of the active WS
        '''              SA 14/05/2012 - Function name changed from IsThereAnyISETest to IsThereAnyTestByType, and added parameter to inform 
        '''                              the TestType. These changes are to allow using the function for both STD and ISE Tests
        ''' </remarks>
        Public Function IsThereAnyTestByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                             ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE  OT.TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus  <> 'CLOSED' " & vbCrLf & _
                                                " AND    WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag = 0 " & vbCrLf

                        Dim thereAreTests As Boolean = False
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    thereAreTests = CBool(dbDataReader.Item(0))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = thereAreTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.IsThereAnyTestByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in table twksOrderTests:
        ''' ** If there is a Calibrator Order Test for the specified TestID/SampleType OR
        ''' ** If there is a Blank Order Test for the specified TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Sample Class Code: BLANK or CALIB</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Code of the Sample Type. Optional parameter, informed only when the informed SampleClass is CALIB</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsDS containing all data for the found Calibrator or Blank Order Test</returns>
        ''' <remarks>
        ''' Created by:  SA 15/05/2013
        ''' </remarks>
        Public Function ReadBlankOrCalibByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String, ByVal pTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.*, O.OrderID FROM twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    OT.TestType   = 'STD' " & vbCrLf & _
                                                " AND    OT.TestID     = " & pTestID.ToString.Trim & vbCrLf

                        'If the SampleType parameter has been informed, then the filter is added to the query
                        If (pSampleType <> String.Empty) Then cmdText &= " AND OT.SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myOrderTestsDS
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.ReadBlankOrCalibByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search if the Order Test ID of a Calibrator is used as Alternative for the same Test and another
        ''' Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test used as Alternative</param>
        ''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of Order Tests
        '''          using the informed one as Alternative</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        '''              SA 12/04/2012 - Added optional parameter pWorkSessionID to allow excluding OrderTests that are 
        '''                              already linked to the active WorkSession
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        '''              SA 02/10/2012 - Sort returned records by SampleType
        ''' </remarks>
        Public Function ReadByAlternativeOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                     Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID, SampleType " & vbCrLf & _
                                                " FROM   twksOrderTests " & vbCrLf & _
                                                " WHERE  AlternativeOrderTestID = " & pOrderTestID & vbCrLf & _
                                                " AND    TestType               = 'STD' " & vbCrLf

                        If (pWorkSessionID <> String.Empty) Then cmdText &= " AND OrderTestID NOT IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                                                    " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "') " & vbCrLf
                        cmdText &= " ORDER BY SampleType " & vbCrLf

                        Dim alternativeOTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(alternativeOTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = alternativeOTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestDAO.ReadByAlternativeOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of OrderTests having the specified Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Order Test Status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of OrderTests having the specified Status</returns>
        ''' <remarks>
        ''' Created by: AG 18/07/2013
        ''' </remarks>
        Public Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.* " & vbCrLf & _
                                                " FROM   twksOrderTests OT " & vbCrLf & _
                                                " WHERE  OT.OrderTestStatus = '" & pStatus & "' "

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.ReadByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' Modified by: SA  31/01/2012 - Changed the function template
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTests " & vbCrLf & _
                                            " WHERE OrderTestID NOT IN (SELECT OrderTestID FROM twksWSOrderTests) " & vbCrLf & _
                                            " AND   OrderTestID NOT IN (SELECT OrderTestID FROM twksResults) "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum OrderTestID currently stored in table twksOrderTests (if any) and restart the Identity
        ''' value for that column. New initial value for OrderTestID will be 1 if there are not records in the table.
        ''' or Max(OrderTestID)+1 if there are recoreds int it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 07/09/2010
        ''' Modified by: SA 22/09/2011 - When there are not records in the table, the SEED value will be always 1 (it 
        '''                              could be 0 according the MSSQLSERVER documentation, but sometimes it does not 
        '''                              work and insert the first record with ID 0 instead of ID 1. When there are 
        '''                              records in the table, then it will be MAX(OrderTestID)+1
        ''' </remarks>
        Public Function RestartIdentity(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(OrderTestID) AS IniSeedValue FROM twksOrderTests "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim iniSeedVal As Integer = 1
                        Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                        If (dbDataReader.HasRows) Then
                            dbDataReader.Read()
                            If (Not DBNull.Value.Equals(dbDataReader.Item("IniSeedValue"))) Then
                                iniSeedVal = CInt(dbDataReader.Item("IniSeedValue")) + 1
                            End If
                        End If
                        dbDataReader.Close()

                        'Restart the initial value of the Identity column OrderTestID
                        cmdText = " DBCC CHECKIDENT ('twksOrderTests', RESEED, " & iniSeedVal.ToString & ") "
                        dbCmd.CommandText = cmdText
                        dbCmd.ExecuteNonQuery()
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.RestartIdentity", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Dilution Solutions needed for automatic dilutions programmed for Tests/SampleTypes requested for Patient Samples 
        ''' in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with the list of Dilution Solutions needed for automatic dilutions 
        '''          programmed for Tests/SampleTypes requested for Patient Samples in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2010
        ''' Modified by: SA 26/01/2011 - Add new parameter to specify the Diluent Solution, and change the query to filter
        '''                              for field DiluentSolution in table tparTestSamples
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 28/05/2014 - BT #1519 ==> Changes to get all Dilution Solutions needed in the specified Work Session instead of 
        '''                                           verify for an specific Dilution Solution if it is needed for automatic predilutions of 
        '''                                           one or more Tests requested for Patient Samples in the Work Session:
        '''                                           * Parameter pDiluentSolution has been removed
        '''                                           * Return value has been changed from INTEGER to a typed DataSet TestSamplesDS containing the
        '''                                             list of Dilution Solutions needed in the Work Session
        '''                                           * Query has been also changed to get the list of needed Dilution Solution
        ''' </remarks>
        Public Function VerifyAutomaticDilutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                                 Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT TS.DiluentSolution " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                         " INNER JOIN tparTestSamples TS ON OT.TestID = TS.TestID AND OT.SampleType = TS.SampleType " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    OT.TestType = 'STD' " & vbCrLf & _
                                                " AND    TS.PredilutionMode = 'INST' " & vbCrLf 

                        If (pWorkSessionID <> "") Then
                            cmdText &= " AND OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                       " AND OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                              " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                              " AND   OpenOTFlag = 0 " & vbCrLf & _
                                                              " AND   ToSendFlag = 1) " & vbCrLf
                        Else
                            cmdText &= " AND OT.OrderTestID IN (" & pOrderTestsList & ") " & vbCrLf
                        End If

                        Dim myTestSamplesDS As New TestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.VerifyAutomaticDilutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NEW METHODS NEEDED FOR IMPLEMENTATION OF HISTORIC MODULE"
        '''' <summary>
        '''' Add a list of Order Tests to an existing Order
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pNewOrderTests">List of Order Tests to add</param>
        '''' <returns>Global object containing error information and the updated DataSet</returns>
        '''' <remarks>
        '''' Created by: 
        '''' Modified by: VR 10/12/2009 - Included field ReplicatesNumber in insert command (TESTED: OK)
        ''''              VR 05/02/2010 - Included field TestProfileID in Insert Command (Tested: OK)
        ''''              SA 23/02/2010 - Included fields PreviousOrderTestID, AlternativeOrderTestID and ControlNumber in Insert Command
        ''''              SA 26/03/2010 - Included field CreationOrder in Insert Command
        ''''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_User; if audit fields are not informed
        ''''                              in the parameter DS, use the current logged User and the current date
        ''''              TR 06/08/2011 - Do not declare variables inside loops 
        ''''              SA 29/08/2012 - Query changed to insert value of new field PreviousWSID when it is informed
        ''''              SA 04/10/2012 - Query changed to insert value of new field CalibrationFactor when it is informed
        '''' </remarks>
        'Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewOrderTests As OrderTestsDS) As GlobalDataTO
        '    Dim dataToReturn As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            dataToReturn.HasError = True
        '            dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not IsNothing(pNewOrderTests)) Then
        '            Dim dbCmd As New SqlCommand
        '            dbCmd.Connection = pDBConnection

        '            Dim i As Integer = 0
        '            Dim cmdText As String = ""
        '            Dim noError As Boolean = True
        '            Dim nextOrderTestID As Integer = 0
        '            Do While (i < pNewOrderTests.twksOrderTests.Rows.Count) AndAlso (noError)
        '                'SQL Sentence to insert the new Order Test
        '                cmdText = " INSERT INTO twksOrderTests(OrderID, TestType, TestID, SampleType, OrderTestStatus, TubeType, AnalyzerID, " & _
        '                                                     " TestProfileID, ExportDateTime, ReplicatesNumber, PreviousOrderTestID, PreviousWSID, " & _
        '                                                     " CalibrationFactor, AlternativeOrderTestID, ControlID, CreationOrder, TS_User, TS_DateTime) " & _
        '                          " VALUES('" & pNewOrderTests.twksOrderTests(i).OrderID & "', " & _
        '                                 " '" & pNewOrderTests.twksOrderTests(i).TestType & "', " & _
        '                                        pNewOrderTests.twksOrderTests(i).TestID & ", " & _
        '                                 " '" & pNewOrderTests.twksOrderTests(i).SampleType & "', " & _
        '                                 " '" & pNewOrderTests.twksOrderTests(i).OrderTestStatus & "', "

        '                If (pNewOrderTests.twksOrderTests(i).IsTubeTypeNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= " '" & pNewOrderTests.twksOrderTests(i).TubeType & "', "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsAnalyzerIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= " '" & pNewOrderTests.twksOrderTests(i).AnalyzerID & "', "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsTestProfileIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).TestProfileID & ", "
        '                End If

        '                'Field ExportDateTime is always Null when creation
        '                cmdText = cmdText & " NULL, "

        '                If (pNewOrderTests.twksOrderTests(i).IsReplicatesNumberNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).ReplicatesNumber & ", "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsPreviousOrderTestIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).PreviousOrderTestID & ", "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsPreviousWSIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= " '" & pNewOrderTests.twksOrderTests(i).PreviousWSID.Trim & "', "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsCalibrationFactorNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= ReplaceNumericString(pNewOrderTests.twksOrderTests(i).CalibrationFactor) & ", "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsAlternativeOrderTestIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).AlternativeOrderTestID & ", "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsControlIDNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).ControlID & ", "
        '                End If

        '                If (pNewOrderTests.twksOrderTests(i).IsCreationOrderNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= pNewOrderTests.twksOrderTests(i).CreationOrder & ", "
        '                End If

        '                'Audit fields 
        '                If (pNewOrderTests.twksOrderTests(i).IsTS_UserNull) Then
        '                    Dim myGlobalBase As New GlobalBase
        '                    cmdText &= " N'" & myGlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
        '                Else
        '                    cmdText &= " N'" & pNewOrderTests.twksOrderTests(i).TS_User.Replace("'", "''") & "', "
        '                End If
        '                If (pNewOrderTests.twksOrderTests(i).IsTS_DateTimeNull) Then
        '                    cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
        '                Else
        '                    cmdText += " '" & pNewOrderTests.twksOrderTests(i).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
        '                End If

        '                'Execute the SQL Sentence
        '                dbCmd.CommandText = cmdText
        '                dbCmd.CommandText &= " SELECT SCOPE_IDENTITY()"

        '                nextOrderTestID = 0
        '                nextOrderTestID = CType(dbCmd.ExecuteScalar(), Integer)

        '                If (nextOrderTestID > 0) Then
        '                    'Get the generated Order Test ID and update the correspondent field in the DataSet
        '                    pNewOrderTests.twksOrderTests(i).BeginEdit()
        '                    pNewOrderTests.twksOrderTests(i).OrderTestID = nextOrderTestID
        '                    pNewOrderTests.twksOrderTests(i).EndEdit()
        '                Else
        '                    'If the OrderTestID was not returned, then it was an uncontrolled error
        '                    noError = False
        '                End If
        '                i += 1
        '            Loop

        '            If (noError) Then
        '                dataToReturn.HasError = False
        '                dataToReturn.AffectedRecords = i
        '                dataToReturn.SetDatos = pNewOrderTests
        '            Else
        '                dataToReturn.HasError = True
        '                dataToReturn.AffectedRecords = 0
        '            End If
        '        End If
        '    Catch ex As Exception
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        dataToReturn.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CreateNEW", EventLogEntryType.Error, False)
        '    End Try
        '    Return dataToReturn
        'End Function

        '''' <summary>
        '''' Get the list of Order Tests of Blanks and Calibrators that have been not included in a WorkSession (those
        '''' with Status OPEN) and optionally, also the list of Order Tests already linked to the specified Work Session
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Blanks or Calibrators
        ''''          obtained</returns>
        '''' <remarks>
        '''' Created by:  SA 23/02/2010
        '''' Modified by: SA 17/03/2010 - For Blank and Calibrator Order Tests included in a WorkSession, return always "INPROCESS" as
        ''''                              OrderTestStatus instead of the current value of field OrderTestStatus in twksOrderTests
        ''''                              (needed to show Blank and Calibrator Order Tests properly sorted in the screen of WS Preparation);
        ''''                              Queries changed to ANSI.
        ''''              SA 26/03/2010 - Get also field CreationOrder from table twksOrderTests. Sort results by SampleClass and CreationOrder    
        ''''              SA 26/04/2010 - Query changed to get Open and InProcess Blanks and Calibrators linked to the informed WorkSession;
        ''''                              parameter pWorkSessionID changed from optional to fixed
        ''''              SA 01/09/2010 - Query changed to allow get value of twksWSOrderTests.ToSendFlag. Table twksWSOrderTests is now included
        ''''                              in the JOIN instead of be part of a subquery in the WHERE clause         
        ''''              SA 01/09/2011 - Changed the function template  
        ''''              SA 29/08/2012 - Query changed to get also new field PreviousWSID from table twksOrderTests
        ''''              SA 04/10/2012 - Query changed to get also new field CalibrationFactor (as FactorValue) from table twksOrderTests
        '''' </remarks>
        'Public Function GetBlankCalibOrderTestsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Get Open Blanks and Calibrators linked to the informed WorkSession
        '                Dim cmdText As String = " SELECT 1 AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, " & vbCrLf & _
        '                                               " OT.TubeType,  OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates, " & vbCrLf & _
        '                                               " 1 AS NewCheck,  OT.PreviousOrderTestID, OT.PreviousWSID, OT.CalibrationFactor AS FactorValue,  " & vbCrLf & _
        '                                               " OT.OrderTestStatus AS OTStatus, OT.CreationOrder, WSOT.ToSendFlag, T.BlankMode " & vbCrLf & _
        '                                         " FROM  twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
        '                                                                 " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
        '                                                                 " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
        '                                         " WHERE OT.TestType = 'STD' " & vbCrLf & _
        '                                         " AND   WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
        '                                         " AND   WSOT.OpenOTFlag = 1 " & vbCrLf & _
        '                                         " AND   OT.AlternativeOrderTestID IS NULL " & vbCrLf & _
        '                                         " AND   O.SampleClass IN ('BLANK', 'CALIB') " & vbCrLf

        '                '... and get In Process Blanks and Calibrators linked to the informed WorkSession
        '                cmdText &= " UNION " & vbCrLf & _
        '                           " SELECT 1 AS Selected, O.SampleClass AS SampleClass, OT.OrderID, OT.OrderTestID, OT.TubeType, " & vbCrLf & _
        '                                  " OT.TestType, OT.TestID, T.TestName, OT.SampleType, OT.ReplicatesNumber AS NumReplicates, 1 AS NewCheck, " & vbCrLf & _
        '                                  " OT.PreviousOrderTestID, OT.PreviousWSID, OT.CalibrationFactor AS FactorValue, 'INPROCESS' AS OTStatus, " & vbCrLf & _
        '                                  " OT.CreationOrder, WSOT.ToSendFlag, T.BlankMode " & vbCrLf & _
        '                           " FROM   twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
        '                                                    " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
        '                                                    " INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf & _
        '                           " WHERE  OT.TestType = 'STD' " & vbCrLf & _
        '                           " AND    WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
        '                           " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
        '                           " AND    OT.AlternativeOrderTestID IS NULL " & vbCrLf & _
        '                           " AND    O.SampleClass IN ('BLANK', 'CALIB') " & vbCrLf

        '                '... sort all obtained Blanks and Calibrators by SampleClass and CreationOrder
        '                cmdText &= " ORDER BY O.SampleClass, OT.CreationOrder " & vbCrLf

        '                Dim blankCalibsDS As New WorkSessionResultDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(blankCalibsDS.BlankCalibrators)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = blankCalibsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetBlankCalibOrderTestsNEW", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Update value of field CalibrationFactor for the specified Order Test ID.  It is used for requested single point 
        '''' Calibrators using a result from a previous WorkSession when value of CalibrationFactor is change from the 
        '''' screen of WS Samples Requests 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pOrderTestsID">Order Test Identifier</param>
        '''' <param name="pCalibFactor">Calibration Factor entered manually</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 04/10/2012
        '''' </remarks>
        'Public Function UpdateCalibrationFactor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsID As Integer, _
        '                                        ByVal pCalibFactor As Single) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE twksOrderTests " & vbCrLf & _
        '                                    " SET    CalibrationFactor = " & ReplaceNumericString(pCalibFactor) & vbCrLf & _
        '                                    " WHERE  OrderTestID       = " & pOrderTestsID.ToString

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateCalibrationFactor", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

#Region "TO TEST - NEW METHODS TO DIVIDE ADD WS IN SEVERAL DB TRANSACTIONS"
        ''' <summary>
        ''' Update value of field CreationOrder for all Order Tests requested by an external LIS system but still not positioned (these with
        ''' OrderTestStatus = OPEN
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLstOpenLISOTs">List of WorkSessionResultDS.PatientsRow containing all LIS Order Tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Public Function UpdateCreationOrderForOpenLISOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLstOpenLISOTs As List(Of WorkSessionResultDS.PatientsRow)) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Const maxUpdates As Integer = 500
                    For Each orderTestRow As WorkSessionResultDS.PatientsRow In pLstOpenLISOTs
                        cmdText.Append(" UPDATE twksOrderTests SET CreationOrder = ")
                        cmdText.AppendFormat("{0}", orderTestRow.CreationOrder)
                        cmdText.Append(" WHERE  OrderTestID = ")
                        cmdText.AppendFormat("{0}", orderTestRow.OrderTestID)
                        cmdText.Append(vbCrLf)

                        i += 1
                        If (i = maxUpdates) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not myGlobalDataTO.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Updates...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateCreationOrderForOpenLISOTs", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO REVIEW - DELETE?"
        ''' <summary>
        ''' Get TestID and SampleType of an specific Order Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by : DL 19/02/2010
        ''' Modified by: AG 25/02/2010 - Return also OrderID, TestType, ReplicatesNumber
        '''              SA 14/06/2010 - Catch area was bad written (it had Throw ex and not inform the Error fields in GlobalDataTO) 
        '''                              PENDING: WHY THIS METHOD IS USED INSTEAD THE READ??
        ''' </remarks>
        Public Function GetTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim mytwksOrderTestDS As New OrderTestsDS

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += " SELECT TestID, SampleType, OrderID, TestType, ReplicatesNumber, OrderID "
                        cmdText += " FROM   twksOrderTests "
                        cmdText += " WHERE  OrderTestID = " & pOrderTestID

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(mytwksOrderTestDS.twksOrderTests)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytwksOrderTestDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mytwksOrderTestDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = False
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksOrderTestsDAO.GetTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

