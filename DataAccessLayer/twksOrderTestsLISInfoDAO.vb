Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksOrderTestsLISInfoDAO
          

#Region "Declarations"
        'Comparisons by fields SpecimenID and AwosID have to be done in a CASE SENSITIVE way. So this SQL Sentence has to
        'be added to some SQL Queries in this class
        Private caseSensitiveCollation As String = " COLLATE Modern_Spanish_CS_AS "
#End Region

#Region "CRUD"
        ''' <summary>
        ''' Add data to the auxiliary tabla used to store LIS fields for each OrderTestID/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS containing all data to add to the auxiliary
        '''                                     tabla used to store LIS fields for each OrderTestID/RerunNumber</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 26/03/2013
        ''' Modified by: SA 03/04/2013 - Changed the SQL QUERY due to fields AwosID and LISOrderID allow NULL values
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsLISInfoDS As OrderTestsLISInfoDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each pRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pOrderTestsLISInfoDS.twksOrderTestsLISInfo
                        cmdText.Append(" INSERT INTO twksOrderTestsLISInfo ")
                        cmdText.Append(" (OrderTestID, RerunNumber, SpecimenID, ESOrderID, ESPatientID, AwosID, LISOrderID, LISPatientID) ")
                        cmdText.Append(" VALUES (")
                        cmdText.Append(pRow.OrderTestID & ", ")
                        cmdText.Append(pRow.RerunNumber & ", ")
                        cmdText.Append("N'" & pRow.SpecimenID & "', ")
                        cmdText.Append("N'" & pRow.ESOrderID & "', ")
                        cmdText.Append("N'" & pRow.ESPatientID & "', ")

                        If (pRow.IsAwosIDNull) Then
                            cmdText.Append("NULL,")
                        Else
                            cmdText.Append("'" & pRow.AwosID & "', ")
                        End If

                        If (pRow.IsLISOrderIDNull) Then
                            cmdText.Append("NULL,")
                        Else
                            cmdText.Append("N'" & pRow.LISOrderID & "', ")
                        End If

                        If (pRow.IsLISPatientIDNull) Then
                            cmdText.Append("NULL)")
                        Else
                            cmdText.Append("N'" & pRow.LISPatientID & "' )")
                        End If
                    Next

                    If (Not myGlobalDataTO.HasError) Then
                        If (cmdText.Length > 0) Then
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete LIS fields for the specified OrderTestID/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 18/03/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " DELETE twksOrderTestsLISInfo " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber = " & pRerunNumber.ToString

                        Using cmd As New SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = cmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of AwosID that has been received from LIS for the existing Order Test, sorted by RerunNumber DESC
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with all fields sent by LIS for the specified OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID, RerunNumber, AwosID, SpecimenID, ESOrderID, LISOrderID, ESPatientID, LISPatientID " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " ORDER BY RerunNumber DESC "

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.GetByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all LIS fields for the specified OrderTestID/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with all LIS fields for the specified OrderTestID/RerunNumber</returns>
        ''' <remarks>
        ''' Created by:  AG 06/03/2013
        ''' Modified by: XB 08/03/2013 - Added pRerunNumber parameter
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID, RerunNumber, AwosID, SpecimenID, ESOrderID, LISOrderID, ESPatientID, LISPatientID " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber = " & pRerunNumber.ToString

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all records in table twksOrderTestsLISInfo
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with all data in table twksOrderTestsLISInfo</returns>
        ''' <remarks>
        ''' Created by:  AG 24/04/2013</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksOrderTestsLISInfo "

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update LIS information for the specified OrderTestID/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS containing the list of Order Tests LIS to update</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsLISInfoDS As OrderTestsLISInfoDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Const maxUpdates As Integer = 500
                    For Each pRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pOrderTestsLISInfoDS.twksOrderTestsLISInfo
                        cmdText.Append(" UPDATE twksOrderTestsLISInfo SET ")

                        cmdText.Append(" AwosID = ")
                        If (pRow.IsAwosIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("{0}", pRow.AwosID)
                        End If

                        cmdText.Append(", ESPatientID  = ")
                        If (pRow.IsESPatientIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", pRow.ESPatientID)
                        End If

                        cmdText.Append(", LISPatientID  = ")
                        If (pRow.IsLISPatientIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", pRow.LISPatientID)
                        End If

                        cmdText.Append(", ESOrderID  = ")
                        If (pRow.IsESOrderIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", pRow.ESOrderID)
                        End If

                        cmdText.Append(", LISOrderID  = ")
                        If (pRow.IsLISOrderIDNull) Then
                            cmdText.AppendFormat("{0}", "NULL")
                        Else
                            cmdText.AppendFormat("'{0}'", pRow.LISOrderID)
                        End If


                        cmdText.Append(" WHERE  OrderTestID = ")
                        cmdText.AppendFormat("{0}", pRow.OrderTestID)
                        cmdText.Append(" AND  RerunNumber  = ")
                        cmdText.AppendFormat("{0}", pRow.RerunNumber)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Delete all Order Tests with status OPEN  belonging to Orders of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <returns>GlobalDataTO containing Success/Error information </returns>
        ''' <remarks>
        ''' Created by:  XB 26/04/2013
        ''' Modified by: SA 14/03/2014 - BT #1542 ==> Changed the SQL sentence to exclude from the DELETE the Order Tests requested by LIS 
        '''                                           This change should have been made in BT #1491, but it was forgotten by error
        ''' </remarks>
        Public Function DeleteOpenOrderTestsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " DELETE twksOrderTestsLISInfo " & vbCrLf & _
                                                " WHERE  OrderTestID IN (SELECT OT.OrderTestID FROM twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                       " WHERE  OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                                       " AND    O.SampleClass = '" & pSampleClass & "' " & vbCrLf & _
                                                                       " AND   (LISRequest IS NULL OR LISRequest = 0)) " & vbCrLf

                        Using cmd As New SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = cmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.DeleteOpenOrderTestsBySampleClass", EventLogEntryType.Error, False)
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
        ''' Created by:  SA 09/04/2013
        ''' Modified by: SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by SpecimenID (Barcodes are case sensitive). This is 
        '''                              needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function GetOrderTestBySpecimenID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSpecimenID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT LI.OrderTestID, LI.SpecimenID, WOT.OpenOTFlag, OT.SampleType, O.SampleID, O.PatientID " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo LI INNER JOIN twksWSOrderTests WOT ON LI.OrderTestID = WOT.OrderTestID " & vbCrLf & _
                                                                                " INNER JOIN twksOrderTests OT    ON LI.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                " INNER JOIN twksOrders O         ON OT.OrderID     = O.OrderID " & vbCrLf & _
                                                " WHERE  LI.SpecimenID = N'" & pSpecimenID.Trim.Replace("'", "''") & "' " & caseSensitiveCollation & vbCrLf & _
                                                " ORDER BY OT.SampleType " & vbCrLf

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.GetOrderTestBySpecimenID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the specified AwosID exists in table twksOrderTestsLISInfo, and in this case, get data of the related Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAwosID">AWOS Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with the Order Test data of the specified AwosID</returns>
        ''' <remarks>
        ''' Created by:  SG 18/03/2013
        ''' Modified by: SA 04/04/2013 - Changed the SQL to get also field OrderID from table twksOrderTests
        '''              AG 08/05/2013 - Changed the SQL to get also field TestID from table twksOrderTests (required for update the InUse 
        '''                              field of the Test when needed)
        '''              SA 14/05/2013 - Changed the SQL by adding an INNER JOIN with table twksOrders to get also field SampleClass; get also field
        '''                              SampleType from table twksOrderTests
        '''              SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by AwosID (GUIDs have upper and lower case letters). This is 
        '''                              needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function GetOrderTestInfoByAwosID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT O.SampleClass, OT.OrderTestID, OT.OrderID, OT.TestType, OT.TestID, OT.SampleType, OT.OrderTestStatus, " & vbCrLf & _
                                                       " OTL.AwosID, OTL.RerunNumber " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo OTL INNER JOIN twksOrderTests OT ON OT.OrderTestID = OTL.OrderTestID " & vbCrLf & _
                                                                                 " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  OTL.AwosID = '" & pAwosID.Trim & "' " & caseSensitiveCollation

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.GetOrderTestInfoByAwosID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For each different PatientID-SampleType return the related SpecimenID's list
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsLISInfoDS with the list of SpecimenIDs received for each PatientID-SampleType</returns>
        ''' <remarks>
        ''' Created by: SGM 29/04/2013
        ''' </remarks>
        Public Function GetSpecimensByPatientSampleType(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT (CASE WHEN O.PatientID IS NULL THEN O.SampleID ELSE O.PatientID END) AS LISPatientID, " & vbCrLf & _
                                                       " OT.SampleType, OTL.SpecimenID " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo OTL INNER JOIN twksOrderTests OT ON OTL.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                 " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  O.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " ORDER BY LISPatientID, OT.SampleType "

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.GetSpecimensByPatientSampleType", EventLogEntryType.Error, False)
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
        ''' Created By: XB 22/04/2013
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderTestsLISInfo"

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.ResetWS", EventLogEntryType.Error, False)
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
                        Dim cmdText As String = " SELECT OT.SampleType, OTL.*  " & vbCrLf & _
                                                " FROM   twksOrderTestsLISInfo OTL INNER JOIN twksOrderTests OT ON OTL.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  OTL.LISPatientID = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " ORDER BY OT.SampleType "

                        Dim myDataSet As New OrderTestsLISInfoDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderTestsLISInfo)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderTestsLISInfoDAO.GetLISInfoByLISPatient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace