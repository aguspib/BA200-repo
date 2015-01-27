Option Strict On
Option Explicit On

Imports System.Text
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class tparPatientsDAO
          

#Region "CRUD"

        ''' <summary>
        ''' Add a new Patient
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the data of the Patient to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: GDS 03/05/2010 - Apply the new Delegate template
        '''              SA  16/06/2010 - Some errors fixed
        '''              SA  27/10/2010 - Added N preffix for multilanguage of field TS_User
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO tparPatients(PatientID, PatientType, FirstName, LastName, DateOfBirth, Gender, ExternalPID, " & _
                                                       " ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime) " & _
                              " VALUES(N'" & pPatientDetails.tparPatients(0).PatientID.Trim & "', " & _
                                      " '" & pPatientDetails.tparPatients(0).PatientType.Trim & "', " & _
                                     " N'" & pPatientDetails.tparPatients(0).FirstName.Trim.Replace("'", "''") & "', " & _
                                     " N'" & pPatientDetails.tparPatients(0).LastName.Trim.Replace("'", "''") & "', "

                    'Control of values of non required fields
                    If (pPatientDetails.tparPatients(0).IsDateOfBirthNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pPatientDetails.tparPatients(0).DateOfBirth.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsGenderNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pPatientDetails.tparPatients(0).Gender.Trim & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsExternalPIDNull) Then
                        cmdText &= " NULL, NULL, "
                    Else
                        cmdText &= " '" & pPatientDetails.tparPatients(0).ExternalPID.Trim.Replace("'", "''") & "', " & _
                                   " '" & pPatientDetails.tparPatients(0).ExternalArrivalDate.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsPerformedByNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " N'" & pPatientDetails.tparPatients(0).PerformedBy.Trim.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsCommentsNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " N'" & pPatientDetails.tparPatients(0).Comments.Trim.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsTS_UserNull) Then
                        'Get the logged User
                        'Dim currentSession As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " N'" & pPatientDetails.tparPatients(0).TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsTS_DateTimeNull) Then
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= " '" & pPatientDetails.tparPatients(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (dataToReturn.AffectedRecords = 1) Then
                        dataToReturn.HasError = False
                        dataToReturn.SetDatos = pPatientDetails
                    Else
                        dataToReturn.HasError = True
                        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 16/06/2010 - Changes to fulfill the new DB template
        '''              SA 27/10/2010 - Added N preffix for multilanguage of field PatientID when it is informed as filter
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparPatients " & vbCrLf & _
                                            " WHERE  UPPER(PatientID)= UPPER(N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf
                    '" WHERE  UPPER(PatientID)= N'" & pPatientID.ToUpper.Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the specified Patient 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <returns>GlobalDataTO containing the data of the specified Patient</returns>
        ''' <remarks>
        ''' Modified by: SA 16/06/2010 - Name changed to Read (old name was ReadByPatientID)
        '''              SA 27/10/2010 - Added N preffix for multilanguage of field PatientID when it is informed as filter
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM   tparPatients " & vbCrLf & _
                                                " WHERE  UPPER(PatientID) = UPPER(N'" & pPatientID.Replace("'", "''") & "') " & vbCrLf
                        '" WHERE  UPPER(PatientID) = N'" & pPatientID.ToUpper.Replace("'", "''") & "' " & vbCrLf

                        Dim myPatientsData As New PatientsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPatientsData.tparPatients)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Patients that fulfill the informed searching criteria
        ''' </summary>
        ''' <param name="pPatientsFilters">Typed DataSet PatientFilterCriteriaDS containing the searching criteria</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with data of all Patients that
        '''          fulfill the informed searching criteria</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 12/03/2010 - Changed the way of opening the DB Connection to fulfill the new template; when a filter
        '''                              by PatientID has been informed, the searching is executed using LIKE instead of the exact value, 
        '''                              and the same for FirstName and LastName
        '''              SG 13/07/2010 - Check that the data values are between the smalldatetime range limits
        '''              SA 19/07/2010 - Changed the format applied to fields Date of Birth From/To when they are informed
        '''              SG 30/07/2010 - Prevent the case that no filter criteria is informed
        '''              SA 27/10/2010 - Added N preffix for multilanguage of fields PatientID, FirstName and LastName when they are informed
        '''                              as filters
        ''' </remarks>
        Public Function ReadAllWithFilters(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientsFilters As PatientFilterCriteriaDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If (Not pPatientsFilters Is Nothing AndAlso pPatientsFilters.tparPatientFilterCriteria.Rows.Count > 0) Then
                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsPatientIDNull) Then
                                cmdText &= " UPPER(PatientID) LIKE UPPER(N'%" & pPatientsFilters.tparPatientFilterCriteria(0).PatientID.ToString.Replace("'", "''") & "%') AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsGenderNull) Then
                                cmdText &= cmdText & " Gender = '" & pPatientsFilters.tparPatientFilterCriteria(0).Gender & "' AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsHisLisNull) Then
                                If (pPatientsFilters.tparPatientFilterCriteria(0).HisLis) Then
                                    cmdText &= " ExternalPID IS NOT NULL AND "
                                Else
                                    cmdText &= " ExternalPID IS NULL AND "
                                End If
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsFirstNameNull) Then
                                cmdText &= " UPPER(FirstName) LIKE UPPER(N'%" & pPatientsFilters.tparPatientFilterCriteria(0).FirstName.ToString.Replace("'", "''") & "%') AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsLastNameNull) Then
                                cmdText &= " UPPER(LastName) LIKE UPPER(N'%" & pPatientsFilters.tparPatientFilterCriteria(0).LastName.ToString.Replace("'", "''") & "%') AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsDobFromNull) Then
                                cmdText &= " DateOfBirth >= '" & Format(pPatientsFilters.tparPatientFilterCriteria(0).DobFrom, "yyyyMMdd") & "' AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsDobToNull) Then
                                cmdText &= " DateOfBirth <= '" & Format(pPatientsFilters.tparPatientFilterCriteria(0).DobTo, "yyyyMMdd") & "' AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsAgeFromNull) Then
                                cmdText &= " Age >= " & pPatientsFilters.tparPatientFilterCriteria(0).AgeFrom.ToString & " AND "
                            End If

                            If (Not pPatientsFilters.tparPatientFilterCriteria(0).IsAgeToNull) Then
                                cmdText &= " Age <= " & pPatientsFilters.tparPatientFilterCriteria(0).AgeTo.ToString & " AND "
                            End If

                            If (cmdText.EndsWith(" AND ")) Then
                                cmdText = cmdText.Substring(0, cmdText.LastIndexOf(" AND "))
                            End If
                        End If

                        'SQL Sentence to get data
                        If (cmdText.Trim <> "") Then
                            cmdText = "SELECT * FROM tparPatients WHERE " & cmdText
                        Else
                            cmdText = "SELECT * FROM tparPatients "
                        End If

                        Dim myPatientsData As New PatientsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPatientsData.tparPatients)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.ReadAllWithFilters", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified PatientID sent by an External LIMS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExternalPID">LIMS Patient Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with data of the specified Patient</returns>
        ''' <remarks>
        ''' Created by:  SA 06/09/2010 
        ''' Modified by: SA 14/09/2010 - Use UPPER in both sides of the WHERE clause
        '''              SA 27/10/2010 - Added N preffix for multilanguage of field ExternalPID when it is informed as filter
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadByExternalPID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExternalPID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparPatients " & vbCrLf & _
                                                " WHERE  UPPER(ExternalPID) = UPPER(N'" & pExternalPID.Replace("'", "''") & "') " & vbCrLf
                        '" WHERE  UPPER(ExternalPID) = N'" & pExternalPID.Replace("'", "''").ToUpper & "' " & vbCrLf

                        Dim myPatientsData As New PatientsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPatientsData.tparPatients)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.ReadByExternalPID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the specified Patient</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: GDS 03/05/2010 - Apply the new Delegate template
        '''              SA  16/06/2010 - Some errors fixed
        '''              SA  27/10/2010 - Added N preffix for multilanguage of field TS_User. Added N preffix for multilanguage of 
        '''                               field PatientID when it is informed as filter
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        '''              AG 13/03/2013 - update also new field PatientType
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparPatients " & _
                                            " SET    FirstName = N'" & pPatientDetails.tparPatients(0).FirstName.ToString.Replace("'", "''") & "', " & _
                                            "        LastName =  N'" & pPatientDetails.tparPatients(0).LastName.ToString.Replace("'", "''") & "', "

                    'AG 13/03/2013 - Update field PatientType
                    If (pPatientDetails.tparPatients(0).IsPatientTypeNull) Then
                        'Do nothing, this field does not allow NULL values, if not informed leave the current value!!
                        'cmdText &= " PatientType = NULL, "
                    Else
                        cmdText &= " PatientType = '" & pPatientDetails.tparPatients(0).PatientType.ToString.Replace("'", "''") & "', "
                    End If
                    'AG 13/03/2013

                    If (pPatientDetails.tparPatients(0).IsGenderNull) Then
                        cmdText &= " Gender = NULL, "
                    Else
                        cmdText &= " Gender = '" & pPatientDetails.tparPatients(0).Gender.ToString.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsDateOfBirthNull) Then
                        cmdText &= " DateOfBirth = NULL, "
                    Else
                        cmdText &= " DateOfBirth = '" & pPatientDetails.tparPatients(0).DateOfBirth.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsExternalPIDNull) Then
                        cmdText &= " ExternalPID = NULL, " & _
                                   " ExternalArrivalDate = NULL, "
                    Else
                        cmdText &= " ExternalPID = '" & pPatientDetails.tparPatients(0).ExternalPID.ToString.Replace("'", "''") & "', " & _
                                   " ExternalArrivalDate = '" & pPatientDetails.tparPatients(0).ExternalArrivalDate.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsCommentsNull) Then
                        cmdText &= " Comments = NULL, "
                    Else
                        cmdText &= " Comments = N'" & pPatientDetails.tparPatients(0).Comments.ToString.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsPerformedByNull) Then
                        cmdText &= " PerformedBy = NULL, "
                    Else
                        cmdText &= " PerformedBy = N'" & pPatientDetails.tparPatients(0).PerformedBy.ToString.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsTS_UserNull) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Trim.ToString & "', "
                    Else
                        cmdText &= " TS_User = N'" & pPatientDetails.tparPatients(0).TS_User.ToString.Replace("'", "''") & "', "
                    End If

                    If (pPatientDetails.tparPatients(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        cmdText &= " TS_DateTime = '" & pPatientDetails.tparPatients(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    cmdText &= " WHERE UPPER(PatientID) = UPPER(N'" & pPatientDetails.tparPatients(0).PatientID.Replace("'", "''") & "') "
                    'cmdText &= " WHERE UPPER(PatientID) = N'" & pPatientDetails.tparPatients(0).PatientID.ToUpper.Replace("'", "''") & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update data of the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the specified Patient</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 15/11/2010 
        ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function UpdateByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS.tparPatientsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparPatients " & vbCrLf & _
                                            " SET    FirstName = N'" & pPatientDetails.FirstName.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                   " LastName =  N'" & pPatientDetails.LastName.ToString.Replace("'", "''") & "'" & vbCrLf & _
                                            " WHERE  UPPER(PatientID) = UPPER(N'" & pPatientDetails.PatientID.Replace("'", "''") & "') " & vbCrLf
                    '" WHERE  UPPER(PatientID) = N'" & pPatientDetails.PatientID.ToUpper.Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.UpdateByPatientID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Returns selected data of the group of selected Patient to show in Patients' Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAppLang">Current application Language</param>
        ''' <param name="pSelectedPatients">List of selected Patient IDs. Optional parameter; when it is not informed, it means all 
        '''                                 existing Patients have to be included in the report</param>
        ''' <remarks>
        ''' Created by:  RH 02/12/2011
        ''' Modified by: SA 15/09/2014 - BA-1921 ==> For each PatientID added to the list of Patients to search using the IN Clause, call function
        '''                                          Replace to avoid exception when the PatientID contains single quotes
        ''' </remarks>
        Public Function GetPatientsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAppLang As String, _
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

                            mySelectedPatients = String.Format(" WHERE P.PatientID IN ({0})", patientsList.ToString().Substring(0, patientsList.Length - 2))
                        End If

                        Dim cmdText As String = String.Format(" SELECT P.PatientID, P.FirstName, P.LastName, P.DateOfBirth, " & _
                                                                    " (CASE WHEN P.Gender IS NULL THEN NULL ELSE MR1.ResourceText END) AS Gender, " & _
                                                                    "  P.PerformedBy, P.Comments" & _
                                                              " FROM   tparPatients P LEFT OUTER JOIN tfmwPreloadedMasterData PMD1 ON P.Gender = PMD1.ItemID " & _
                                                                                                                                " AND PMD1.SubTableID = 'SEX_LIST' " & _
                                                                                    " LEFT OUTER JOIN tfmwMultiLanguageResources MR1 ON PMD1.ResourceID = MR1.ResourceID " & _
                                                                                                                                  " AND MR1.LanguageID = '{0}'" & _
                                                              " {1}" & _
                                                              " ORDER BY P.PatientID", pAppLang, mySelectedPatients)

                        Dim patientsData As New PatientsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(patientsData.tparPatients)
                                resultData.SetDatos = patientsData
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Patients added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Patients that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Change the Query. To set InUse=TRUE, the current query works only for positioned Patient Samples, 
        '''                               and it should set both, positioned and not positioned Patient Samples. Added new optional parameter
        '''                               to reuse this method to set InUse=False for Patient Samples that have been excluded from
        '''                               the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                        ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    If (Not pUpdateForExcluded) Then
                        cmdText = " UPDATE tparPatients " & vbCrLf & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & vbCrLf & _
                                  " WHERE  PatientID IN (SELECT WSOT.PatientID " & vbCrLf & _
                                                       " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                       " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                       " AND    WSOT.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                       " AND    WSOT.SampleClass   = 'PATIENT' " & vbCrLf & _
                                                       " AND    WSOT.PatientID IS NOT NULL) " & vbCrLf
                    Else
                        cmdText = " UPDATE tparPatients " & vbCrLf & _
                                  " SET    InUse = 0 " & vbCrLf & _
                                  " WHERE  PatientID NOT IN (SELECT WSOT.PatientID " & vbCrLf & _
                                                           " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                           " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                           " AND    WSOT.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                           " AND    WSOT.SampleClass   = 'PATIENT' " & vbCrLf & _
                                                           " AND    WSOT.PatientID IS NOT NULL) " & vbCrLf & _
                                  " AND    InUse = 1 " & vbCrLf
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Methods for Create RSAT (encrypt/decrypt Patient First and Last Name)"
        ''' <summary>
        ''' For all Patients in the entry DS, replace fields FirstName and LastName for their original values
        ''' This function does not fulfill the template for DAO functions due to the quantity of data to update can be huge
        ''' </summary>
        ''' <param name="pPatientsToUpdateDS">Typed DataSet PatientsDS containing all Patients to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 11/02/2014 - BT #1506
        ''' Modified by: SA 15/05/2014 - BT #1628 ==> Changed the call to function GetOpenDBTransaction for a call to function GetOpenDBConnection 
        '''                                           (a DB Transaction is managed for each block of 100 updates, then the first one is not needed)
        ''' </remarks>
        Public Function DecryptDataAfterRSAT(ByVal pPatientsToUpdateDS As PatientsDS) As GlobalDataTO
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

                        For Each row As PatientsDS.tparPatientsRow In pPatientsToUpdateDS.tparPatients
                            cmdText.AppendLine(" UPDATE tparPatients " & vbCrLf & _
                                               " SET    FirstName = N'" & row.FirstName.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                               "        LastName =  N'" & row.LastName.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                               " WHERE  PatientID = N'" & row.PatientID.Trim.Replace("'", "''") & "' " & vbCrLf)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.DecryptDataAfterRSAT", EventLogEntryType.Error, False)
            Finally
                If (Not dbConnection Is Nothing AndAlso openDBConn) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all records in Patients Table, replace fields FirstName and LastName for character "-" to hide Patient data when 
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
                    Dim cmdText As String = " UPDATE tparPatients SET FirstName = '-', LastName = '-' "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.EncryptDataForRSAT", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Patients having informed at least one of the Name fields (FirstName and/or LastName). This function is used to get
        ''' all Patients which data have to be encrypted before generate a RSAT
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with all Patients having informed at least one of the name 
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
                        Dim cmdText As String = " SELECT PatientID, FirstName, LastName FROM tparPatients " & vbCrLf & _
                                                " WHERE (FirstName <> '-' OR LastName <> '-') "

                        Dim myPatientsData As New PatientsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPatientsData.tparPatients)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.ReadAllForRSAT", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE"
        ''' <summary>
        ''' Update data of the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the specified Patient</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 15/11/2010 
        ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' JC 11/06/2013 - loop and append line
        ''' AG 12/06/2013 - do not execute query if no items!!!
        ''' </remarks>
        Public Function UpdatePatientsByID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS.tparPatientsDataTable) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim cmdText As StringBuilder

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If pPatientDetails.Count > 0 Then 'AG 12/06/2013
                        cmdText = New StringBuilder()

                        For Each patientRow As PatientsDS.tparPatientsRow In pPatientDetails
                            cmdText.AppendLine(" UPDATE tparPatients " & vbCrLf & _
                                      " SET    FirstName = N'" & patientRow.FirstName.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                             " LastName =  N'" & patientRow.LastName.ToString.Replace("'", "''") & "'" & vbCrLf & _
                                      " WHERE  UPPER(PatientID) = UPPER(N'" & patientRow.PatientID.Replace("'", "''") & "') " & vbCrLf)

                        Next

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.UpdateByPatientID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' All Patients included in an Import from LIMS process are automatically added to Patients table informing 
        '''' ExternalPID and ExternalDateTime, setting PatientType to LIS, and generating an automatic PatientID also 
        '''' used to set mandatory fields FirstName and LastName. The format of this automatic PatientID is "LIMS-" plus 
        '''' a sequential number. This function searches the last generated sequence, adding 1 to it to return the
        '''' sequence value for the next LIMS patient to be created
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing an integer value that will be used as next sequence part of the PatientID
        ''''          automatically generated for patients read from a LIMS Import file</returns>
        '''' <remarks>
        '''' Created by:  SA 14/09/2010 - Not used from 04/08/2011; the PatientID for Patients sent by LIS systems are
        ''''                              generated as "E-" (prefix defined in GlobalConstants) + the ExternalPID
        '''' </remarks>
        'Public Function GetLastLIMSSequence(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT TOP 1 CONVERT(int, SUBSTRING(PATIENTID, 6, 11)) AS NextSequence " & _
        '                          " FROM   tparPatients " & _
        '                          " WHERE  PatientType = 'LIS' " & _
        '                          " ORDER BY NextSequence DESC "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim nextSeq As Integer = 1
        '                Dim dbDataReader As SqlClient.SqlDataReader
        '                dbDataReader = dbCmd.ExecuteReader()
        '                If (dbDataReader.HasRows) Then
        '                    dbDataReader.Read()
        '                    If (Not DBNull.Value.Equals(dbDataReader.Item("NextSequence"))) Then
        '                        nextSeq = CInt(dbDataReader.Item("NextSequence")) + 1
        '                    End If
        '                End If
        '                dbDataReader.Close()

        '                resultData.SetDatos = nextSeq
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparPatientsDAO.GetLastLIMSSequence", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

#End Region
    End Class
End Namespace
