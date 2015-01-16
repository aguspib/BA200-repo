Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisPatientsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Receive a list of Patient Identifiers and for each one of them, verify if it already exists in Historics Module and when it does not
        ''' exists, get the needed data and create the new Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisPatientsDS">Typed DataSet HisPatientsDS containing the list of PatientIDs to verify if they already exist in Historics
        '''                              Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisPatientDS with the identifier in Historics Module for each PatientID in it</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Public Function CheckPatientsInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisPatientsDS As HisPatientDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientDataDS As PatientsDS
                        Dim myPatientsDelegate As New PatientDelegate

                        Dim auxiliaryDS As HisPatientDS
                        Dim patientsToAddDS As New HisPatientDS
                        Dim myHisPatientsDAO As New thisPatientsDAO

                        For Each patientRow As HisPatientDS.thisPatientsRow In pHisPatientsDS.thisPatients
                            myGlobalDataTO = myHisPatientsDAO.ReadByPatientID(dbConnection, patientRow.PatientID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)
                                If (auxiliaryDS.thisPatients.Rows.Count = 0) Then
                                    'New Patient; get basic data from table tparPatients
                                    myGlobalDataTO = myPatientsDelegate.GetPatientData(dbConnection, patientRow.PatientID)

                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myPatientDataDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                                        If (myPatientDataDS.tparPatients.Rows.Count > 0) Then
                                            patientRow.BeginEdit()
                                            patientRow.FirstName = myPatientDataDS.tparPatients.First.FirstName
                                            patientRow.LastName = myPatientDataDS.tparPatients.First.LastName

                                            If (Not myPatientDataDS.tparPatients.First.IsDateOfBirthNull) Then patientRow.DateOfBirth = myPatientDataDS.tparPatients.First.DateOfBirth
                                            If (Not myPatientDataDS.tparPatients.First.IsGenderNull) Then patientRow.Gender = myPatientDataDS.tparPatients.First.Gender
                                            If (Not myPatientDataDS.tparPatients.First.IsExternalPIDNull) Then patientRow.ExternalPID = myPatientDataDS.tparPatients.First.ExternalPID
                                            If (Not myPatientDataDS.tparPatients.First.IsCommentsNull) Then patientRow.Comments = myPatientDataDS.tparPatients.First.Comments
                                            patientRow.EndEdit()
                                        End If
                                    Else
                                        'Error getting data of the Patient
                                        Exit For
                                    End If

                                    'Copy all Patient data to the auxiliary DS of Patients to add
                                    patientsToAddDS.thisPatients.ImportRow(patientRow)
                                Else
                                    'The patient already exists in Historics Module; inform all fields in the DS
                                    patientRow.BeginEdit()
                                    patientRow.HistPatientID = auxiliaryDS.thisPatients.First.HistPatientID()
                                    patientRow.FirstName = auxiliaryDS.thisPatients.First.FirstName
                                    patientRow.LastName = auxiliaryDS.thisPatients.First.LastName

                                    If (Not auxiliaryDS.thisPatients.First.IsDateOfBirthNull) Then patientRow.DateOfBirth = auxiliaryDS.thisPatients.First.DateOfBirth
                                    If (Not auxiliaryDS.thisPatients.First.IsGenderNull) Then patientRow.Gender = auxiliaryDS.thisPatients.First.Gender
                                    If (Not auxiliaryDS.thisPatients.First.IsExternalPIDNull) Then patientRow.ExternalPID = auxiliaryDS.thisPatients.First.ExternalPID
                                    If (Not auxiliaryDS.thisPatients.First.IsCommentsNull) Then patientRow.Comments = auxiliaryDS.thisPatients.First.Comments
                                    patientRow.EndEdit()
                                End If
                            Else
                                'Error verifying if Patient already exists in Historics Module
                                Exit For
                            End If
                        Next

                        'Add to Historics Module all new Patients
                        If (Not myGlobalDataTO.HasError AndAlso patientsToAddDS.thisPatients.Rows.Count > 0) Then
                            myGlobalDataTO = myHisPatientsDAO.Create(dbConnection, patientsToAddDS)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                patientsToAddDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)

                                'Search the added Patient in the entry DS and inform the PatientID in Historics Module
                                Dim lstPatientToUpdate As List(Of HisPatientDS.thisPatientsRow)
                                For Each patientRow As HisPatientDS.thisPatientsRow In patientsToAddDS.thisPatients
                                    lstPatientToUpdate = (From a As HisPatientDS.thisPatientsRow In pHisPatientsDS.thisPatients _
                                                          Where String.Compare(a.PatientID, patientRow.PatientID, False) = 0 _
                                                          AndAlso a.IsHistPatientIDNull).ToList

                                    If (lstPatientToUpdate.Count = 1) Then
                                        lstPatientToUpdate.First.BeginEdit()
                                        lstPatientToUpdate.First.HistPatientID = patientRow.HistPatientID
                                        lstPatientToUpdate.First.EndEdit()
                                    End If
                                Next
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pHisPatientsDS
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientsDelegate.CheckPatientsInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' </remarks>
        Public Function GetPatientsForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pAppLang As String, _
                                             Optional ByVal pSelectedPatients As List(Of String) = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistPatientsDAO As New thisPatientsDAO
                        resultData = myHistPatientsDAO.GetPatientsForReport(dbConnection, pAppLang, pSelectedPatients)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Patient 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistPatientID">Historical Patient Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisPatientDS with data of the specified Patient</returns>
        ''' <remarks>
        ''' Created by:  JB 22/10/2012
        ''' </remarks>
        Public Function GetPatientData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistPatientID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisPatientsDAO As New thisPatientsDAO
                        resultData = myHisPatientsDAO.ReadByPatientID(dbConnection, pHistPatientID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.GetPatientData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns all patients histroy data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by: JC 11/06/2013
        ''' </remarks>
        Public Function GetAllPatientsHistory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistPatientsDAO As New thisPatientsDAO
                        resultData = myHistPatientsDAO.GetAllPatientsHistory(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use closed Patients
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisPatientsDAO
                        resultData = myDAO.DeleteClosedNotInUse(dbConnection)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Methods for Create RSAT (encrypt/decrypt Patient First and Last Name)"
        ''' <summary>
        ''' For all Patients in the entry DS, replace fields FirstName and LastName for their original values
        ''' This function does not fulfill the template for Delegate functions due to the quantity of data the DAO function has to update can be huge 
        ''' </summary>
        ''' <param name="pPatientsToUpdateDS">Typed DataSet HisPatientDS containing all Patients to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 11/02/2014 - BT #1506
        ''' </remarks>
        Public Function DecryptDataAfterRSAT(ByVal pPatientsToUpdateDS As HisPatientDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myPatientsDAO As New thisPatientsDAO
                myGlobalDataTO = myPatientsDAO.DecryptDataAfterRSAT(pPatientsToUpdateDS)

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.DecryptDataAfterRSAT", EventLogEntryType.Error, False)
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
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New thisPatientsDAO
                        myGlobalDataTO = myPatientsDAO.EncryptDataForRSAT(dbConnection)

                        If (Not myGlobalDataTO.HasError) Then
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientsDelegate.EncryptDataForRSAT", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        Public Function GetAllForRSAT(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New thisPatientsDAO
                        resultData = myPatientsDAO.ReadAllForRSAT(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisPatientDelegate.GetAllForRSAT", EventLogEntryType.Error, False)
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
        ' ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the 
        ' '''                               specified Patient</param>
        ' ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ' ''' <remarks>
        ' ''' Created by: JC 11/06/201
        ' ''' AG 12/06/2013 - if not patients do not call DAO!!!
        ' ''' </remarks>
        'Public Function ModifyPatientsByID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As HisPatientDS.thisPatientsDataTable) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim patientToUpdate As New thisPatientsDAO
        '                If pPatientDetails.Count > 0 Then
        '                    myGlobalDataTO = patientToUpdate.UpdatePatientsByID(dbConnection, pPatientDetails)
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    myGlobalDataTO.SetDatos = pPatientDetails

        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PatientDelegate.ModifyByPatientID", EventLogEntryType.Error, False)
        '    Finally
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace

