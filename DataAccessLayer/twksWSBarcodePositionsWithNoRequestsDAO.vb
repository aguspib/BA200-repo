Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class twksWSBarcodePositionsWithNoRequestsDAO
        Inherits DAOBase

#Region "Declarations"
        'Comparisons by field BarcodeInfo have to be done in a CASE SENSITIVE way. So this SQL Sentence has to
        'be added to some SQL Queries in this class
        Private caseSensitiveCollation As String = " COLLATE Modern_Spanish_CS_AS "
#End Region

#Region "CRUD Methods"

        ''' <summary>
        ''' After BarCode scanning, insert cells with incomplete data in table twksWSBarcodePositionsWithNoRequests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarcodePositionsWithNoRequestsDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing the information
        '''                                                 of the incomplete cells to add</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 03/08/2011
        ''' Modified by: SA 01/09/2011 - Insert also new field PatientID when it is informed
        '''              SA 19/09/2011 - When value of fields ExternalPID, PatientID and/or SampleType are String.Empty, insert them as NULL 
        '''              TR 21/09/2011 - Insert value for new table field NotSampleType: If SampleType IS NULL Then TRUE, Else FALSE
        '''            JCID 25/03/2013 - Insert value for new fields BarCodeInfo, Status
        '''              XB 13/03/2014 - Add .Replace("'", "''") protection against SQL Incorrect syntax near errors for Unclosed quotation mark after the character string '
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pBarcodePositionsWithNoRequestsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each bcPositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In pBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests
                        cmdText = "  INSERT INTO twksWSBarcodePositionsWithNoRequests "
                        cmdText &= "        (AnalyzerID, WorkSessionID, RotorType, CellNumber, ExternalPID, SampleType, PatientID, NotSampleType, BarCodeInfo, LISStatus, MessageId) "
                        cmdText &= " VALUES ("

                        'Inform required values
                        cmdText &= "  '" & bcPositionsWithNoRequestsRow.AnalyzerID & "' "
                        cmdText &= ", '" & bcPositionsWithNoRequestsRow.WorkSessionID & "' "
                        cmdText &= ", '" & bcPositionsWithNoRequestsRow.RotorType & "' "
                        cmdText &= ",  " & bcPositionsWithNoRequestsRow.CellNumber

                        'Set value of optional fields (NULL or the informed value)
                        If (Not bcPositionsWithNoRequestsRow.IsExternalPIDNull AndAlso bcPositionsWithNoRequestsRow.ExternalPID <> String.Empty) Then
                            cmdText &= ", N'" & bcPositionsWithNoRequestsRow.ExternalPID.Replace("'", "''") & "' "
                        Else
                            cmdText &= ", NULL "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsSampleTypeNull AndAlso bcPositionsWithNoRequestsRow.SampleType <> String.Empty) Then
                            cmdText &= ", '" & bcPositionsWithNoRequestsRow.SampleType & "' "
                        Else
                            cmdText &= ", NULL "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsPatientIDNull AndAlso bcPositionsWithNoRequestsRow.PatientID <> String.Empty) Then
                            cmdText &= ", N'" & bcPositionsWithNoRequestsRow.PatientID.Replace("'", "''") & "' "
                        Else
                            cmdText &= ", NULL "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsSampleTypeNull AndAlso bcPositionsWithNoRequestsRow.SampleType <> String.Empty) Then
                            cmdText &= ", 0 "
                        Else
                            cmdText &= ", 1 "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsBarCodeInfoNull AndAlso Not String.IsNullOrEmpty(bcPositionsWithNoRequestsRow.BarCodeInfo)) Then
                            ' XB 13/03/2014
                            'cmdText &= ", '" & bcPositionsWithNoRequestsRow.BarCodeInfo & "' "
                            cmdText &= ", '" & bcPositionsWithNoRequestsRow.BarCodeInfo.Replace("'", "''") & "' "
                        Else
                            cmdText &= ", NULL "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsLISStatusNull AndAlso Not String.IsNullOrEmpty(bcPositionsWithNoRequestsRow.LISStatus)) Then
                            cmdText &= ", '" & bcPositionsWithNoRequestsRow.LISStatus & "' "
                        Else
                            cmdText &= ", 'PENDING' "
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsMessageIdNull AndAlso Not String.IsNullOrEmpty(bcPositionsWithNoRequestsRow.MessageId)) Then
                            cmdText &= ", '" & bcPositionsWithNoRequestsRow.MessageId & "' "
                        Else
                            cmdText &= ", NULL "
                        End If

                        cmdText &= ") " & vbCrLf   'Insert line break
                    Next bcPositionsWithNoRequestsRow

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete from the table of Incomplete Patient Samples all positions having the same ExternalPID and SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pExternalPID">External Patient Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2011 
        ''' </remarks>
        Public Function DeleteByExternalPIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                          ByVal pRotorType As String, ByVal pExternalPID As String, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " WHERE  AnalyzerID    =  '" & pAnalyzerID & "' " & vbCrLf & _
                                            " AND    WorkSessionID =  '" & pWorkSessionID & "' " & vbCrLf & _
                                            " AND    RotorType     =  '" & pRotorType & "' " & vbCrLf & _
                                            " AND    ExternalPID   = N'" & pExternalPID.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    SampleType    =  '" & pSampleType & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.DeleteByExternalPIDAndSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete from the table of Incomplete Patient Samples the specified position (Cell Number) in the informed Rotor
        ''' for an Analyzer and WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Position in the specified Rotor</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 29/08/2011
        ''' Modified by: DL 02/09/2011 - Changed field WorkSesionID by WorkSessionID
        ''' </remarks>
        Public Function DeletePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                       ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM twksWSBarcodePositionsWithNoRequests "
                    cmdText &= " WHERE AnalyzerID   = '" & pAnalyzerID & "'"
                    cmdText &= " AND   WorkSessionID = '" & pWorkSessionID & "'"
                    cmdText &= " AND   RotorType    = '" & pRotorType & "'"
                    cmdText &= " AND   CellNumber   =  " & pCellNumber

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.DeletePosition", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all positions with incomplete data in the specified Analyzer, WorkSession and optionally, RotorType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with "incomplete" cells</returns>
        ''' <remarks>
        ''' Created by:  DL 03/08/2011
        ''' Modified by: SA 05/08/2011 - Changed the function template (it has the one for Insert/Update/Delete, not the one for Selects)
        '''              SA 09/04/2013 - Changed the query by adding an INNER JOIN with table twksWSRotorContentByPositions to get field RingNumber
        '''              SA 11/06/2013 - Changed the query by adding an INNER JOIN with table twksWSAnalyzers to get field WSStatus
        ''' </remarks>
        Public Function ReadByAnalyzerAndWorkSession(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     Optional ByVal pRotorType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BPW.*, RCP.RingNumber, WSA.WSStatus " & vbCrLf & _
                                                " FROM   twksWSBarcodePositionsWithNoRequests BPW INNER JOIN twksWSRotorContentByPosition RCP ON BPW.AnalyzerID    = RCP.AnalyzerID " & vbCrLf & _
                                                                                                                                           " AND BPW.WorkSessionID = RCP.WorkSessionID " & vbCrLf & _
                                                                                                                                           " AND BPW.RotorType     = RCP.RotorType " & vbCrLf & _
                                                                                                                                           " AND BPW.CellNumber    = RCP.CellNumber " & vbCrLf & _
                                                                                                " INNER JOIN twksWSAnalyzers WSA ON BPW.AnalyzerID    = WSA.AnalyzerID " & vbCrLf & _
                                                                                                                              " AND BPW.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                                                " WHERE  BPW.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    BPW.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf

                        If (pRotorType <> String.Empty) Then cmdText &= " AND BPW.RotorType = '" & pRotorType & "' " & vbCrLf
                        cmdText &= " ORDER BY BPW.ExternalPID, BPW.SampleType "

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ReadByAnalyzerAndWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets information about an specific position in table twksWSBarcodePositionsWithNoRequests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Position in the specified Rotor</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with the ExternalPID
        '''          read in the specified Rotor Position</returns>
        ''' <remarks></remarks>
        Public Function ReadPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                     ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TOP 1 FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    CellNumber = " & pCellNumber

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ReadPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Mark as completed the specified position (Cell Number) in the informed Rotor Type for an Analyzer and WorkSession
        ''' in the table of Incomplete Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Position in the specified Rotor</param>
        ''' <param name="pCompletedFlag">Value to set to the CompletedFlag</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/09/2011
        ''' Modified by: SA 20/09/2011 - Added optional parameter to allow update also the SampleType when it is informed; 
        '''                              Added parameter to indicate the value to set for the CompletedFlag 
        '''              TR 21/09/2011 - When SampleType is not informed, then set field value to NULL
        ''' </remarks>
        Public Function UpdateCompletedFlagByPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pCompletedFlag As Boolean, _
                                                      Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " SET    CompletedFlag = " & Convert.ToInt32(IIf(pCompletedFlag, 1, 0)) & vbCrLf

                    'When the optional parameter is informed, update also the SampleType
                    If (pSampleType <> String.Empty) Then
                        cmdText &= ", SampleType = '" & pSampleType & "' " & vbCrLf
                    Else
                        cmdText &= ", SampleType = NULL" & vbCrLf
                    End If

                    'Add conditions for the updation
                    cmdText &= " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                               " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                               " AND    RotorType     = '" & pRotorType & "' " & vbCrLf & _
                               " AND    CellNumber    = " & pCellNumber & vbCrLf


                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateCompletedFlagByPosition", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the LIS Status of all incomplete Patient Samples having the informed Barcodes
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatusBarCodeInfoList">List of SpecimenIDs (Barcodes) to update</param>
        ''' <param name="pNewStatus">New LIS Status to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2013
        ''' Modified by: SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by BarcodeInfo (Barcodes are case sensitive).  
        '''                              This is needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function UpdateLISStatus(ByVal pDBConnection As SqlClient.SqlConnection, pStatusBarCodeInfoList As List(Of String), ByVal pNewStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = String.Empty
                    For Each myStatusBarcode As String In pStatusBarCodeInfoList
                        cmdText &= " UPDATE twksWSBarcodePositionsWithNoRequests "
                        cmdText &= " SET   LISStatus = '" & pNewStatus.Trim & "' "
                        cmdText &= " WHERE  BarcodeInfo = N'" & myStatusBarcode.Trim & "' " & caseSensitiveCollation
                        cmdText &= Environment.NewLine
                    Next

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateLISStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other methods"
        ''' <summary>
        ''' Mark as completed in the table of Incomplete Patient Samples all positions having the same ExternalPID and SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pExternalPID">External Patient Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pPatientID">Identifier of a Patient that exists in the DB. Optional parameter used only when Patient
        '''                          data is imported from LIS; in this case the value is updated for all positions having the 
        '''                          same ExternalPID</param>
        ''' <param name="pSTypeAsFilter">Flag indicating if the informed SampleType has to be used as query filter (default value)
        '''                              When data is imported from LIS, the informed Sample Type is updated for all positions having
        '''                              the same ExternalPID and SampleType NULL</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/09/2011
        ''' Modified by: SA 26/09/2011 - Added optional parameters to save also the DB PatientID and the SampleType(used only when the Patient Sample 
        '''                              was "Completed" from LIS). In case the SampleType is updated, then it is not used as filter (query is filtered
        '''                              by Sample Type = NULL) 
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function CompleteByExternalPIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                           ByVal pRotorType As String, ByVal pExternalPID As String, ByVal pSampleType As String, _
                                                           Optional ByVal pPatientID As String = "", Optional ByVal pSTypeAsFilter As Boolean = True) _
                                                           As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE  twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " SET     CompletedFlag = 1 " & vbCrLf

                    If (pPatientID <> String.Empty) Then cmdText &= ", PatientID = N'" & pPatientID.Trim.Replace("'", "''") & "' "
                    If (Not pSTypeAsFilter) Then cmdText &= ", SampleType = '" & pSampleType.Trim & "' "

                    cmdText &= " WHERE   AnalyzerID         =  '" & pAnalyzerID & "' " & vbCrLf & _
                               " AND     WorkSessionID      =  '" & pWorkSessionID & "' " & vbCrLf & _
                               " AND     RotorType          =  '" & pRotorType & "' " & vbCrLf & _
                               " AND     UPPER(ExternalPID) = UPPER(N'" & pExternalPID.Replace("'", "''") & "') " & vbCrLf
                    '" AND     UPPER(ExternalPID) = N'" & pExternalPID.Replace("'", "''").ToUpper & "' " & vbCrLf

                    If (pSTypeAsFilter) Then
                        cmdText &= " AND SampleType IS NOT NULL AND SampleType = '" & pSampleType & "' "
                    Else
                        cmdText &= " AND SampleType IS NULL "
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.CompleteByExternalPIDAndSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count the number of different SampleType values that exists for an specific Barcode (number of incomplete Patient Samples having the
        ''' same Barcode but for which different Samples Types have been assigned). NOTE: comparison by Barcode has to be done in a CASE SENSITIVE
        ''' way; the COLLATE sentence used has to be added due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, 
        ''' but not in this)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBCInfoRow">Row of a typed DS BarcodePositionsWithNoRequestsDS containing all information of an incomplete Patient Sample
        '''                          placed in the Samples Rotor</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of different Sample Types (including NULL and Empty values) that 
        '''          exists for the specified Barcode</returns>
        ''' <remarks>
        ''' Created by:  SA 30/05/2013 
        ''' Modified by: SA 03/06/2013 - Changed the SQL to get also the different SampleTypes for the IN USE tubes with the same specified Barcode
        ''' </remarks>
        Public Function CountSampleTypesByBC(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBCInfoRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow) _
                                             As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT SampleType FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pBCInfoRow.AnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pBCInfoRow.WorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    BarcodeInfo   = N'" & pBCInfoRow.BarCodeInfo.Trim.Replace("'", "''") & "' " & caseSensitiveCollation & vbCrLf & _
                                                " UNION ALL " & vbCrLf & _
                                                " SELECT DISTINCT SampleType " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN twksWSRotorContentByPosition RCP ON RE.ElementID = RCP.ElementID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = N'" & pBCInfoRow.AnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pBCInfoRow.WorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                " AND    RCP.BarcodeInfo   = N'" & pBCInfoRow.BarCodeInfo.Trim.Replace("'", "''") & "' " & caseSensitiveCollation & vbCrLf & _
                                                " AND    RE.TubeContent    = 'PATIENT' "

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet.twksWSBarcodePositionsWithNoRequests.Rows.Count
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.CountSampleTypesByBC", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete from table of Incomplete Patient Samples all records marked as completed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2011
        ''' </remarks>
        Public Function DeleteCompletedSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " WHERE  AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    CompletedFlag = 1 "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.DeleteCompletedSamples", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all elements with Barcode informed and not in use on the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with all information of the 
        '''          incomplete Patient Samples returned</returns>
        ''' <remarks>
        ''' Created by: TR 07/05/2013
        ''' </remarks>
        Public Function GetScannedAndNotInUseElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    BarcodeInfo IS NOT NULL " & vbCrLf

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.GetScannedAndNotInUseElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Samples Barcodes with LISStatus = ASKING (a Host Query was sent to LIS for them) for which LIS has not sent information or it has 
        ''' not sent any accepted Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with the list of Samples Barcodes with LISStatus = ASKING
        '''          (a Host Query was sent to LIS for them) for which LIS has not sent information or it has not sent any accepted Order Test</returns>
        ''' <remarks>
        ''' Created by:  SA 04/07/2013
        ''' </remarks>
        Public Function ReadAskingBCNotSentByLIS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSBarcodePositionsWithNoRequests BP " & vbCrLf & _
                                                " WHERE  BP.LISStatus = 'ASKING' " & vbCrLf & _
                                                " AND    BP.BarcodeInfo NOT IN (SELECT DISTINCT SOT.SpecimenID " & vbCrLf & _
                                                                              " FROM   tparSavedWSOrderTests SOT INNER JOIN tparSavedWS S ON SOT.SavedWSID = S.SavedWSID " & vbCrLf & _
                                                                               " WHERE  S.FromLIMS = 1) " & vbCrLf

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ReadAskingBCNotSentByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all positions with incomplete data in the specified Analyzer, WorkSession and optionally, RotorType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with "incomplete" cells</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2011
        ''' Modified by: SA 16/09/2011 - Changed the query to get following information: 
        '''                                 ** If SampleType is informed, only one record for each different ExternalPID/Sample Type
        '''                                 ** If SampleType is not informed, all ExternalIDs have to be returned, no matter if they are duplicated
        '''              SA 19/09/2011 - Changed the query: use UNION ALL instead of UNION; filter both sub-queries by CompletedFlag = False
        ''' </remarks>
        Public Function ReadDistinctPatientSamples(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     Optional ByVal pRotorType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all different ExternalPID/SampleType with SampleType informed
                        Dim cmdText As String = " SELECT DISTINCT RotorType, ExternalPID, SampleType, StatFlag, PatientID, 0 AS CellNumber " & vbCrLf & _
                                                " FROM   twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    SampleType IS NOT NULL " & vbCrLf & _
                                                " AND    CompletedFlag = 0 " & vbCrLf

                        If (pRotorType <> String.Empty) Then cmdText &= " AND RotorType = '" & pRotorType & "' " & vbCrLf

                        'Now get all ExternalPID without SampleType informed
                        cmdText &= " UNION ALL " & vbCrLf & _
                                   " SELECT RotorType, ExternalPID, SampleType, StatFlag, PatientID, CellNumber " & vbCrLf & _
                                   " FROM   twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                   " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                   " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                   " AND    SampleType IS NULL " & vbCrLf & _
                                   " AND    CompletedFlag = 0 " & vbCrLf

                        If (pRotorType <> String.Empty) Then cmdText &= " AND RotorType = '" & pRotorType & "' " & vbCrLf

                        'Finally, sort data for ExternalID and SampleType
                        cmdText &= " ORDER BY RotorType, ExternalPID, SampleType, StatFlag "

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ReadDistinctPatientSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all records from the table twksWSBarcodePositionsWithNoRequest by Analyzer, WorkSession and Rotor Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 15/09/2011 Delete by AnalyzerID, WorksessionID and Rotor Type identifier
        ''' </remarks>
        Public Function ResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String
                    cmdText = String.Format(" DELETE FROM twksWSBarcodePositionsWithNoRequests " & _
                                            " WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' AND RotorType = '{2}'", _
                                            pAnalyzerID, pWorkSessionID, pRotorType)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ResetRotor", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all records from the table twksWSBarcodePositionsWithNoRequest filtering them by Analyzer and WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 05/09/2011 - Delete by Analyzer and WorkSession Identifier
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " WHERE  AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of Sample by MessageID. Sets MessageID to NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">AnalyzerID</param>
        ''' <param name="pRotorType">RotorType</param>
        ''' <param name="pMessageId">Value of MessageID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by : JC 24/04/2013
        ''' </remarks>
        Public Function UpdateHQStatus(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pMessageID As String, _
                                       ByVal pLISStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "    UPDATE   twksWSBarcodePositionsWithNoRequests "
                    cmdText &= "       SET   MessageID = NULL"
                    cmdText &= "           , LISStatus  = '" & pLISStatus & "' "
                    cmdText &= "     WHERE   AnalyzerID = '" & pAnalyzerID & "'"
                    cmdText &= "       AND   RotorType  = '" & pRotorType & "'"
                    cmdText &= "       AND   MessageID  = '" & pMessageID & "'"
                    cmdText &= String.Format("{0}", vbNewLine) 'Insert line break

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateHQStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the MessageId of the specified by SpecimentId. LisStatus MustBe Asking, and Pevious MessageId NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">AnalyerId</param>
        ''' <param name="pRotorType">RotorType</param>
        ''' <param name="pSpecimenID">SpecimentId, internally is BarcodeInfo</param>
        ''' <param name="pMessageID">Value of MessageId</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by : JC  18/04/2013
        ''' Modified by: SGM 03/05/2013 - Allow update if field MessageID is already informed
        '''              SA  30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by BarcodeInfo=SpecimenID (Barcodes are case sensitive).  
        '''                               This is needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function UpdateMessageIDBySpecimenID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                    ByVal pSpecimenID As String, ByVal pMessageID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pSpecimenID Is Nothing) Then
                    Dim cmdText As String = " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " SET    MessageID = '" & pMessageID.Trim & "' " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                            " AND    BarcodeInfo = N'" & pSpecimenID.Trim.Replace("'", "''") & "' " & caseSensitiveCollation & vbCrLf & _
                                            " AND    LISStatus = 'ASKING' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateMessageIDBySpecimenID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Once a LIS Saved WS has been successfully processed, LIS Status of all SpecimenIDs contained in it has to be changed from ASKING to
        ''' PENDING, and the MessageID has to be updated to PROCESSED. Afterwards, tubes having a Barcode that match only one SpecimenID/SampleType 
        ''' will be removed from the list of incomplete Patient Samples, but tubes for which LIS sent Tests of more than one SampleType will remain 
        ''' as incomplete and show the warning of SampleType in the Host Query screen to indicate to final User he/she has to indicate the SampleType 
        ''' of the tube; to show the warning, field MessageID has to be updated to PROCESSED
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the LIS Saved WS</param>
        ''' <param name="pSpecimenIDList">List of SpecimenIDs for which LIS has sent Tests for several Sample Types</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 29/05/2013
        ''' Modified by: SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by BarcodeInfo=SpecimenID (Barcodes are case sensitive).  
        '''                              This is needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        '''              SA 17/07/2013 - Removed parameter pSavedWSID. Added parameter containing the list of SpecimenIDs (Barcodes) for which LIS sent Tests for
        '''                              more than one Sample Type
        ''' </remarks>
        Public Function UpdateMessageIDToProcessed(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, ByVal pSpecimenIDList As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " SET    LISStatus = 'PENDING' " & vbCrLf & _
                                            " WHERE  LISStatus = 'ASKING' " & vbCrLf & _
                                            " AND    BarcodeInfo " & caseSensitiveCollation & " IN (SELECT DISTINCT SpecimenID FROM tparSavedWSOrderTests " & vbCrLf & _
                                                                                                  " WHERE SavedWSID = " & pSavedWSID.ToString & ") " & vbCrLf

                    If (pSpecimenIDList.Trim <> String.Empty) Then
                        cmdText &= " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                   " SET    MessageID = 'PROCESSED' " & vbCrLf & _
                                   " WHERE  BarcodeInfo " & caseSensitiveCollation & " IN (" & pSpecimenIDList.Trim & ") " & vbCrLf
                    End If

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateMessageIDToProcessed", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the StatFlag, SampleType and LISStatus of the specified Incomplete Patient Sample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarcodePositionsWithNoRequestsRow">Typed DataSet BarcodePositionsWithNoRequestsDS containing the information
        '''                                                 of the incomplete cells to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by : JC 08/03/2013
        ''' </remarks>
        Public Function UpdateSamples(ByVal pDBConnection As SqlConnection, _
                                      ByVal pBarcodePositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pBarcodePositionsWithNoRequestsRow Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText &= " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                   "    SET StatFlag      = " & Convert.ToInt32(pBarcodePositionsWithNoRequestsRow.StatFlag) & vbCrLf & _
                                   "      , SampleType    = '" & pBarcodePositionsWithNoRequestsRow.SampleType & "'" & vbCrLf & _
                                   "      , LISStatus     = '" & pBarcodePositionsWithNoRequestsRow.LISStatus & "'" & vbCrLf & _
                                   " WHERE  AnalyzerID    =  '" & pBarcodePositionsWithNoRequestsRow.AnalyzerID & "' " & vbCrLf & _
                                   " AND    WorkSessionID =  '" & pBarcodePositionsWithNoRequestsRow.WorkSessionID & "' " & vbCrLf & _
                                   " AND    RotorType     =  '" & pBarcodePositionsWithNoRequestsRow.RotorType & "' " & vbCrLf & _
                                   " AND    CellNumber    = " & Convert.ToInt32(pBarcodePositionsWithNoRequestsRow.CellNumber) & vbCrLf & _
                                   " AND    ExternalPID   = N'" & pBarcodePositionsWithNoRequestsRow.ExternalPID.Replace("'", "''") & "' " & vbCrLf
                    cmdText &= String.Format("{0}", vbNewLine) 'Insert line break

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateSamples", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the StatFlag of the specified list of Incomplete Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarcodePositionsWithNoRequestsDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing the information
        '''                                                 of the incomplete cells to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by : DL 03/08/2011
        ''' Modified by: SA 05/08/2011 - Removed updation of field ExternalID (it is not possible to update it). Added updation of 
        '''                              field StatFlag 
        '''              DL 01/09/2011 - SQL sentence has errors
        '''              SA 13/09/2011 - Changed the SQL to update only field StatFlag
        '''              SA 04/10/2011 - When the CellNumber is informed, filter data also for it
        ''' </remarks>
        Public Function UpdateStatFlag(ByVal pDBConnection As SqlConnection, ByVal pBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pBarcodePositionsWithNoRequestsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each bcPositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In pBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests
                        cmdText &= " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                   " SET    StatFlag      = " & Convert.ToInt32(bcPositionsWithNoRequestsRow.StatFlag) & vbCrLf & _
                                   " WHERE  AnalyzerID    =  '" & bcPositionsWithNoRequestsRow.AnalyzerID & "' " & vbCrLf & _
                                   " AND    WorkSessionID =  '" & bcPositionsWithNoRequestsRow.WorkSessionID & "' " & vbCrLf & _
                                   " AND    RotorType     =  '" & bcPositionsWithNoRequestsRow.RotorType & "' " & vbCrLf & _
                                   " AND    ExternalPID   = N'" & bcPositionsWithNoRequestsRow.ExternalPID.Replace("'", "''") & "' " & vbCrLf

                        If (Not bcPositionsWithNoRequestsRow.IsSampleTypeNull) Then
                            cmdText &= " AND SampleType = '" & bcPositionsWithNoRequestsRow.SampleType & "' " & vbCrLf
                        End If

                        If (Not bcPositionsWithNoRequestsRow.IsCellNumberNull) Then
                            cmdText &= " AND CellNumber = " & bcPositionsWithNoRequestsRow.CellNumber & vbCrLf
                        End If

                        cmdText &= String.Format("{0}", vbNewLine) 'Insert line break
                    Next bcPositionsWithNoRequestsRow

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateStatFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the Specimen is positioned in Samples Rotor (the Tube has been already scanned).
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSpecimenID">Specimen Identifier</param>
        ''' <param name="pPatientID">Sample/Patient Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns>GlobalDataTO containing an Integer value with the number of Tubes placed in Samples Rotor and marked as incompleted Samples</returns>
        ''' <remarks>
        ''' Created by:  TR 11/04/2013
        ''' Modified by: SA 18/04/2013 - Changed the SQL Query: filter should be by BarcodeInfo OR ExternalPID, instead of by
        '''                              BarcodeInfo OR PatientID
        '''              TR-SA 15/05/2013 - Add filter SampleType = '', because the sample status is change to empty.  
        '''              SA    30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by BarcodeInfo (Barcodes are case sensitive). This is 
        '''                                 needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function VerifyScannedSpecimen(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pSpecimenID As String, ByVal pPatientID As String, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumTubes FROM twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                                " WHERE AnalyzerID    = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND  (BarcodeInfo   = N'" & pSpecimenID.Trim.Replace("'", "''") & "' " & caseSensitiveCollation & vbCrLf & _
                                                " OR    ExternalPID   = N'" & pPatientID.Trim.Replace("'", "''") & "') " & vbCrLf & _
                                                " AND  (SampleType IS NULL OR SampleType = '' " & vbCrLf & _
                                                " OR   (SampleType IS NOT NULL AND SampleType = '" & pSampleType.Trim & "')) " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.VerifyScannedSpecimen", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the CellNumber of the specified Patient Sample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCellNumber">New Position in the specified Rotor</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pBarcodeInfo">Sample Barcode</param>
        ''' <param name="pSampleType">Sample Type Code. optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 27/08/2013
        ''' </remarks>
        Public Function UpdateCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCellNumber As Integer, ByVal pAnalyzerID As String, _
                                         ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pBarcodeInfo As String, _
                                         Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksWSBarcodePositionsWithNoRequests " & vbCrLf & _
                                            " SET    CellNumber = " & pCellNumber & vbCrLf

                    'Add conditions for the updation
                    cmdText &= " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                               " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                               " AND    RotorType     = '" & pRotorType & "' " & vbCrLf & _
                               " AND    BarcodeInfo     = '" & pBarcodeInfo & "' " & vbCrLf

                    'When the optional parameter is informed, update also the SampleType
                    If (pSampleType <> String.Empty) Then
                        cmdText &= " AND    SampleType = '" & pSampleType & "' " & vbCrLf
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.UpdateCellNumber", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Commented methods"

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID"></param>
        '''' <param name="pRotorType"></param>
        '''' <param name="pCellNumber"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATED BY: DL 03/08/2011
        '''' </remarks>
        'Public Function Read(ByVal pDBConnection As SqlConnection, _
        '                     ByVal pAnalyzerID As String, _
        '                     ByVal pWorkSessionID As String, _
        '                     ByVal pRotorType As String, _
        '                     ByVal pCellNumber As Integer) As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection

        '            cmdText &= "SELECT AnalyzerID, WorkSesionID, RotorType, CellNumber, ExternalPID, SampleType" & vbCrLf
        '            cmdText &= "  FROM twksWSBarcodePositionsWithNoRequests" & vbCrLf
        '            cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
        '            cmdText &= "   AND WorkSesionID = '" & pWorkSessionID & "'" & vbCrLf
        '            cmdText &= "   AND RotorType = '" & pRotorType & "'" & vbCrLf
        '            cmdText &= "   AND CellNumber = " & pCellNumber & vbCrLf

        '            dbCmd.CommandText = cmdText
        '            Dim myDataDS As New BarcodePositionsWithNoRequestsDS
        '            Dim dbDataAdapter As New SqlDataAdapter(dbCmd)
        '            dbDataAdapter.Fill(myDataDS.twksWSBarcodePositionsWithNoRequests)

        '            myGlobalDataTO.SetDatos = myDataDS

        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSBarcodePositionsWithNoRequestsDAO.Read", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region

    End Class
End Namespace
