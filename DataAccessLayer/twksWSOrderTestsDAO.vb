Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSOrderTestsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Adds a group of Order Tests to a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWSOrderTests">Dataset with structure of table twksWSOrderTests</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/04/2010 - Added new field OpenOTFlag to the Insert sentence
        '''              SA 17/02/2011 - Changed the implementation to sent groups of INSERTS instead of one by one
        '''              RH 18/04/2011 - Some optimizations for speed up processing velocity.
        '''                              Remove unneeded type conversions and object creations
        '''              SA 01/02/2012 - Manage the DB Connection with Using statement; clear the StringBuilder instead create a new one
        '''              TR 19/07/2012 - Added new file CtrlsSendingGroup used on QC.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTests As WSOrderTestsDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()

                ElseIf (Not pWSOrderTests Is Nothing) Then
                    Dim myGlobalBase As New GlobalBase
                    Dim myUserName As String = GlobalBase.GetSessionInfo().UserName().Trim

                    Dim i As Integer = 0
                    Dim maxInserts As Integer = 500

                    For Each row As WSOrderTestsDS.twksWSOrderTestsRow In pWSOrderTests.twksWSOrderTests
                        If (row.IsTS_UserNull) Then row.TS_User = myUserName
                        If (row.IsTS_DateTimeNull) Then row.TS_DateTime = DateTime.Now

                        cmdText.Append("INSERT INTO twksWSOrderTests")
                        cmdText.Append("(WorkSessionID, OrderTestID, ToSendFlag, OpenOTFlag, TS_User, TS_DateTime, CtrlsSendingGroup) VALUES(")
                        cmdText.AppendFormat("'{0}', {1}, {2}, {3}, N'{4}', '{5:yyyyMMdd HH:mm:ss}' ", _
                                             row.WorkSessionID.Trim, row.OrderTestID, _
                                             IIf(row.ToSendFlag, 1, 0), IIf(row.OpenOTFlag, 1, 0), row.TS_User.Trim.Replace("'", "''"), _
                                             row.TS_DateTime)

                        If row.IsCtrlsSendingGroupNull Then
                            cmdText.Append(", NULL)" & Environment.NewLine)
                        Else
                            cmdText.Append(", " & row.CtrlsSendingGroup.ToString & ") " & Environment.NewLine)
                        End If

                        'Increment the sentences counter and verify if the max has been reached
                        i += 1
                        If (i = maxInserts) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not dataToReturn.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Inserts...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete all Order Tests linked to the specified WorkSession and that belong to the specified PATIENT Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' Modified by: SA 01/02/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pOrderID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSOrderTests " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksOrderTests OT " & vbCrLf & _
                                                                   " WHERE  OT.OrderID = '" & pOrderID & "') " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.DeleteByOrderID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the informed Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestID">Order Identifier</param>
        ''' <returns>Global object containing Succes/Error information </returns>
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
                    Dim cmdText As String = " DELETE twksWSOrderTests " & vbCrLf & _
                                            " WHERE  OrderTestID = '" & pOrderTestID.ToString & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.DeleteByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified WorkSession, delete all Order Tests that are not included in the list of 
        ''' informed Order Test IDs - This function does work for deleted OffSystem Order Tests having results 
        ''' due to their OrderTestStatus is not OPEN
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the WorkSession</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010
        ''' Modified by: SA 01/02/2012 - Changed the function template
        '''              SA 05/02/2014 - BT #1491 ==> Changed the SQL sentence to exclude from the DELETE the Order Tests requested by LIS
        ''' </remarks>
        Public Function DeleteNotInList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pOrderID As String, ByVal pOrderTestsList As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSOrderTests " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OrderTestID FROM twksOrderTests " & vbCrLf & _
                                                                   " WHERE  OrderID = '" & pOrderID.Trim & "' " & vbCrLf & _
                                                                   " AND    OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                                   " AND    OrderTestID NOT IN (" & pOrderTestsList.Trim & ") " & vbCrLf & _
                                                                   " AND   (LISRequest IS NULL OR LISRequest = 0)) " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.DeleteNotInList", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Order Tests with status OPEN linked to the specified WorkSession and belonging to Orders 
        ''' of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010
        ''' Modified by: SA 01/02/2012 - Changed the function template
        '''              SA 14/03/2014 - BT #1542 ==> Changed the SQL sentence to exclude from the DELETE the Order Tests requested by LIS 
        '''                                           This change should have been done in BT #1491, but it was forgotten by error
        ''' </remarks>
        Public Function DeleteWSOpenOTsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                     ByVal pSampleClass As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSOrderTests " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                            " AND    OpenOTFlag = 1 " & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                   " WHERE  OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                                   " AND    O.SampleClass = '" & pSampleClass & "' " & vbCrLf & _
                                                                   " AND   (LISRequest IS NULL OR LISRequest = 0)) " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.DeleteWSOpenOTsBySampleClass", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read data of the specified Order Test included in the informed Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSOrderTests containing data of the specified Order Test included
        '''          in the informed Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010
        ''' Modified by: SA 02/09/2011 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSOrderTests " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestID = " & pOrderTestID

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
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update flags for an Open Order Test linked to a Work Session when:
        ''' ** It is sent to positioning in the Analyzer Rotor: OpenOTFlag = False and ToSendFlag = True
        ''' ** It was added to the WS reusing a previous result, but now has been selected as NEW
        '''       *** If besides it is sent to positioning in the Analyzer Rotor: OpenOTFlag = False and ToSendFlag = True
        '''       *** If it is not still positioning (WS is just saved): OpenOTFlag = True and ToSendFlag = True  
        ''' For Controls, field CtrlSendingGroup is also updated. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSOrderTests">Typed DataSet WSOrderTestsDS containing the list of WorkSesionID/OrderTestID to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010
        ''' Modified by: SA 01/09/2010 - Name changed from UpdateOpenOTFlag to UpdateWSOTFlag; added new boolean parameters 
        '''                              to inform values to set for flags OpenOTFlag and ToSendFlag
        '''              SA 01/02/2012 - Changed the function template
        '''              TR 19/07/2012 - Added the new column CtrlsSendingGroup to update 
        '''              SA 24/02/2014 - BT #1528 ==> Changes to receive all data to update in a DS, prepare all the updates and send them
        '''                                           in blocks of 500. To improve the performance of function AddWorkSession 
        ''' </remarks>
        Public Function UpdateWSOTFlags(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTests As WSOrderTestsDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()

                ElseIf (Not pWSOrderTests Is Nothing) Then
                    Dim myGlobalBase As New GlobalBase
                    Dim myUserName As String = GlobalBase.GetSessionInfo().UserName().Trim

                    Dim i As Integer = 0
                    Dim maxUpdates As Integer = 500

                    For Each row As WSOrderTestsDS.twksWSOrderTestsRow In pWSOrderTests.twksWSOrderTests
                        cmdText.Append("UPDATE twksWSOrderTests SET ")
                        cmdText.AppendFormat("  OpenOTFlag = {0}", IIf(row.OpenOTFlag, 1, 0).ToString)
                        cmdText.AppendFormat(", ToSendFlag = {0}", IIf(row.ToSendFlag, 1, 0).ToString)
                        If (Not row.IsCtrlsSendingGroupNull) Then cmdText.AppendFormat(", CtrlsSendingGroup = {0}", row.CtrlsSendingGroup.ToString)

                        If (row.IsTS_UserNull) Then
                            cmdText.AppendFormat(", TS_User = N'{0}'", myUserName.Replace("'", "''"))
                        Else
                            cmdText.AppendFormat(", TS_User = N'{0}'", row.TS_User.Replace("'", "''"))
                        End If

                        If (row.IsTS_DateTimeNull) Then
                            cmdText.AppendFormat(", TS_DateTime = '{0}'", Now.ToString("yyyyMMdd HH:mm:ss"))
                        Else
                            cmdText.AppendFormat(", TS_DateTime = '{0}'", row.TS_DateTime.ToString("yyyyMMdd HH:mm:ss"))
                        End If

                        cmdText.AppendFormat(" WHERE WorkSessionID = '{0}'", row.WorkSessionID.Trim)
                        cmdText.AppendFormat(" AND   OrderTestID   =  {0}", row.OrderTestID)
                        cmdText.Append(vbCrLf)

                        'Increment the sentences counter and verify if the max has been reached
                        i += 1
                        If (i = maxUpdates) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not dataToReturn.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Updates...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateOpenOTFlag", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Counts the number of Order Tests with status different of CLOSED for the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOFFSOrderTest">Optional parameter. When TRUE, it indicates that the Order Test is a request for an OffSystem Test</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests in the specified Order that are still not closed</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 25/08/2010 - Return error in the GlobalDataTO instead of Throw ex; missing the New in declaration of resultData variable
        '''              SA 01/09/2010 - Moved here from twksOrderTestsDAO
        '''              SA 18/02/2011 - Removed parameter pOrderTestID; added parameter pOrderID. Changed the returned type from DS to an integer 
        '''                              value; changed the SQL
        '''              SA 01/02/2012 - Changed the function template 
        '''              SA 16/07/2012 - Excluded from the COUNT all Order Tests that have not been sent to positioning:
        '''                              ** ToSendFlag = False -> reused Blanks or Calibrators 
        '''                              ** OpenOTFlags = True -> Patients, Ctrls, Calibs and/or Blanks not selected to positioning
        '''              TR 28/06/2013 - Added parameter pOFFSOrderTest. When value of this parameter is TRUE, it means the Order Test is a request of an
        '''                              OffSystem Test, in which case the filters by OpenOTFlag=0 and ToSendFlag=1 have to be ignored.
        ''' </remarks>
        Public Function CountClosedOTsByOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                              ByVal pOrderID As String, Optional pOFFSOrderTest As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumOrderTestsNotClosed " & vbCrLf & _
                                                " FROM   vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  OrderTestStatus <> 'CLOSED' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    OrderID       = '" & pOrderID.Trim & "' " & vbCrLf

                        If (Not pOFFSOrderTest) Then
                            cmdText &= " AND    OpenOTFlag    = 0 " & vbCrLf & _
                                       " AND    ToSendFlag    = 1 " & vbCrLf
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.CountClosedOTsByOrder", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Counts the number of Order Tests with status different of CLOSED in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOFFSOrderTest">Optional parameter. When TRUE, it indicates that the Order Test is a request for an OffSystem Test</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests in the specified Work Session that are 
        '''          still not closed</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 25/08/2010 - Return error in the GlobalDataTO instead of Throw ex; missing the New in declaration of resultData variable
        '''              SA 01/09/2010 - Moved here from twksOrderTestsDAO
        '''              SA 01/02/2012 - Changed the function template
        '''              SA 16/07/2012 - Excluded from the COUNT all Order Tests that have not been sent to positioning:
        '''                              ** ToSendFlag = False -> reused Blanks or Calibrators 
        '''                              ** OpenOTFlags = True -> Patients, Ctrls, Calibs and/or Blanks not selected to positioning.
        '''              TR 28/06/2013 - Added parameter pOFFSOrderTest. When value of this parameter is TRUE, it means the Order Test is a request of an
        '''                              OffSystem Test, in which case the filters by OpenOTFlag=0 and ToSendFlag=1 have to be ignored.
        ''' </remarks>
        Public Function CountClosedOTsByWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                           ByVal pAnalyzerID As String, Optional pOFFSOrderTest As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumNotClosed " & vbCrLf & _
                                                " FROM   vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  OrderTestStatus <> 'CLOSED' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID = '" & pAnalyzerID.Trim & "' "
                        If (Not pOFFSOrderTest) Then
                            cmdText &= " AND    OpenOTFlag   = 0 " & vbCrLf & _
                                       " AND    ToSendFlag    = 1 " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountClosedOTsByWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count the number of Controls or Patients that have been requested by LIS (value of field LISRequest is TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing an integer value with the number of LIS Request </returns>
        ''' <remarks>
        ''' Created by:  DL 07/05/2013
        ''' </remarks>
        Public Function CountLISRequestActive(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(OT.LISRequest) AS NumLISRequest " & vbCrLf & _
                                                " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE (O.SampleClass = 'PATIENT' OR O.SampleClass = 'CTRL') " & vbCrLf & _
                                                " AND   (OT.AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    OT.LISRequest = 1) "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()

                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 0
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NumLISRequest"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountLISRequestActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count the number of Order Tests in the specified WorkSession and related with the specified required Reagent, that have been 
        ''' sent to positioning but for which the Executions have not been still created (this can happen only when new Standard or ISE 
        ''' Tests are requested in the WorkSession and sent to positioning, but button Create Executions in Rotor Positioning Screen has 
        ''' not been still pressed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests without Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 27/02/2012
        ''' Modified by: SA 06/03/2012 - Added parameter for the ElementID. Changed the query to count only OrderTestIDs related
        '''                              with the informed required Element
        ''' </remarks>
        Public Function CountOTsWithoutExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumWithNoExec " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksWSRequiredElemByOrderTest WSREOT " & vbCrLf & _
                                                                                     " ON WSOT.OrderTestID = WSREOT.OrderTestID " & vbCrLf & _
                                                                                    " AND WSREOT.ElementID = " & pElementID.ToString & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
                                                " AND    WSOT.OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions " & vbCrLf & _
                                                                                 " WHERE WorkSessionID = '" & pWorkSessionID & "') " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountOTsWithoutExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Counts the number of Patient Samples in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the total number of Patient Samples requested in the active WS</returns>
        ''' <remarks>
        ''' Created by:  DL 01/09/2011
        ''' Modified by: SA 01/02/2012 - Changed the function template
        ''' </remarks>
        Public Function CountPatientOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumPatientSamples " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID AND O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.CountPatientOrderTests ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if for the informed PatientID/SampleType (or OrderID/SampleType when the patient is unknown) there is 
        ''' at least an Order Test that corresponds to an Stat Order included in the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <param name="pSampleID">Sample Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCountNotPositioned">Optional parameter to indicate if the Stat Patient Samples to count are the ones corresponding
        '''                                   to OrderTestIDs still not sent to positioning in the Analyzer Rotor (default value) or the ones 
        '''                                   already sent</param>
        ''' <returns>True if there is an Stat Order Test requested for the Patient in the Work Session; otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: AG 23/11/2009 - Adapt code to template (Tested: OK 26/11/2009)
        '''              SA 11/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''                              Function moved to "Other Methods" Region, and Region "For testing-Provisional" was removed
        '''              SA 09/03/2010 - Added parameter for the SampleID
        '''              SA 26/04/2010 - Filter data to exclude Order Tests linked to the WorkSession but still not sent to the Analyzer Rotor
        '''              SA 13/10/2010 - Changed OpenOTFlag=0 by OpenOTFlag=1; changed query to ANSI format 
        '''              SA 04/11/2010 - Added N preffix for multilanguage of fields PatientID and SampleID
        '''              SA 14/02/2011 - Added optional parameter pCountNotPositioned to indicate if the Stat Patient Samples to count are
        '''                              the ones corresponding to OrderTestIDs still not sent to positioning in the Analyzer Rotor (default
        '''                              value) or the ones already sent
        '''              SA 02/09/2011 - Changed the function template
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function ExistStatPatientSampleInWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   ByVal pPatientID As String, ByVal pSampleID As String, ByVal pOrderID As String, _
                                                   ByVal pSampleType As String, Optional ByVal pCountNotPositioned As Boolean = True) _
                                                   As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumStats " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag      = " & Convert.ToInt32(IIf(pCountNotPositioned, 1, 0)) & vbCrLf & _
                                                " AND    UPPER(OT.SampleType) = UPPER('" & pSampleType.Trim & "') " & vbCrLf & _
                                                " AND    O.SampleClass        = 'PATIENT' " & vbCrLf & _
                                                " AND    O.StatFlag           = 1 " & vbCrLf

                        If (pPatientID.Trim <> "") Then
                            'cmdText &= " AND UPPER(O.PatientID) = N'" & pPatientID.ToUpper.Trim.Replace("'", "''") & "' " & vbCrLf
                            cmdText &= " AND O.PatientID = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf
                        ElseIf (pSampleID.Trim <> "") Then
                            'cmdText &= " AND UPPER(O.SampleID) = N'" & pSampleID.ToUpper.Trim.Replace("'", "''") & "' " & vbCrLf
                            cmdText &= " AND O.SampleID = N'" & pSampleID.Trim.Replace("'", "''") & "' " & vbCrLf
                        ElseIf (pOrderID.Trim <> "") Then
                            cmdText &= " AND O.OrderID = '" & pOrderID.Trim & "' " & vbCrLf
                        End If

                        'Init to false the boolean flag
                        Dim existStatOrder As Boolean = False
                        dataToReturn.SetDatos = existStatOrder

                        'Execute the SQL Query
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                existStatOrder = CInt(dbDataReader.Item("NumStats")) > 0
                                dataToReturn.SetDatos = existStatOrder
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.ExistStatPatientSampleInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' For the informed STD TestID and SampleType, search all requested Blank and Calibrator Order Tests that have to be positioned (ToSendFlag = TRUE) 
        ''' but that have not been selected yet for it (OpenOTFlag = TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Standard Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>Global object containing a typed DataSet WSOrderTestsDS with the list of Blank and Calibrator Order Tests for the specified STD
        '''          Test and SampleType and having OpenOTFlag = TRUE (they have been not selected to send to positioning)</returns>
        ''' <remarks>
        ''' Created by: SA 10/04/2013
        ''' </remarks>
        Public Function GetBlankAndCalibByTestAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pTestID As Integer, _
                                                            ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.WorkSessionID, WSOT.OrderTestID, WSOT.ToSendFlag, 0 AS OpenOTFlag " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag    = 1 " & vbCrLf & _
                                                " AND    O.SampleClass      = 'BLANK' " & vbCrLf & _
                                                " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                " AND    OT.TestID          = " & pTestID.ToString & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT WSOT.WorkSessionID, WSOT.OrderTestID, WSOT.ToSendFlag, 0 AS OpenOTFlag " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag    = 1 " & vbCrLf & _
                                                " AND    O.SampleClass      = 'CALIB' " & vbCrLf & _
                                                " AND    OT.TestType        = 'STD' " & vbCrLf & _
                                                " AND    OT.TestID          = " & pTestID.ToString & vbCrLf & _
                                                " AND    OT.SampleType      = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    OT.AlternativeOrderTestID IS NULL " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT WSOT.WorkSessionID, WSOT.OrderTestID, WSOT.ToSendFlag, 0 AS OpenOTFlag " & vbCrLf & _
                                                " FROM   twksOrderTests OT1 INNER JOIN twksOrderTests OT2 ON OT1.AlternativeOrderTestID = OT2.OrderTestID " & vbCrLf & _
                                                                          " INNER JOIN twksWSOrderTests WSOT ON OT2.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                          " INNER JOIN twksOrders O ON OT1.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag    = 1 " & vbCrLf & _
                                                " AND    O.SampleClass      = 'CALIB' " & vbCrLf & _
                                                " AND    OT1.TestType        = 'STD' " & vbCrLf & _
                                                " AND    OT1.TestID          = " & pTestID.ToString & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetBlankAndCalibByTestAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the informed PatientID/SampleID and SampleType, search all requested Order Tests that have to be positioned (ToSendFlag = TRUE) 
        ''' but that have not been selected yet for it (OpenOTFlag = TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient or Sample Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>Global object containing a typed DataSet WSOrderTestsDS with the list of Order Tests requested for the specified Patient 
        '''          and SampleType and having OpenOTFlag = TRUE (they have been not selected to send to positioning)</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 17/04/2013 - Deleted parameter pPatientExists. Changed the SQL Query to filter twksOrders table by PatientID OR SampleID equal
        '''                              to the value specified by parameter pPatientID
        ''' </remarks>
        Public Function GetByPatientAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pPatientID As String, _
                                                  ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.WorkSessionID, WSOT.OrderTestID, WSOT.ToSendFlag, 0 AS OpenOTFlag, OT.TestType, OT.TestID " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag    = 1 " & vbCrLf & _
                                                " AND    OT.SampleType      = '" & pSampleType.Trim & "'" & vbCrLf & _
                                                " AND    O.SampleClass      = 'PATIENT' " & vbCrLf & _
                                                " AND   (O.PatientID = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " OR     O.SampleID = N'" & pPatientID.Trim.Replace("'", "''") & "') " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetByPatientAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified WorkSession and optionally, for the specified Order, get all OFF-SYSTEM Order Tests with status CLOSED 
        ''' (those having a result). Optionally, get only those OFF-SYSTEM Order Tests that are not included in the list of informed
        '''  OrderTestIDs 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier. Optional parameter</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the Order. Optional parameter</param>
        ''' <returns>Global object containing a typed DataSet WSOrderTestsDS with the list of obtained OFF-SYSTEM OrderTests</returns>
        ''' <remarks>
        ''' Created by:  SA 20/01/2011
        ''' Modified by: SA 02/09/2011 - Changed the function template
        '''              SA 05/02/2014 - BT #1491 ==> When optional parameter for the list of Order Test IDs that should remain in the Order is informed, added a new condition 
        '''                                           to exclude the Order Tests requested by LIS (due to they should not be deleted)
        ''' </remarks>
        Public Function GetClosedOffSystemOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              Optional ByVal pOrderID As String = "", Optional ByVal pOrderTestsList As String = "") _
                                              As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.OrderTestID " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    OT.TestType = 'OFFS' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus = 'CLOSED'" & vbCrLf

                        'Add filters by optional parameters when they are informed
                        If (pOrderID.Trim <> "") Then cmdText &= " AND OT.OrderID = '" & pOrderID & "' " & vbCrLf
                        If (pOrderTestsList.Trim <> "") Then cmdText &= " AND  WSOT.OrderTestID NOT IN (" & pOrderTestsList & ") " & vbCrLf & _
                                                                        " AND (OT.LISRequest IS NULL OR LISRequest = 0) "

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetClosedOffSystemOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all information needed to create the executions of all Order Tests to be executed in the 
        ''' informed Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsForExecutionsDS with all information of the 
        '''          Order Tests to be executed in the active Analyzer Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2011
        ''' Modified by: SA 09/11/2011 - Get also field CreationOrder for the returned Order Tests; changed the function template
        '''              AG 10/04/2012 - Changed the query because Controls are duplicated when a Test uses two Control levels
        '''              AG 17/04/2012 - Changed the query to get information of ISE Order Tests from an specific subquery   
        '''              SA 19/06/2012 - Changed the subquery for ISE Order Tests to get the ControlID as ElementID when the OrderTest
        '''                              belongs to a CTRL Order
        ''' </remarks>
        Public Function GetInfoOrderTestsForExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                       ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT WSOT.TestType, WSOT.TestID, WSOT.SampleType, WSOT.SampleClass, WSOT.OrderID, " & vbCrLf & _
                                                                " WSOT.StatFlag, WSOT.OrderTestID, WSOT.AlternativeOrderTestID, WSOT.CreationOrder, TR.ReagentID, " & vbCrLf & _
                                                               " (CASE WHEN T.FirstReadingCycle < T.SecondReadingCycle THEN T.SecondReadingCycle " & vbCrLf & _
                                                                     " ELSE T.FirstReadingCycle END) AS ReadingCycle, " & vbCrLf & _
                                                               " (CASE WSOT.SampleClass WHEN 'CALIB' THEN C.CalibratorID " & vbCrLf & _
                                                                                      " WHEN 'CTRL'  THEN WSOT.ControlID ELSE 0 END) AS ElementID " & vbCrLf & _
                                                " FROM vwksWSOrderTests WSOT INNER JOIN tparTests T ON WSOT.TestID = T.TestID " & vbCrLf & _
                                                                           " INNER JOIN tparTestReagents TR ON WSOT.TestID = TR.TestID " & vbCrLf & _
                                                                           " LEFT OUTER JOIN tparTestCalibrators C ON WSOT.TestID = C.TestID AND WSOT.SampleType = C.SampleType " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   (WSOT.ToSendFlag = 1 OR WSOT.AlternativeOrderTestID IS NOT NULL) " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
                                                " AND    TR.ReagentNumber = 1 " & vbCrLf & _
                                                " AND    WSOT.TestType = 'STD' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT DISTINCT WSOT.TestType, WSOT.TestID, WSOT.SampleType, WSOT.SampleClass, WSOT.OrderID, " & vbCrLf & _
                                                                " WSOT.StatFlag, WSOT.OrderTestID, WSOT.AlternativeOrderTestID, WSOT.CreationOrder, 0 AS ReagentID, " & vbCrLf & _
                                                                " 0 AS ReadingCycle, " & vbCrLf & _
                                                                " (CASE WSOT.SampleClass WHEN 'CTRL' THEN WSOT.ControlID ELSE 0 END) AS ElementID " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT INNER JOIN tparISETests IT ON WSOT.TestID = IT.ISETestID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag = 0 " & vbCrLf & _
                                                " AND    WSOT.TestType = 'ISE' " & vbCrLf

                        Dim myOrderTestsDS As New OrderTestsForExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.OrderTestsForExecutionsTable)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetInfoOrderTestsForExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer WorkSession get the maximum CtrlsSendingGroup created. If there is not any CtrlsSendingGroup
        ''' then it returns zero
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test. Optional parameter</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing an integer value with the maximum CtrlsSendingGroup in the specified
        '''          Analyzer WorkSession. If there is not any CtrlsSendingGroup, then it contains zero</returns>
        ''' <remarks>
        ''' Created by:  SA 10/07/2012 
        ''' Modified by: JC 14/06/2013 - Added optionals parameters for TestID and SampleType. When these parameters are informed, the maximum
        '''                              CtrlsSendingGroup is searched for the specified Standard Test/SampleType (the change is to group controls
        '''                              properly when an automatic rerun is launched for the Test/Sample Type and each one of its linked controls)
        ''' </remarks>
        Public Function GetMaxCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                Optional pTestID As Integer = 0, Optional pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If (pTestID > 0 AndAlso Not String.IsNullOrEmpty(pSampleType)) Then
                            cmdText = "  SELECT MAX(WOT.CtrlsSendingGroup) AS NextCtrlsSendingGroup   "
                            cmdText &= " FROM   twksWSOrderTests WOT INNER JOIN twksOrderTests OT ON WOT.OrderTestID = OT.OrderTestID "
                            cmdText &= " WHERE  WOT.WorkSessionID = '" & pWorkSessionID.Trim & "' "
                            cmdText &= " AND    WOT.CtrlsSendingGroup IS NOT NULL "
                            cmdText &= " AND    OT.TestType = 'STD' "
                            cmdText &= " AND    OT.TestID = " & pTestID.ToString
                            cmdText &= " AND    OT.SampleType = '" & pSampleType.Trim & "' "
                        Else
                            cmdText = " SELECT MAX(CtrlsSendingGroup) AS NextCtrlsSendingGroup " & vbCrLf & _
                                      " FROM   twksWSOrderTests " & vbCrLf & _
                                      " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                      " AND    CtrlsSendingGroup IS NOT NULL " & vbCrLf
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 0
                                Else
                                    resultData.SetDatos = CInt(dbDataReader.Item("NextCtrlsSendingGroup"))
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetMaxCtrlsSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Orders of the specified SampleClass currently linked to the specified WorkSession but that are not in the informed list of 
        ''' Orders that remains in the Work Session 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pListOfWSOrders">List of IDs of Orders included in the specified Work Session</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet with the list of Orders that have to be removed from
        '''          the WorkSession and physically deleted with all their Order Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' Modified by: SA 01/02/2012 - Added parameter for the SampleClass; function name changed to GetOrdersNotInWS due to now the 
        '''                              it is used for all different Sample Classes; changed the function template 
        '''              SA 05/02/2014 - BT #1491 ==> Changed the SQL sentence to exclude from the query Orders having all Order Tests requested by LIS
        ''' </remarks>
        Public Function GetOrdersNotInWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pListOfWSOrders As String, _
                                         ByVal pSampleClass As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OT.OrderID FROM twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                      " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    O.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    OT.OrderID NOT IN (" & pListOfWSOrders & ") " & vbCrLf & _
                                                " AND   (OT.LISRequest IS NULL OR OT.LISRequest = 0) " & vbCrLf

                        'Dim cmdText As String = " SELECT DISTINCT OrderID FROM vwksWSOrderTests " & vbCrLf & _
                        '                        " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                        " AND    SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                        '                        " AND    OrderID NOT IN (" & pListOfWSOrders & ")"

                        Dim myWSOrdersDS As New OrdersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSOrdersDS.twksOrders)
                            End Using
                        End Using

                        resultData.SetDatos = myWSOrdersDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetOrdersNotInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all SampleTypes currently in use in the WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a String List with all the SampleType Codes in use in the active WS</returns>
        ''' <remarks>
        ''' Created by: SG 08/04/2013
        ''' </remarks>
        Public Function GetSampleTypesInWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim mySampleTypes As New List(Of String)
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDataTable As New DataTable
                        myDataTable.Columns.Add("SampleType", Type.GetType("System.String"))

                        Dim cmdText As String = " SELECT DISTINCT SampleType AS SampleType " & vbCrLf & _
                                                " FROM twksOrderTests INNER JOIN twksWSOrderTests ON twksOrderTests.OrderTestID = twksWSOrderTests.OrderTestID "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataTable)
                            End Using
                        End Using

                        If (myDataTable IsNot Nothing) Then
                            For Each dr As DataRow In myDataTable.Rows
                                mySampleTypes.Add(CStr(dr(0)))
                            Next
                        End If
                        resultData.SetDatos = mySampleTypes
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetSampleTypesInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for each OrderTests included in the informed WorkSession and having the specified SampleClass, but
        ''' only for those Order Tests without Executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pOrderTestID">Order Test Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSOrderTestsForExecutions with the list of Order Tests and required Elements by Order Tests
        '''          included in the WorkSession and corresponding to the informed Sample Class</returns>
        ''' <remarks>
        ''' Created by:  SA 20/04/2010
        ''' Modified by: DL 11/05/2010 - Added additional condition by OrderTestID
        '''              SA 25/10/2010 - Get also field TestType from view vwksWSOrderTests
        '''              AG 20/06/2011 - Besides the tube with the Sample Type, Patient Samples can need the tube containing the dilution solution for
        '''                              standard Tests (SPEC_SOL), the tube containing the washing solution needed to avoid contaminations (WASH_SOL), 
        '''                              and/or the dilution solution for ISE Tests (TUBE_WASH_SOL)
        '''              SA 02/09/2011 - Changed the function template 
        '''              AG 20/09/2011 - When SampleClass is not Blank get also the required Reagents
        '''              SA 05/07/2012 - Changes in the subquery used to exclude OrderTests with Executions already created: add DISTINCT clause and 
        '''                              add a filter by SampleClass
        ''' </remarks>
        Public Function GetOrderTestsForExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pSampleClass As String, Optional ByVal pOrderTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.WorkSessionID, WSOT.AnalyzerID, WSOT.OrderTestID, WSOT.SampleClass, WSOT.ReplicatesNumber, " & vbCrLf & _
                                                       " WSOT.TestID, WSOT.TestType, WSOT.SampleType, REOT.StatFlag, RE.ElementID,  " & vbCrLf & _
                                                       " RE.MultiItemNumber, RE.ElementStatus, RE.TubeContent " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT INNER JOIN twksWSRequiredElemByOrderTest REOT ON  WSOT.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksWSRequiredElements RE ON REOT.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.AnalyzerID    = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass   = '" & pSampleClass.Trim & "' " & vbCrLf

                        'Add optional filters
                        'When Calib, Ctrl or Patient get also the required Reagent (session without blanks)
                        If (pSampleClass <> "BLANK") Then cmdText &= " AND (RE.TubeContent = 'REAGENT' OR RE.TubeContent = '" & pSampleClass & "' " & vbCrLf

                        'Patients need the Patient Sample Tube and perhaps the Special Solution Tube for Dilutions ('SPEC_SOL'), the Washing Solution 
                        'Tube for STD Tests Contaminations ('WASH_SOL') or the Washing Solution Tube for ISE Tests ('TUBE_WASH_SOL')
                        If (pSampleClass = "PATIENT") Then
                            cmdText &= " OR RE.TubeContent = 'SPEC_SOL' OR RE.TubeContent = 'WASH_SOL' OR RE.TubeContent = 'TUBE_WASH_SOL') " & vbCrLf
                        ElseIf (pSampleClass <> "BLANK") Then 'CALIB and CTRL
                            cmdText &= " OR RE.TubeContent = 'WASH_SOL') " & vbCrLf
                        End If

                        If (pOrderTestID = -1) Then
                            cmdText &= " AND WSOT.OrderTestID NOT IN (SELECT DISTINCT OrderTestID FROM twksWSExecutions " & vbCrLf & _
                                                                    " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                    " AND    SampleClass   = '" & pSampleClass.Trim & "') " & vbCrLf
                        Else
                            'Filter by an specific Order Test 
                            cmdText &= " AND WSOT.OrderTestID = " & pOrderTestID & vbCrLf
                        End If
                        cmdText &= " ORDER BY WSOT.CreationOrder, RE.MultiItemNumber " & vbCrLf

                        Dim orderTestsForExecDS As New WSOrderTestsForExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(orderTestsForExecDS.WSOrderTestsForExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = orderTestsForExecDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetOrderTestsForExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the WorkSessionID and the AnalyzerID in which the specified Order Test is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test ID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestWSAnalyzerDS with the obtained
        '''          WorkSessionID, AnalyzerID and OrderID</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 25/08/2010 - Return error in the GlobalDataTO instead of Throw ex; missing the
        '''                              New in declaration of resultData variable
        '''              SA 01/09/2010 - Moved here from twksOrderTestsDAO
        '''              SA 18/02/2011 - Get also the OrderID and OrderStatus for the specified OrderTestID
        '''              SA 01/02/2012 - Changed the function template
        '''              AG 22/05/2012 - Changed the query to get also field WSStatus from table twksWSAnalyzers
        '''              SA 25/02/2014 - BT #1521 ==> Changed the query to remove the use the needed tables instead of the 
        '''                                           view vwksWSOrderTests (for memory performance)
        ''' </remarks>
        Public Function GetWSAndAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim cmdText As String = " SELECT VW.WorkSessionID, VW.AnalyzerID, VW.OrderID, VW.OrderStatus, WS.WSStatus " & vbCrLf & _
                        '                        " FROM   vwksWSOrderTests VW INNER JOIN twksWSAnalyzers WS ON VW.WorkSessionID = WS.WorkSessionID " & vbCrLf & _
                        '                                                                                " AND VW.AnalyzerID = WS.AnalyzerID " & vbCrLf & _
                        '                        " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf

                        Dim cmdText As String = " SELECT WSOT.WorkSessionID, O.OrderID, O.OrderStatus, WSA.AnalyzerID, WSA.WSStatus " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT   ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O        ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                             " INNER JOIN twksWSAnalyzers WSA ON WSOT.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                                                " WHERE WSOT.OrderTestID = " & pOrderTestID.ToString & vbCrLf

                        Dim myOrderTestWSAnalyzerDS As New OrderTestWSAnalyzerDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestWSAnalyzerDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetWSAndAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' Modified by: SA 01/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSOrderTests" & vbCrLf & _
                                            " WHERE WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For an specific OrderTesrtID corresponding to a CTRL requested in the WorkSession, update value of field CtrlsSendingGroup
        ''' when a new Rerun is requested 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pNextCtrlsSendingGroup">Value of CtrlsSendingGroup to update for the specified OrderTestID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/07/2012
        ''' </remarks>
        Public Function UpdateCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer, _
                                                ByVal pNextCtrlsSendingGroup As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSOrderTests " & vbCrLf & _
                                            " SET    CtrlsSendingGroup = " & pNextCtrlsSendingGroup.ToString & vbCrLf & _
                                            " WHERE  WorkSessionID     = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    OrderTestID       = " & pOrderTestID.ToString

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateCtrlsSendingGroup", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the Calibrator for the specified TestID/SampleType or the Blank for the specified TestID has to be executed in the 
        ''' WorkSession (ToSendFlag=1). ToSendFlag=0 means a previous result will be used, then when executions for Control or Patient 
        ''' Samples requested for the TestID/SampleType are created it is not needed verifying if the Calibrator is positioned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code: CALIB or BLANK</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code; for SampleClass=BLANK it is not needed to inform it</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if it is needed to verify if the Calibrator for the specified 
        ''' TestID/SampleType or the Blank for the specified TestID has been positioned</returns>
        ''' <remarks>
        ''' Created by:  SA 01/09/2010
        ''' Modified by: SA 01/02/2012 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function VerifyToSendFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pSampleClass As String, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID FROM vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    SampleClass   = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    TestType      = 'STD' " & vbCrLf & _
                                                " AND    TestID        = " & pTestID & vbCrLf & _
                                                " AND    ToSendFlag    = 1 " & vbCrLf

                        If (pSampleClass = "CALIB") Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myWSOrderTestsDS As New WSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSOrderTestsDS.twksWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = (myWSOrderTestsDS.twksWSOrderTests.Rows.Count > 0)
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.VerifyToSendFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        'A new version of this function was created to improve performance of function AddWorkSession
        'Public Function UpdateWSOTFlags_OLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer, _
        '                                    ByVal pOpenOTFlag As Boolean, ByVal pToSendFlag As Boolean, Optional ByVal pCtrlSendingGroup As Integer = 0) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            cmdText = " UPDATE twksWSOrderTests " & vbCrLf & _
        '                      " SET    OpenOTFlag = " & Convert.ToInt32(IIf(pOpenOTFlag, 1, 0)) & ", " & vbCrLf & _
        '                      " ToSendFlag = " & Convert.ToInt32(IIf(pToSendFlag, 1, 0)) & vbCrLf

        '            If pCtrlSendingGroup > 0 Then
        '                cmdText &= ", CtrlsSendingGroup = " & pCtrlSendingGroup.ToString()
        '            End If

        '            cmdText &= " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
        '                                    " AND    OrderTestID   = " & pOrderTestID

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsDAO.UpdateOpenOTFlag_OLD", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the last OrderTestID related to the entry parameters (used to find the last blank or calibrator results)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <param name="pSampleClass">Sample Class Code</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>Global data to with data as OrderTestsDS</returns>
        '''' <remarks>
        '''' Created by:  AG 26/02/2010 (Tested PD)
        '''' Modified by: SA 26/04/2010 - Filter data to exclude Order Tests linked to the WorkSession but still not sent to the Analyzer Rotor.
        ''''                              Changed query to ANSI SQL
        '''' </remarks>
        'Public Function GetLastOrderTestIDBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                                ByVal pWorkSessionID As String, ByVal pSampleClass As String, ByVal pTestID As Integer, _
        '                                                ByVal pSampleType As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT DISTINCT OT.OrderTestID, OT.PreviousOrderTestID, O.OrderID " & vbCrLf & _
        '                                        " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
        '                                        " WHERE  OT.TestID = " & pTestID & vbCrLf & _
        '                                        " AND    OT.SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                                        " AND    O.SampleClass = '" & pSampleClass & "' " & vbCrLf & _
        '                                        " AND    OT.OrderTestID IN (SELECT OrderTestID " & vbCrLf & _
        '                                                                  " FROM   twksWSOrderTests B INNER JOIN twksWSAnalyzers A ON B.WorkSessionID = A.WorkSessionID " & vbCrLf & _
        '                                                                  " WHERE  B.WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf & _
        '                                                                  " AND    B.OpenOTFlag = 0 " & vbCrLf & _
        '                                                                  " AND    A.AnalyzerID = '" & pAnalyzerID & "') "

        '                Dim myOrderTestsDS As New OrderTestsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using da As New SqlClient.SqlDataAdapter(dbCmd)
        '                        da.Fill(myOrderTestsDS.twksOrderTests)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myOrderTestsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetLastOrderTestIDBySampleClass", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Order Tests included in the specified WorkSession
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSOrderTestsDS with the list of Order Tests included
        ''''          in the specified WorkSession</returns>
        '''' <remarks>
        '''' Created by:  TR 13/05/2010
        '''' Modified by: SA 01/02/2012 - Changed the function template
        '''' </remarks>
        'Public Function GetOrderTestByWorkSessionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing
        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM twksWSOrderTests " & vbCrLf & _
        '                                        " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

        '                Dim myWSOrderTestDS As New WSOrderTestsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myWSOrderTestDS.twksWSOrderTests)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myWSOrderTestDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSOrderTestsDAO.GetOrderTestByWorkSessionID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace




