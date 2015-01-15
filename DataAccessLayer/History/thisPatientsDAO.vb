Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisPatientsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Add a list of Patients to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisPatientDS">Typed DataSet HisPatientDS containing all Patients to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisPatientsDS with all created Patients with the generated HistPatientID</returns>
        ''' <remarks>
        ''' Created by:  TR 16/02/2012
        ''' Modified by: SA 22/02/2012 - Removed field DeletePatient from INSERT, it has a default value. Added N prefix also for field ExternalPID
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisPatientDS As HisPatientDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim newHistPatientID As Integer = -1

                    For Each hisPatientRow As HisPatientDS.thisPatientsRow In pHisPatientDS.thisPatients.Rows
                        newHistPatientID = -1
                        cmdText.Append(" INSERT INTO thisPatients (PatientID, FirstName, LastName, DateOfBirth, Gender, ")
                        cmdText.Append(" ExternalPID, Comments) VALUES (")

                        cmdText.AppendFormat("N'{0}', N'{1}', N'{2}'", hisPatientRow.PatientID.Replace("'", "''"), _
                                             hisPatientRow.FirstName.Replace("'", "''"), hisPatientRow.LastName.Replace("'", "''"))

                        If (hisPatientRow.IsDateOfBirthNull) Then
                            cmdText.Append(", NULL ")
                        Else
                            cmdText.Append(", '" & hisPatientRow.DateOfBirth.ToString("yyyyMMdd HH:mm:ss") & "' ")
                        End If

                        If (hisPatientRow.IsGenderNull OrElse hisPatientRow.Gender.Trim = String.Empty) Then
                            cmdText.Append(", NULL ")
                        Else
                            cmdText.Append(", '" & hisPatientRow.Gender & "' ")
                        End If

                        If (hisPatientRow.IsExternalPIDNull OrElse hisPatientRow.ExternalPID.Trim = String.Empty) Then
                            cmdText.Append(", NULL ")
                        Else
                            cmdText.Append(", N'" & hisPatientRow.ExternalPID.Replace("'", "''") & "' ")
                        End If

                        If (hisPatientRow.IsCommentsNull OrElse hisPatientRow.Comments.Trim = String.Empty) Then
                            cmdText.Append(", NULL ")
                        Else
                            cmdText.Append(", N'" & hisPatientRow.Comments.Replace("'", "''") & "' ")
                        End If

                        'Add the last parenthesis and the sentence needed to get the ID automatically generated
                        cmdText.Append(") ")
                        cmdText.Append(" SELECT SCOPE_IDENTITY() ")

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            newHistPatientID = CType(dbCmd.ExecuteScalar(), Integer)
                            If (newHistPatientID > 0) Then
                                hisPatientRow.HistPatientID = newHistPatientID
                            End If

                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        End Using
                    Next

                    myGlobalDataTO.SetDatos = pHisPatientDS
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a Patient is deleted in the application, if it exists in the corresponding table in Historics Module, then it is marked as 
        ''' closed by updating field ClosedPatient to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistPatientID">Patient Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2012
        ''' </remarks>
        Public Function ClosePatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistPatientID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisPatients SET ClosedPatient = 1 " & vbCrLf & _
                                            " WHERE  HistPatientID = " & pHistPatientID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.ClosePatient", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all not in use closed Patients
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisPatients " & vbCrLf & _
                                            " WHERE  ClosedPatient  = 1 " & vbCrLf & _
                                            " AND    HistPatientID  NOT IN (SELECT HistPatientID   FROM thisWSOrderTests " & vbCrLf & _
                                                                          " WHERE   SampleClass = 'PATIENT') " & vbCrLf

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
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns selected data of the group of selected Patient to show in Patients' Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAppLang">Current application Language</param>
        ''' <param name="pSelectedPatients">List of selected Patient IDs. Optional parameter; when it is not informed, it means all 
        '''                                 existing Patients have to be included in the report</param>
        ''' <remarks>
        ''' Created by: DL 24/10/2012
        ''' Modified by: SA 15/09/2014 - BA-1921 ==> For each PatientID added to the list of Patients to search using the IN Clause, call function
        '''                                          Replace to avoid exception when the PatientID contains single quotes
        ''' </remarks>
        Public Function GetPatientsForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pAppLang As String, _
                                             Optional ByVal pSelectedPatients As List(Of String) = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySelectedPatients As String = String.Empty

                        If (Not pSelectedPatients Is Nothing AndAlso pSelectedPatients.Count > 0) Then
                            Dim patientsList As New StringBuilder()
                            For Each p As String In pSelectedPatients
                                patientsList.AppendFormat("N'{0}', ", p.Trim.Replace("'", "''"))
                            Next

                            mySelectedPatients = String.Format("   WHERE P.PatientID IN ({0})", patientsList.ToString().Substring(0, patientsList.Length - 2))
                        End If

                        Dim cmdText As String = String.Empty
                        cmdText &= "  SELECT P.PatientID" & vbCrLf
                        cmdText &= "        ,P.FirstName" & vbCrLf
                        cmdText &= "        ,P.LastName" & vbCrLf
                        cmdText &= "        ,P.DateOfBirth" & vbCrLf
                        'cmdtext &= "       ,P.PerformedBy" & vbcrlf
                        cmdText &= "        ,P.Gender" & vbCrLf
                        cmdText &= "        ,P.Comments" & vbCrLf
                        cmdText &= "    FROM thisPatients P LEFT OUTER JOIN tfmwPreloadedMasterData PMD1 ON P.Gender = PMD1.ItemID AND PMD1.SubTableID = 'SEX_LIST'" & vbCrLf
                        cmdText &= "                        LEFT OUTER JOIN tfmwMultiLanguageResources MR1 ON PMD1.ResourceID = MR1.ResourceID AND MR1.LanguageID = '{0}'" & vbCrLf
                        cmdText &= "{1}" & vbCrLf
                        cmdText &= "ORDER BY P.PatientID"
                        cmdText = String.Format(cmdText, pAppLang, mySelectedPatients)

                        Dim myHisPatientDS As New HisPatientDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisPatientDS.thisPatients)
                                resultData.SetDatos = myHisPatientDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the informed PatientID already exists in Historics Module and in this case, get the its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisPatientDS with all data saved for the Patient in Historics Module</returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2012
        ''' Modified by: SA 22/02/2012 - Added prefix N and replacement of single quotes in filter by PatientID. Removed optional parameter
        '''                              pDeletedPatient due to this function search only "open" Patients. Added Finally section  
        ''' </remarks>
        Public Function ReadByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HP.* FROM thisPatients HP " & vbCrLf & _
                                                " WHERE  HP.PatientID = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    HP.ClosedPatient = 0 " & vbCrLf

                        Dim myHisPatientDS As New HisPatientDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisPatientDS.thisPatients)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisPatientDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.ReadByPatientID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Patient (FirstName, LastName, DateOfBirth, Gender and/or Comments) is changed in Patients Programming Screen or 
        ''' in the process of LIMS Import, values are updated for the corresponding record in the Historics Module table (the one having the same 
        ''' PatientID and ClosedPatient=False)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientDS">Typed DataSet PatientDS containing the Patient data to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2012
        ''' Modified by: SA 22/02/2012 - Removed the For/Next. Filter ClosedPatient=0 is fixed. Name of the function changed to UpdateByPatientID,
        '''                              this is not the CRUD Update. Field ExternalPID is never updated
        ''' </remarks>
        Public Function UpdateByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDS As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisPatients " & vbCrLf & _
                                            " SET    FirstName = N'" & pPatientDS.tparPatients.First.FirstName.Replace("'", "''") & "', " & vbCrLf & _
                                                   " LastName  = N'" & pPatientDS.tparPatients.First.LastName.Replace("'", "''") & "', " & vbCrLf

                    'Values for not mandatory fields...
                    If (pPatientDS.tparPatients.First.IsDateOfBirthNull) Then
                        cmdText &= " DateOfBirth = NULL, "
                    Else
                        cmdText &= " DateOfBirth = '" & pPatientDS.tparPatients.First.DateOfBirth.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If (pPatientDS.tparPatients.First.IsGenderNull) Then
                        cmdText &= " Gender = NULL, "
                    Else
                        cmdText &= " Gender = '" & pPatientDS.tparPatients.First.Gender & "', "
                    End If

                    If (pPatientDS.tparPatients.First.IsCommentsNull) Then
                        cmdText &= " Comments = NULL "
                    Else
                        cmdText &= " Comments = N'" & pPatientDS.tparPatients.First.Comments.Replace("'", "''") & "' "
                    End If

                    'Finally, link the filters
                    cmdText &= " WHERE PatientID = N'" & pPatientDS.tparPatients.First.PatientID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " AND   ClosedPatient = 0 "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.UpdateByPatientID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Returns all history patient data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by: JC 11/06/2013
        ''' </remarks>
        Public Function GetAllPatientsHistory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySelectedPatients As String = String.Empty

                        Dim cmdText As String = String.Empty

                        cmdText &= "  SELECT * FROM thisPatients "

                        Dim myHisPatientDS As New HisPatientDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisPatientDS.thisPatients)
                                resultData.SetDatos = myHisPatientDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Methods for Create RSAT (encrypt/decrypt Patient First and Last Name)"
        ''' <summary>
        ''' For all Patients in the entry DS, replace fields FirstName and LastName for their original values 
        ''' This function does not fulfill the template for DAO functions due to the quantity of data to update can be huge
        ''' </summary>
        ''' <param name="pPatientsToUpdateDS">Typed DataSet HisPatientDS containing all Patients to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 11/02/2014 - BT #1506
        ''' Modified by: SA 15/05/2014 - BT #1628 ==> Changed the call to function GetOpenDBTransaction for a call to function GetOpenDBConnection 
        '''                                           (a DB Transaction is managed for each block of 100 updates, then the first one is not needed)
        ''' </remarks>
        Public Function DecryptDataAfterRSAT(ByVal pPatientsToUpdateDS As HisPatientDS) As GlobalDataTO
            Dim openDBConn As Boolean = False
            Dim openDBTran As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim i As Integer = 0
                Dim maxInserts As Integer = 100
                Dim cmdText As New StringBuilder()

                myGlobalDataTO = GetOpenDBConnection(dbConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        openDBConn = True

                        For Each row As HisPatientDS.thisPatientsRow In pPatientsToUpdateDS.thisPatients
                            cmdText.AppendLine(" UPDATE thisPatients " & vbCrLf & _
                                               " SET    FirstName = N'" & row.FirstName.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                               "        LastName =  N'" & row.LastName.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                               " WHERE  HistPatientID = " & row.HistPatientID & vbCrLf)

                            'Increment the sentences counter and verify if the max has been reached
                            i += 1
                            If (i = maxInserts) Then
                                DAOBase.BeginTransaction(dbConnection)
                                openDBTran = True

                                'Execute the SQL scripts
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, dbConnection)
                                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using

                                If (Not myGlobalDataTO.HasError) Then
                                    DAOBase.CommitTransaction(dbConnection)
                                Else
                                    DAOBase.RollbackTransaction(dbConnection)
                                End If
                                openDBTran = False

                                'Initialize the counter and the StringBuilder
                                i = 0
                                cmdText.Remove(0, cmdText.Length)
                            End If
                        Next

                        If (Not myGlobalDataTO.HasError) Then
                            If (cmdText.Length > 0) Then
                                DAOBase.BeginTransaction(dbConnection)
                                openDBTran = True

                                'Execute the remaining scripts
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, dbConnection)
                                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using

                                If (Not myGlobalDataTO.HasError) Then
                                    DAOBase.CommitTransaction(dbConnection)
                                Else
                                    DAOBase.RollbackTransaction(dbConnection)
                                End If
                                openDBTran = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                If (Not dbConnection Is Nothing AndAlso openDBTran) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.DecryptDataAfterRSAT", EventLogEntryType.Error, False)
            Finally
                If (Not dbConnection Is Nothing AndAlso openDBConn) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all records in Historic Patients Table, replace fields FirstName and LastName for character "-" to hide Patient data when 
        ''' a RSAT is generated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 13/02/2014 - BT #1506
        ''' </remarks>
        Public Function EncryptDataForRSAT(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE thisPatients SET FirstName = '-', LastName = '-' "

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
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.EncryptDataForRSAT", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Patients having informed at least one of the Name fields (FirstName and/or LastName). This function is used to get
        ''' all Patients which data have to be encrypted before generate a RSAT
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisPatientDS with all Patients having informed at least one of the name 
        '''          fields (FirstName and/or LastName)</returns>
        ''' <remarks>
        ''' Created by:  SA 11/02/2014 - BT #1506
        ''' </remarks>
        Public Function ReadAllForRSAT(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HistPatientID, PatientID, FirstName, LastName FROM thisPatients " & vbCrLf & _
                                                " WHERE  FirstName <> '-' OR LastName <> '-' "

                        Dim myPatientsData As New HisPatientDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPatientsData.thisPatients)
                            End Using
                        End Using

                        resultData.SetDatos = myPatientsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisPatientsDAO.ReadAllForRSAT", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE"
        ' ''' <summary>
        ' ''' Update data of the specified Patient
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the specified Patient</param>
        ' ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ' ''' <remarks>
        ' ''' Created by: JCM 11/06/2013
        ' ''' AG 12/06/2013 - if not patients do not execute any query!! (system error)
        ' ''' </remarks>
        'Public Function UpdatePatientsByID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As HisPatientDS.thisPatientsDataTable) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim cmdText As StringBuilder

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            If pPatientDetails.Count > 0 Then
        '                cmdText = New StringBuilder()

        '                For Each patientRow As HisPatientDS.thisPatientsRow In pPatientDetails

        '                    cmdText.AppendLine("   UPDATE   thisPatients ")
        '                    cmdText.AppendLine("      SET   FirstName = N'" & patientRow.FirstName.Replace("'", "''") & "'")
        '                    cmdText.AppendLine("          , LastName  = N'" & patientRow.LastName.Replace("'", "''") & "'")

        '                    If patientRow.IsDateOfBirthNull Then
        '                        cmdText.AppendLine("      , DateOfBirth = NULL ")
        '                    Else
        '                        cmdText.AppendLine("      , DateOfBirth = '" & patientRow.DateOfBirth.ToString("yyyyMMdd HH:mm:ss") & "'")
        '                    End If

        '                    If patientRow.IsGenderNull Then
        '                        cmdText.AppendLine("      , Gender = NULL ")
        '                    Else
        '                        cmdText.AppendLine("      , Gender = '" & patientRow.Gender & "'")
        '                    End If

        '                    If patientRow.IsCommentsNull Then
        '                        cmdText.AppendLine("      , Comments = NULL ")
        '                    Else
        '                        cmdText.AppendLine("      , Comments = N'" & patientRow.Comments.Replace("'", "''") & "' ")
        '                    End If

        '                    cmdText.AppendLine("    WHERE   PatientID = N'" & patientRow.PatientID.Trim.Replace("'", "''") & "'")
        '                    cmdText.AppendLine("      AND   ClosedPatient = 0 ")

        '                Next

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
        '                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                    myGlobalDataTO.HasError = False
        '                End Using
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparPatientsDAO.UpdateByPatientID", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace

