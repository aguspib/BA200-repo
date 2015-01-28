Option Strict On
Option Explicit On

Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class thisWSOrderTestsDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Add to Historic Module all Order Tests having accepted and validated Results in the active Analyzer WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS containing the list of Order Tests to add to Historic Module</param>
        ''' <returns>GlobalDataTO containing the same entry DS but updated with the HistOrderTestID generated for each added Order Test</returns>
        ''' <remarks>
        ''' Created by:  TR 19/06/2012 
        ''' Modified by: AG 24/04/2013 - Added new fields: LISRequest, ExternalQC, SpecimenID, AwosID, ESOrderID, ESPatientID, LISOrderID, 
        '''                              LISPatientID, LISTestName, LISSampleType, LISUnits
        '''              SA 04/09/2014 - BA-1919 ==> When insert LIS fields, use the N prefix to save correctly texts containing two-bytes characters.
        '''                                          Use also the Replace function to save correctly texts containing single quotes
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim HistOrderTestID As Integer = -1

                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        For Each hisWSOrderTestRow As HisWSOrderTestsDS.thisWSOrderTestsRow In pHisWSOrderTestsDS.thisWSOrderTests.Rows
                            cmdText.Append("  INSERT INTO thisWSOrderTests ")
                            cmdText.AppendLine(" (AnalyzerID, WorkSessionID, OrderDateTime, SampleClass, StatFlag, TestType, HistTestID, ")
                            cmdText.AppendLine("  SampleType, TestVersionNumber, ReplicatesNumber, HistCalibratorID, HistPatientID, ")
                            cmdText.AppendLine("  SampleID, MeasureUnit, LISRequest, ExternalQC, SpecimenID, AwosID, ESOrderID, ESPatientID, ")
                            cmdText.AppendLine("  LISOrderID, LISPatientID, LISTestName, LISSampleType, LISUnits ) ")

                            cmdText.AppendFormat(" VALUES(N'{0}', '{1}', '{2}', '{3}', {4}, '{5}', {6}, '{7}', ", _
                                                 hisWSOrderTestRow.AnalyzerID.Trim, _
                                                 hisWSOrderTestRow.WorkSessionID.Trim, _
                                                 hisWSOrderTestRow.OrderDateTime.ToString("yyyyMMdd HH:mm:ss"), _
                                                 hisWSOrderTestRow.SampleClass.Trim, _
                                                 Convert.ToInt32(IIf(hisWSOrderTestRow.StatFlag, 1, 0)), _
                                                 hisWSOrderTestRow.TestType.Trim, _
                                                 hisWSOrderTestRow.HistTestID, _
                                                 hisWSOrderTestRow.SampleType.Trim)

                            'Test Version Number is informed only for Standard Tests...
                            If (Not hisWSOrderTestRow.IsTestVersionNumberNull) Then
                                cmdText.AppendLine(hisWSOrderTestRow.TestVersionNumber.ToString() & ", ")
                            Else
                                cmdText.AppendLine("NULL, ")
                            End If

                            cmdText.AppendLine(hisWSOrderTestRow.ReplicatesNumber & ", ")

                            If (Not hisWSOrderTestRow.IsHistCalibratorIDNull) Then
                                cmdText.AppendLine(hisWSOrderTestRow.HistCalibratorID.ToString() & ", ")
                            Else
                                cmdText.AppendLine("NULL, ")
                            End If

                            If (Not hisWSOrderTestRow.IsHistPatientIDNull) Then
                                cmdText.AppendLine(hisWSOrderTestRow.HistPatientID.ToString() & ", ")
                            Else
                                cmdText.AppendLine("NULL, ")
                            End If

                            If (Not hisWSOrderTestRow.IsSampleIDNull) Then
                                cmdText.AppendLine("N'" & hisWSOrderTestRow.SampleID.Replace("'", "''") & "', ")
                            Else
                                cmdText.AppendLine("NULL, ")
                            End If

                            If (Not hisWSOrderTestRow.IsMeasureUnitNull) Then
                                cmdText.AppendLine("'" & hisWSOrderTestRow.MeasureUnit.Trim & "' ")
                            Else
                                cmdText.AppendLine("NULL ")
                            End If

                            'AG 24/04/2013 Added fields: LISRequest, ExternalQC, SpecimenID, AwosID, ESOrderID, ESPatientID, LISOrderID, LISPatientID, LISTestName, LISSampleType, LISUnits
                            If (Not hisWSOrderTestRow.IsLISRequestNull) Then
                                cmdText.Append(", " & IIf(hisWSOrderTestRow.LISRequest, "1", "0").ToString)
                            Else
                                cmdText.Append(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsExternalQCNull) Then
                                cmdText.Append(", " & IIf(hisWSOrderTestRow.ExternalQC, "1", "0").ToString)
                            Else
                                cmdText.Append(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsSpecimenIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.SpecimenID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsAwosIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.AwosID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsESOrderIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.ESOrderID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsESPatientIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.ESPatientID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsLISOrderIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.LISOrderID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsLISPatientIDNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.LISPatientID.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsLISTestNameNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.LISTestName.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsLISSampleTypeNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.LISSampleType.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL ")
                            End If

                            If (Not hisWSOrderTestRow.IsLISUnitsNull) Then
                                cmdText.AppendLine(", N'" & hisWSOrderTestRow.LISUnits.Replace("'", "''").Trim & "' ")
                            Else
                                cmdText.AppendLine(", NULL")
                            End If
                            cmdText.Append(")")
                            'AG 24/04/2013

                            cmdText.AppendFormat("{0} SELECT SCOPE_IDENTITY()", vbCrLf)

                            dbCmd.CommandText = cmdText.ToString()
                            HistOrderTestID = CInt(dbCmd.ExecuteScalar())
                            If (HistOrderTestID > 0) Then
                                hisWSOrderTestRow.HistOrderTestID = HistOrderTestID
                            End If
                            cmdText.Length = 0
                        Next
                    End Using

                    myGlobalDataTO.SetDatos = pHisWSOrderTestsDS
                    myGlobalDataTO.HasError = False
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Historic data saved for the specified HistOrderTestID/AnalyzerID/WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pHistWSID">Work Session Identifier in Historic Module</param>
        ''' <param name="pHistOTID">Order Test Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with all data of the specified
        '''          Order Test in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 19/10/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                             ByVal pHistWSID As String, ByVal pHistOTID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisWSOrderTests " & vbCrLf & _
                                                " WHERE  HistOrderTestID = " & pHistOTID.ToString & vbCrLf & _
                                                " AND    AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pHistWSID.Trim & "' " & vbCrLf

                        Dim myDataSet As New HisWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.thisWSOrderTests)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all Order Tests in Historic Module for the specified Analyzer (for all Work Sessions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with all HistOrderTestIDs for the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by: SGM 25/04/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisWSOrderTests " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim & "' "

                        Dim myDataSet As New HisWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.thisWSOrderTests)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' After a result has been exported from historical results the fields used for upload to LIS are cleared (free database space)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID">LIS Message Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 24/04/2013
        ''' </remarks>
        Public Function ClearIdentifiersForLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISMessageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "UPDATE thisWSOrderTests " & vbCrLf
                    cmdText &= "   SET LISRequest = NULL" & vbCrLf
                    cmdText &= "      ,ExternalQC = NULL" & vbCrLf
                    cmdText &= "      ,ESOrderID = NULL" & vbCrLf
                    cmdText &= "      ,LISOrderID = NULL" & vbCrLf
                    cmdText &= "      ,ESPatientID = NULL" & vbCrLf
                    cmdText &= "      ,LISPatientID = NULL" & vbCrLf
                    cmdText &= "      ,LISTestName = NULL" & vbCrLf
                    cmdText &= "      ,LISSampleType = NULL" & vbCrLf
                    cmdText &= "      ,LISUnits = NULL" & vbCrLf
                    'AJG. ADDED THOSE TWO LINES OF CODE
                    'cmdText &= " WHERE HistOrderTestID IN (SELECT DISTINCT HistOrderTestID FROM thisWSResults WHERE LISMessageID = '" & pLISMessageID & "')"
                    cmdText &= " WHERE EXISTS (SELECT HistOrderTestID FROM thisWSResults WHERE LISMessageID = '" & pLISMessageID & "') " & vbCrLf
                    cmdText &= " AND thisWSOrderTests.HistOrderTestID = HistOrderTestID"

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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.ClearIdentifiersForLIS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count the number of HistOrderTestIDs that exist in Historic Module for the specified TestID / SampleType / TestVersionNumber, to known 
        ''' if it can be deleted from thisTestSamples (if there are nor HistOrderTests for it, then it can be deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">STD Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestVersionNum">Test Version Number</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of HistOrderTestIDs that exist in Historic Module for 
        '''          the specified TestID / SampleType / TestVersionNumber</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function CountForSTDTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                        ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS myNumLinkedHistOTs FROM thisWSOrderTests " & vbCrLf & _
                                                " WHERE  TestType = 'STD' " & vbCrLf & _
                                                " AND    HistTestID = " & pHistTestID.ToString & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    TestVersionNumber = " & pTestVersionNum.ToString & vbCrLf

                        Dim myNumLinkedHistOTs As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    myNumLinkedHistOTs = CInt(dbDataReader.Item("myNumLinkedHistOTs"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = myNumLinkedHistOTs
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.CountForSTDTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the Historical Patient Order Tests selected to be Export to LIS. Function created for BT #1453 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestId">The list of HistOrderTestId to be exported</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with basic data of the Historical Patient Order Tests to Export to LIS</returns>
        ''' <remarks>
        ''' Created by:  SA 16/01/2014
        ''' </remarks>
        Public Function GetDataToExportFromHIST(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  HOT.HistOrderTestID AS OrderTestID, 1 as RerunNumber, HOT.SampleClass, HOT.AnalyzerID, " & vbCrLf & _
                                                       "  HOT.WorkSessionID, HOT.LISRequest, HOT.TestType, HOT.SampleType, " & vbCrLf & _
                                                       " (CASE WHEN SampleID IS NOT NULL THEN SampleID " & vbCrLf & _
                                                       "  ELSE (SELECT P.PatientID FROM thisPatients P WHERE P.HistPatientID = HOT.HistPatientID) END) AS SampleID " & vbCrLf & _
                                                " FROM   thisWSOrderTests HOT " & vbCrLf & _
                                                " WHERE  HOT.SampleClass = 'PATIENT' " & vbCrLf

                        Dim cmdOrderTests As String = String.Empty
                        For Each elem As Integer In pHistOrderTestID
                            If (Not String.IsNullOrEmpty(cmdOrderTests)) Then cmdOrderTests &= ", "
                            cmdOrderTests &= elem.ToString
                        Next
                        If (Not String.IsNullOrEmpty(cmdOrderTests)) Then
                            cmdText &= " AND HOT.HistOrderTestID IN (" & cmdOrderTests & ") "
                        End If
                        cmdText &= " ORDER BY SampleID "

                        Dim myDataSet As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSExecutions)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.GetDataToExportFromHIST", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Order Tests of the specified SampleClass that exist in Historic Module for the informed HistTestID and optionally, SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter, informed only when SampleClass is not BLANK</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with the list of Order Tests</returns>
        ''' <remarks>
        ''' Created by:  AG 08/10/2012
        ''' Modified by: SA 19/10/2012 - Removed parameter and filter by TestVersionNumber
        ''' </remarks>
        Public Function ReadByHistTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, _
                                         ByVal pSampleClass As String, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisWSOrderTests " & vbCrLf & _
                                                " WHERE  SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND  HistTestID  = " & pHistTestID.ToString & vbCrLf

                        If (pSampleType <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myDataSet As New HisWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.thisWSOrderTests)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.ReadByHistTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all Calibrators and Patient Results of an specific STD Test (all SampleTypes) executed in the informed Analyzer WorkSession,
        ''' update WorkSession/HistOrderTestID of the Blank used to calculate them (which can be from the same WorkSession or from a previous one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Identifier of the current WorkSession in Historic Module</param>
        ''' <param name="pHistOTBlankRow">Row of a typed DataSet HistWSOrderTestsDS containing data of the used Blank Order Test</param>
        ''' <param name="pIgnoreTestVersion">Optional parameter. When TRUE, it indicates fields of Blank used are updated for the affected 
        '''                                  Blank and Calibrator results although the TestVersionNumber is different (due to it is possible
        '''                                  having Blanks from previous versions still open when the version change was due to modifications
        '''                                  in Calibrator values)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 23/10/2012
        ''' </remarks>
        Public Function UpdateBLANKFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                          ByVal pWorkSessionID As String, ByVal pHistOTBlankRow As HisWSOrderTestsDS.thisWSOrderTestsRow, _
                                          Optional ByVal pIgnoreTestVersion As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisWSOrderTests SET HistWSID_Blank = '" & pHistOTBlankRow.WorkSessionID.Trim & "', " & vbCrLf & _
                                                                        " HistOTID_Blank = " & pHistOTBlankRow.HistOrderTestID.ToString & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    SampleClass IN ('CALIB', 'PATIENT') " & vbCrLf & _
                                            " AND    TestType = 'STD' " & vbCrLf & _
                                            " AND    HistTestID = " & pHistOTBlankRow.HistTestID.ToString & vbCrLf
                    If (Not pIgnoreTestVersion) Then cmdText &= " AND TestVersionNumber = " & pHistOTBlankRow.TestVersionNumber.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.UpdateBLANKFields", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all Patient Results of an specific STD Test/SampleType executed in the informed Analyzer WorkSession, update 
        ''' WorkSession/HistOrderTestID of the Calibrator used to calculate them (which can be from the same WorkSession or from a 
        ''' previous one)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Identifier of the current WorkSession in Historic Module</param>
        ''' <param name="pHistOTCalibRow">Row of a typed DataSet HistWSOrderTestsDS containing data of the used Calibrator Order Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 23/10/2012
        ''' </remarks>
        Public Function UpdateCALIBFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                          ByVal pHistOTCalibRow As HisWSOrderTestsDS.thisWSOrderTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisWSOrderTests SET HistWSID_Calib = '" & pHistOTCalibRow.WorkSessionID.Trim & "', " & vbCrLf & _
                                                                        " HistOTID_Calib = " & pHistOTCalibRow.HistOrderTestID.ToString & vbCrLf
                    If (Not pHistOTCalibRow.IsCalibrationFactorNull) Then cmdText &= ", CalibrationFactorUsed = " & ReplaceNumericString(pHistOTCalibRow.CalibrationFactor)

                    cmdText &= " WHERE AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                               " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                               " AND   SampleClass = 'PATIENT' " & vbCrLf & _
                               " AND   TestType = 'STD' " & vbCrLf & _
                               " AND   HistTestID = " & pHistOTCalibRow.HistTestID.ToString & vbCrLf & _
                               " AND   SampleType = '" & pHistOTCalibRow.SampleType.Trim & "' " & vbCrLf & _
                               " AND   TestVersionNumber = " & pHistOTCalibRow.TestVersionNumber.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.UpdateCALIBFields", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get TestSamples data information of the specified OrderHistTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestID">Order Test Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with the related data information</returns>
        ''' <remarks>
        ''' Created by XB 30/07/2014 - BT #1863
        ''' </remarks>
        Public Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisTestSamples as TS " & vbCrLf & _
                                                " INNER JOIN thisWSOrderTests as OT on  TS.HistTestID = OT.HistTestID " & vbCrLf & _
                                                " WHERE  OT.HistOrderTestID = " & pHistOrderTestID & vbCrLf

                        Dim myHisTestSamplesDS As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisTestSamplesDS.thisTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHisTestSamplesDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.ReadByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete the specified AnalyzerID / WorkSessionID / OrderTestID 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                       ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisWSOrderTests " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND    HistOrderTestID = " & pHistOrderTestID.ToString & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.Delete", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Count the number of HistOrderTestIDs that exist in Historic Module for the specified Analyzer Work Session, to known if it can be deleted
        ' ''' (if the Analyzer Work Session is empty - all its HistOrderTests have been deleted - it can be deleted)
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing an integer value with the number of HistOrderTestIDs that exist in Historic Module for 
        ' '''          the specified Analyzer Work Session</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function CountByAnalyzerWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) AS NumLinkedHistOTs FROM thisWSOrderTests " & vbCrLf & _
        '                                        " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                        " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

        '                Dim myNumLinkedHistOTs As Integer = 0
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

        '                    If (dbDataReader.HasRows) Then
        '                        dbDataReader.Read()
        '                        If (Not dbDataReader.IsDBNull(0)) Then
        '                            myNumLinkedHistOTs = CInt(dbDataReader.Item("NumLinkedHistOTs"))
        '                        End If
        '                    End If
        '                    dbDataReader.Close()
        '                End Using

        '                resultData.SetDatos = myNumLinkedHistOTs
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSOrderTestsDAO.CountByAnalyzerWS", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace
