Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparSavedWSOrderTestsDAO
          

#Region "Declarations"
        'Comparisons by field AwosID have to be done in a CASE SENSITIVE way. So this SQL Sentence has to
        'be added to some SQL Queries in this class
        Private caseSensitiveCollation As String = " COLLATE Modern_Spanish_CS_AS "
#End Region

#Region "CRUD Methods"

        ''' <summary>
        ''' Add OrderTests to a Saved WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSOrderTestsDS">Typed DataSet containing the data of all Order Tests in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 30/03/2010
        ''' Modified by: SA  27/05/2010 - Changed the SQL sentence due to changes in table definition (fields TestProfileID 
        '''                               and TestProfileName removed; field FormulaText added)
        '''              SA  27/10/2010 - Added N preffix for multilanguage of fields SampleID (it can be a PatientID), TestName  
        '''                               and FormulaText (Test names can be included in it)
        '''              TR  14/03/2013 - Add new columns needed for the LIS process.
        '''              XB  28/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, _
                               Optional pSavedWSID As Integer = -1) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf Not pSavedWSOrderTestsDS Is Nothing Then
                    Dim cmdText As String
                    'PatientIDType, AwosID,SpecimenID,ESOrderID,LISOrderID,ESPatientID,LISPatientID,CalcTestIDs,CalcTestNames,ExternalQC
                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand

                    For Each rowtparSavedWSOrderTest As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                        'SQL Sentence to insert data
                        If pSavedWSID > -1 Then
                            rowtparSavedWSOrderTest.SavedWSID = pSavedWSID
                        End If

                        cmdText = " INSERT INTO tparSavedWSOrderTests (SavedWSID, SampleClass, StatFlag, TestType, TestID, SampleType, " & _
                                                                    " ReplicatesNumber,  TestName, CreationOrder, SampleID, TubeType, " & _
                                                                    " ControlID, FormulaText, PatientIDType, AwosID, SpecimenID, ESOrderID, LISOrderID, " & _
                                                                    " ESPatientID, LISPatientID, CalcTestIDs, CalcTestNames, ExternalQC, Selected) " & _
                                  " VALUES(" & rowtparSavedWSOrderTest.SavedWSID & ", " & _
                                        " '" & rowtparSavedWSOrderTest.SampleClass.Trim & "', " & _
                                        " '" & IIf(rowtparSavedWSOrderTest.StatFlag, "True", "False").ToString & "', " & _
                                        " '" & rowtparSavedWSOrderTest.TestType.Trim & "', " & _
                                               rowtparSavedWSOrderTest.TestID & ", " & _
                                        " '" & rowtparSavedWSOrderTest.SampleType.Trim.Replace("'", "''") & "', " & _
                                               rowtparSavedWSOrderTest.ReplicatesNumber & ", " & _
                                        " N'" & rowtparSavedWSOrderTest.TestName.Trim.Replace("'", "''") & "', "


                        'Control of values of non required fields
                        If rowtparSavedWSOrderTest.IsCreationOrderNull Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= rowtparSavedWSOrderTest.CreationOrder & ", "
                        End If

                        If (rowtparSavedWSOrderTest.IsSampleIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " N'" & rowtparSavedWSOrderTest.SampleID.Trim.Replace("'", "''") & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsTubeTypeNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " '" & rowtparSavedWSOrderTest.TubeType.Trim & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsControlIDNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= rowtparSavedWSOrderTest.ControlID & ", "
                        End If

                        If (rowtparSavedWSOrderTest.IsFormulaTextNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " N'" & rowtparSavedWSOrderTest.FormulaText.Trim & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsPatientIDTypeNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.PatientIDType & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsAwosIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.AwosID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsSpecimenIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.SpecimenID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsESOrderIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.ESOrderID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsLISOrderIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.LISOrderID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsESPatientIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.ESPatientID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsLISPatientIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.LISPatientID & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsCalcTestIDsNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.CalcTestIDs & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsCalcTestNamesNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & rowtparSavedWSOrderTest.CalcTestNames & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsExternalQCNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & rowtparSavedWSOrderTest.ExternalQC & "', "
                        End If

                        If (rowtparSavedWSOrderTest.IsSelectedNull) Then
                            cmdText &= "0) "
                        Else
                            cmdText &= IIf(rowtparSavedWSOrderTest.Selected, 1, 0).ToString & ") "
                        End If

                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                        'If Not dataToReturn.AffectedRecords > 0 Then
                        '    dataToReturn.HasError = True
                        '    Exit For
                        'End If
                    Next

                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Order Test from a Saved WS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSOrderTestID">Saved WS Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks> 
        ''' Created by:  SA 23/04/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSOrderTestID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = "DELETE FROM tparSavedWSOrderTests WHERE SavedWSOrderTestID = " & pSavedWSOrderTestID.ToString & vbCrLf

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete all Order Tests included in the specified Saved Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Saved WS Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks> 
        ''' Created by:  GDS 30/03/2010
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = "DELETE FROM tparSavedWSOrderTests WHERE SavedWSID = " & pSavedWSID.ToString & vbCrLf

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Delete from a Saved WS that have been requested to be loaded, all Tests (whatever type) that have
        ''' been deleted or that are incomplete (this last case is only for Calculated Tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks> 
        ''' Created by:  GDS 30/03/2010
        ''' Modified by: SA  26/05/2010 - Changed the query to delete also Calculated Tests that are
        '''                               marked as incomplete (EnableStatus = False)
        '''              SA  22/10/2010 - Changed the query to delete also ISE Tests that have been deactivated
        '''              SA  01/02/2011 - Changed the query to delete also OFF-SYSTEM Tests that have been deleted
        ''' </remarks>
        Public Function ClearDeletedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparSavedWSOrderTests " & _
                                            " WHERE  SavedWSID = " & pSavedWSID & _
                                            " AND    SampleClass = 'PATIENT' " & _
                                            " AND   (TestType = 'STD'  AND TestID NOT IN (SELECT TestID FROM tparTests)) " & _
                                            " OR    (TestType = 'CALC' AND TestID NOT IN (SELECT CalcTestID FROM tparCalculatedTests)) " & _
                                            " OR    (TestType = 'CALC' AND TestID IN (SELECT CalcTestID FROM tparCalculatedTests WHERE EnableStatus = 0)) " & _
                                            " OR    (TestType = 'ISE'  AND TestID IN (SELECT ISETestID FROM tparISETests WHERE Enabled = 0)) " & _
                                            " OR    (TestType = 'OFFS' AND TestID NOT IN (SELECT OffSystemTestID FROM tparOffSystemTests)) "

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ClearDeletedElements", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Verify if there are Order Tests for the specified SampleID in at least a LIS Saved WS pending to process. This function is used
        ''' when XML Messages are processes and a Patient Node contains a Patient Identifier that exists in tparPatients table, but LIS has 
        ''' not sent demographics for it; in that case, if the Patient is not IN USE, then it is deleted from Patients table, where not IN USE
        ''' means:
        '''   ** It is not in the active WorkSession
        '''   ** It is not in any LIS Saved WorkSession pending to process (this is what this function verifies)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleID">Sample Identifier to search</param>
        ''' <returns>GlobalDataTO containing a Boolean value: when TRUE, it means there is at least an Order Test in a LIS Saved WS for 
        '''          the informed SampleID</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2013 
        ''' </remarks>
        Public Function CountBySampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) FROM tparSavedWSOrderTests " & vbCrLf & _
                                                " WHERE  SavedWSID IN (SELECT SavedWSID FROM tparSavedWS WHERE FromLIMS = 1) " & vbCrLf & _
                                                " AND    UPPER(SampleID) = UPPER(N'" & pSampleID.Trim.Replace("'", "''") & "') " & vbCrLf

                        Dim thereAreOTs As Boolean = False
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    thereAreOTs = (CInt(dbDataReader.Item(0)) > 0)
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = thereAreOTs
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.CountBySampleID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete from the specified Saved Work Session all Order Tests containing the specified Standard Test/SampleType
        ''' or the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <param name="pTestType">Type of the Tests to delete from the Saved WS</param>
        ''' <param name="pTestID">Identifier of the Test to delete from the Saved WS</param>
        ''' <param name="pSampleType">Code of the SampleType fro which the specified Test have to be deleted from
        '''                           the Saved WS. Optional parameter informed only for Standard Tests</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks> 
        ''' Created by:  GDS 05/05/2010
        ''' Modified by: SA  27/05/2010 - Added new parameter to allow delete Tests of whatever type from the specified
        '''                               Saved WS; changed the query to filter by SampleType only when this field is 
        '''                               informed (it is needed for Standard Tests)
        ''' </remarks>
        Public Function DeleteSavedTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, ByVal pTestType As String, _
                                         ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparSavedWSOrderTests " & vbCrLf & _
                                            " WHERE  SavedWSID = " & pSavedWSID.ToString & vbCrLf & _
                                            " AND    TestType  = '" & pTestType.Trim & "' " & vbCrLf & _
                                            " AND    TestID    = " & pTestID.ToString & vbCrLf

                    If (pSampleType.Trim <> "") Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.DeleteSavedTests", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all information needed to build the Rejected Delayed message for each Order Test in a group of LIS Saved WS that have to be deleted 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with information needed to build the Rejected Delayed message
        '''          for each Order Test in a group of LIS Saved WS that have to be deleted from LIS Utilities Screen</returns>
        ''' <remarks>
        ''' Created by: TR 23/04/2013
        ''' </remarks>
        Public Function GetAllLISOrderTestToReject(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SOT.AwosID, SOT.TestID, SOT.TestType, SOT.SampleClass, SOT.ExternalQC, SOT.SpecimenID," & vbCrLf & _
                                                       " SOT.SampleType, SOT.ESPatientID, SOT.ESOrderID, SOT.LISPatientID, SOT.LISOrderID " & vbCrLf & _
                                                " FROM   tparSavedWSOrderTests SOT INNER JOIN tparSavedWS SWS ON SOT.SavedWSID = SWS.SavedWSID " & vbCrLf & _
                                                " WHERE  SWS.FromLIMS = 1 " & vbCrLf

                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.GetAllOrderTestToReject", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get from the specified LIS Saved WS all Order Tests for the same SampleID and having field CalcTestIDs informed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the LIS Saved WS</param>
        ''' <param name="pSampleID">Patient/Sample Identifier to which the searched Order Tests must belong</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS containing all data of the obtained Order Tests</returns>
        ''' <remarks> 
        ''' Created by:  JC 22/05/2013
        ''' Modified by: SA 24/05/2013 - Changed the DB Template; changed the SQL by adding the filters needed to get only those OrderTests with
        '''                              field CalcTestIDs informed
        ''' </remarks>
        Public Function GetOrderTestsBySampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, ByVal pSampleID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparSavedWSOrderTests " & vbCrLf & _
                                               " WHERE  SavedWSID = " & pSavedWSID.ToString & vbCrLf & _
                                               " AND    SampleClass = 'PATIENT' " & vbCrLf & _
                                               " AND    UPPER(SampleID) = UPPER(N'" & pSampleID.Trim.Replace("'", "''") & "') " & vbCrLf & _
                                               " AND    CalcTestIDs IS NOT NULL " & vbCrLf & _
                                               " AND    CalcTestIDs <> '' " & vbCrLf

                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.GetOrderTestsBySampleID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get from the specified LIS Saved WS, all Specimens with Tests for more than one Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the LIS Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet </returns>
        ''' <remarks>
        ''' Created by:  SA 17/07/2013
        ''' Modified by: SA 09/01/2014 - BT #1398 ==> Changed the query to return only the not NULL SpecimenIDs with several Sample Types (not with several
        '''                                           Tests, which was the value returned for the previous query)
        ''' </remarks>
        Public Function GetSpecimensWithSeveralSampleTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT   SpecimenID, COUNT(DISTINCT SampleType) AS NumSTypes" & vbCrLf & _
                                                " FROM     tparSavedWSOrderTests " & vbCrLf & _
                                                " WHERE    SavedWSID = " & pSavedWSID.ToString & vbCrLf & _
                                                " AND      SpecimenID IS NOT NULL " & vbCrLf & _
                                                " GROUP BY SpecimenID HAVING COUNT(DISTINCT SampleType) > 1 " & vbCrLf

                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.GetSpecimensWithSeveralSampleTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all different Patients/StatFlags in the informed Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed OrdersDS containing all different Patients/StatFlags in the informed Saved WS</returns>
        ''' <remarks>
        ''' Created by:  SA 13/06/2012
        ''' </remarks>
        Public Function ReadAllDifferentPatients(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT SampleID, StatFlag, MIN(CreationOrder) AS CreationOrder, " & vbCrLf & _
                                                               " (CASE WHEN SampleID IS NULL THEN NULL " & vbCrLf & _
                                                                     " WHEN SUBSTRING(SampleID,1, 1)= '#' THEN 'AUTO' " & vbCrLf & _
                                                                     " WHEN SampleID IN (SELECT PatientID FROM tparPatients) THEN 'DB' " & vbCrLf & _
                                                                     " ELSE 'MANUAL' END) AS SampleIDType " & vbCrLf & _
                                                " FROM   tparSavedWSOrderTests " & vbCrLf & _
                                                " WHERE  SavedWSID = " & pSavedWSID.ToString & vbCrLf & _
                                                " AND    SampleClass = 'PATIENT' " & vbCrLf & _
                                                " GROUP BY SampleID, StatFlag " & vbCrLf & _
                                                " ORDER BY StatFlag DESC, CreationOrder " & vbCrLf

                        Dim myOrdersDS As New OrdersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrdersDS.twksOrders)
                            End Using
                        End Using

                        resultData.SetDatos = myOrdersDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ReadAllDifferentPatients", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search if the specified AwosID exists in the group of Order Tests of LIS Saved WS that have not been still added to the active WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAwosID">Awos Identifier to search in the group of Saved WS Order Tests</param>
        ''' <returns>GlobalDataTO containing a SavedWSOrderTestsDS with all information of the saved Order Test for the informed Awos ID</returns>
        ''' <remarks>
        ''' Created by: SA 23/04/2013
        ''' Modified by: SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by AwosID (GUIDs have upper and lower case letters). This is 
        '''                              needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this) 
        ''' </remarks>
        Public Function ReadByAwosID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparSavedWSOrderTests " & vbCrLf & _
                                                " WHERE  AwosID IS NOT NULL AND AwosID = N'" & pAwosID.Trim & "' " & caseSensitiveCollation & vbCrLf

                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ReadByAwosID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of the Order Tests included in the specified Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with the list of the Order Tests
        '''          included in the specified Saved WS</returns> 
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' Modified by:  SA 22/02/2011 - Changed the SQL to get also the PatientIDType for Patient Samples
        '''               SA 18/04/2012 - Changed the function template
        '''               TR 14/03/2013 - Changed the SQL by adding an INNER JOIN with table tparSavedWS to get value of field SavedWSName
        '''               SA 09/05/2013 - Changed the SQL to get also value of new field DeletedTestFlag
        '''               XB 28/08/2014 - Add new field Selected - BT #1868
        '''               AG 17/09/2014 - BA-1869 saved WS will skip those tests configured as not available
        '''               XB 02/10/2014 - Add ORDER BY OT.SavedWSOrderTestID to fix the correct sorting received from LIS when there are Patients not existing yet in database - BA-1963
        ''' </remarks>
        Public Function ReadBySavedWSID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'AG 17/09/2014 - BA-1869 - Rewrite query adding LEFT OUTER JOIN clauses
						Dim cmdText As String = " SELECT OT.SampleClass, OT.SampleID, OT.StatFlag, OT.TestType, OT.TestID, OT.SampleType, OT.TubeType, OT.Selected, " & vbCrLf & _
                               " OT.ReplicatesNumber, OT.ControlID, OT.CreationOrder, OT.TestName, OT.FormulaText, OT.AwosID, OT.SpecimenID, " & vbCrLf & _
                               " OT.ESOrderID, OT.LISOrderID, OT.ESPatientID, OT.LISPatientID, OT.CalcTestIDs, OT.CalcTestNames, SW.SavedWSName, " & vbCrLf & _
                               " (CASE WHEN OT.ExternalQC IS NULL THEN 0 ELSE OT.ExternalQC END) AS ExternalQC,  " & vbCrLf & _
                               " (CASE WHEN OT.DeletedTestFlag IS NULL THEN 0 ELSE OT.DeletedTestFlag END) AS DeletedTestFlag, " & vbCrLf & _
                               " (CASE WHEN OT.SampleID IS NULL THEN NULL  " & vbCrLf & _
                                     "WHEN SUBSTRING(OT.SampleID,1, 1)= '#' THEN 'AUTO' " & vbCrLf & _
                                     "WHEN OT.SampleID IN (SELECT PatientID FROM tparPatients) THEN 'DB' " & vbCrLf & _
                                     "ELSE 'MAN' END) AS PatienTIDType " & vbCrLf & _
                        " FROM   tparSavedWSOrderTests OT INNER JOIN tparSavedWS SW ON OT.SavedWSID = SW.SavedWSID  " & vbCrLf & _
                        " LEFT OUTER JOIN tparTests T ON OT.TestType = 'STD' AND OT.TestID = T.TestID " & vbCrLf & _
                        " LEFT OUTER JOIN tparCalculatedTests  CT ON OT.TestType = 'CALC' AND OT.TestID = CT.CalcTestID  " & vbCrLf & _
                        " LEFT OUTER JOIN tparISETests IT ON OT.TestType = 'ISE' AND OT.TestID = IT.ISETestID " & vbCrLf & _
                        " LEFT OUTER JOIN tparOffSystemTests OFT ON OT.TestType = 'OFFS' AND OT.TestID = OFT.OffSystemTestID " & vbCrLf & _
                        " WHERE  OT.SavedWSID =" & pSavedWSID & vbCrLf & _
                        " AND (CASE OT.TestType WHEN  'STD' THEN T.Available WHEN 'CALC' THEN CT.Available WHEN 'ISE' THEN IT.Available WHEN 'OFFS' THEN OFT.Available END) = 1 " & vbCrLf & _
						" ORDER BY OT.SavedWSOrderTestID " 'BA-1963
                        'AG 17/09/2014 - BA-1869

                        Dim SavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(SavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = SavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ReadBySavedWSID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all OrderTests of the specified SampleClass in the informed Saved WS. Used when a new Analyzer is connected and 
        ''' there is a WorkSession with status different of EMPTY and OPEN
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <param name="pStatFlag">Flag indicating if the OrderTests were requested for Stat (when True) or for Routine (when False)</param>
        ''' <param name="pNewAnalyzerID">Identifier of the new connected Analyzer</param>
        ''' <param name="pSampleID">Patient/Sample Identifier. Optional parameter informed only when SampleClass=PATIENT</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with all OrderTests of the specified SampleClass in 
        '''          the informed Saved WS</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' Modified by: SA 26/06/2012 - For Calibrators, get also fields CalibratorType and SampleTypeAlternative for the Test/SampleType
        '''                              (apply only to Standard Tests). For Blanks and Calibrators, get also field TestVersionNumber 
        '''              SA 25/04/2013 - Changed the SQL query to get also LIS Fields from the Saved WS
        ''' </remarks>
        Public Function ReadBySavedWSIDToChangeAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                                        ByVal pSampleClass As String, ByVal pStatFlag As Boolean, ByVal pNewAnalyzerID As String, _
                                                        Optional ByVal pSampleID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SOT.SampleClass, SOT.SampleID, SOT.StatFlag AS Stat, SOT.TestType, SOT.TestID, SOT.SampleType, SOT.TubeType, " & vbCrLf & _
                                                       " SOT.ReplicatesNumber, SOT.ControlID, SOT.CreationOrder, 'OPEN' AS OrderTestStatus, " & vbCrLf & _
                                                       " N'" & pNewAnalyzerID.Trim & "' AS AnalyzerID, " & vbCrLf & _
                                                      " (CASE WHEN SOT.TestType = 'STD' THEN (SELECT TestVersionNumber FROM tparTests WHERE TestID = SOT.TestID) ELSE NULL END) AS TestVersionNumber, " & vbCrLf & _
                                                      " (CASE WHEN SOT.SampleClass = 'CALIB' AND SOT.TestType = 'STD' THEN TS.CalibratorType ELSE NULL END) AS CalibratorType, " & vbCrLf & _
                                                      " (CASE WHEN SOT.SampleClass = 'CALIB' AND SOT.TestType = 'STD' THEN TS.SampleTypeAlternative ELSE NULL END) AS SampleTypeAlternative, " & vbCrLf & _
                                                      " SOT.AwosID, SOT.SpecimenID, SOT.ESPatientID, SOT.LISPatientID, SOT.ESOrderID, SOT.LISOrderID, SOT.ExternalQC, " & vbCrLf & _
                                                      " (CASE WHEN SOT.AwosID IS NULL THEN 0 ELSE 1 END) AS LISRequest " & vbCrLf & _
                                                " FROM   tparSavedWSOrderTests SOT LEFT OUTER JOIN tparTestSamples TS " & vbCrLf & _
                                                                                   " ON SOT.TestType = 'STD' AND SOT.TestID = TS.TestID AND SOT.SampleType = TS.SampleType " & vbCrLf & _
                                                " WHERE  SOT.SavedWSID   = " & pSavedWSID.ToString & vbCrLf & _
                                                " AND    SOT.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    SOT.StatFlag    = " & Convert.ToInt32(IIf(pStatFlag, 1, 0)) & vbCrLf

                        If (pSampleID.Trim <> String.Empty) Then cmdText &= " AND SOT.SampleID = N'" & pSampleID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " ORDER BY SOT.CreationOrder " & vbCrLf

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ReadBySavedWSIDToChangeAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all different TestType/Test/SampleType contained in the specified Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS</param>
        ''' <param name="pTestType">Type of Tests to get</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with information of all 
        '''          different Standard Test/SampleType contained in the specified Saved WS</returns>
        ''' <remarks>
        ''' Created by:  GDS 05/05/2010
        ''' Modified by: SA  27/05/2010 - Added new parameter to allow get Tests of whatever type from the specified
        '''                               Saved WS; changed the query to get also field FormulaText (needed for Calculated
        '''                               Tests) and to filter data using the informed Test Type 
        '''              SA  18/04/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadTestsByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                        ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT TestID, SampleType, FormulaText " & vbCrLf & _
                                                " FROM   tparSavedWSOrderTests " & vbCrLf & _
                                                " WHERE  SavedWSID = " & pSavedWSID.ToString & vbCrLf & _
                                                " AND    TestType  = '" & pTestType.Trim & "' " & vbCrLf

                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSOrderTestsDS.tparSavedWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.ReadTestsByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a LIS WorkOrder is cancelled from LIS and it exists in a Saved WS (saved manually for user), this Order Test remains in
        ''' the saved WS but as manual work order (LIS fields are set to NULL)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAwosID">Awos Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pSavedWSID">Identifier of a LIS Saved WS. Optional parameter.  When informed, the SQL is filtered by SavedWSID = pSavedWSID; 
        '''                          when not informed (default value), the filter by NOT LIS Saved WS is applied</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/05/2013
        ''' Modified by: SA 10/05/2013 - Changed the SQL 
        '''              SA 24/05/2013 - Added optional parameter pSavedWSID. When informed, the SQL is filtered by SavedWSID = pSavedWSID; when not informed (default
        '''                              value), the filter by NOT LIS Saved WS is applied
        '''              SA 30/05/2013 - Changed the SQL to execute a CASE SENSITIVE comparison by AwosID (GUIDs have upper and lower case letters). This is 
        '''                              needed due to the COLLATION of the DB is defined as CASE INSENSITIVE (which is OK in most cases, but not in this)
        ''' </remarks>
        Public Function UpdateAsManualOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosID As String, ByVal pTestID As Integer, ByVal pTestType As String, _
                                                Optional ByVal pSavedWSID As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tparSavedWSOrderTests " & vbCrLf & _
                                            " SET    AwosID = NULL, SpecimenID = NULL, ESOrderID = NULL, LISOrderID = NULL, " & vbCrLf & _
                                            "        ESPatientID  = NULL, LISPatientID  = NULL, CalcTestIDs  = NULL, CalcTestNames  = NULL, ExternalQC = 0 " & vbCrLf & _
                                            " WHERE  AwosID = '" & pAwosID.Trim & "' " & caseSensitiveCollation & vbCrLf & _
                                            " AND    TestID = " & pTestID.ToString & vbCrLf & _
                                            " AND    TestType = '" & pTestType.Trim & "' " & vbCrLf

                    If (pSavedWSID = -1) Then
                        cmdText &= " AND SavedWSID IN (SELECT SavedWSID FROM tparSavedWS WHERE FromLIMS = 0) " & vbCrLf
                    Else
                        cmdText &= " AND SavedWSID = " & pSavedWSID.ToString
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.UpdateAsManualOrderTest", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Saved WS OrderTest, update value of fields CalcTestIDs and CalcTestNames
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  JCM 22/05/2013
        ''' Modified by: SA  24/05/2013 - Fixed error: the update has to be done by SavedWSOrderTestID, not by SavedWSID
        ''' </remarks>
        Public Function UpdateCalcTestsLinks(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSOrderTestID As Integer, ByVal pCalcTestIDs As String, _
                                             ByVal pCalcTestNames As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tparSavedWSOrderTests " & vbCrLf & _
                                            " SET    CalcTestIDs     = '" & pCalcTestIDs.Trim & "', " & vbCrLf & _
                                                   " CalcTestNames   = N'" & pCalcTestNames.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " WHERE  SavedWSOrderTestID = " & pSavedWSOrderTestID.ToString

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.UpdateCalcTestsLinks", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Test (whatever type) is deleted, all Order Tests for that TestType/TestID in LIS Saved Work Sessions have to be marked as deleted
        ''' (DeletedTestFlag = TRUE) and additionally, the current LIS mapping value for the TestType/TestID, has to be saved in the field TestName 
        ''' to allow building the RejectedDelayed message properly (this message will be built and sent when the LIS Orders Download is executed and the
        ''' LIS Saved WorkSessions are processed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code; optional parameter. When informed, it means that the Test exists but its relation with the 
        '''                           Sample Type has been deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2013
        ''' </remarks>
        Public Function UpdateDeletedTestFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, ByVal pLISValue As String, _
                                              Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tparSavedWSOrderTests " & vbCrLf & _
                                            " SET    DeletedTestFlag = 1, TestName = N'" & pLISValue.Trim & "' " & vbCrLf & _
                                            " WHERE  TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                            " AND    TestID = " & pTestID.ToString & vbCrLf & _
                                            " AND    SavedWSID IN (SELECT SavedWSID FROM tparSavedWS WHERE FromLIMS = 1) " & vbCrLf

                    'Filter data by SampleType when the optional parameter is informed
                    If (pSampleType <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSOrderTestsDAO.UpdateDeletedTestFlag", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace