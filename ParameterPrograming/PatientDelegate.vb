Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants

Namespace Biosystems.Ax00.BL
    Public Class PatientDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Add a new Patient
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the data of the Patient to add</param>
        ''' <param name="pFromXmlMessage">FALSE add from screen or from file import | TRUE add from embedded Synapse library xml message</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: GDS 03/05/2010 - Apply the new Delegate template
        '''              SA  16/06/2010 - Some errors fixed
        '''              SA  14/09/2010 - Added special management for LIS Patients: the PatientID has to be 
        '''                               automatically generated
        '''              SA  04/08/2011 - Changed management of LIS Patients: the PatientID is generated as LIMS_PATIENTID_PREFIX + ExternalPID
        '''              AG  13/03/2013 - Added new parameter pFromXmlMessage. When True, call directly the Create method
        '''              SA  04/06/2013 - Added call to new function UpdateOrdersByPatientID to verify if there are Orders in the active WS defined
        '''                               for a SampleID (in uppercase) equal to the created PatientID, and in this case, inform for them field PatientID
        '''                               and set to NULL field SampleID
        ''' </remarks>
        Public Function Add(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS, ByVal pFromXmlMessage As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim patientToAdd As New TparPatientsDAO

                        If (Not pFromXmlMessage) Then 'AG 13/03/2013 - FALSE execute next code, TRUE call directly the Create method
                            If (pPatientDetails.tparPatients(0).PatientType = "LIS") Then
                                'Generate the automatic PatientID
                                Dim nextID As String = LIMS_PATIENTID_PREFIX & pPatientDetails.tparPatients(0).ExternalPID

                                pPatientDetails.tparPatients(0).BeginEdit()
                                pPatientDetails.tparPatients(0).PatientID = nextID
                                pPatientDetails.tparPatients(0).FirstName = nextID
                                pPatientDetails.tparPatients(0).LastName = nextID
                                pPatientDetails.tparPatients(0).EndEdit()
                            Else
                                'Validate if there is another Patient with the same ID
                                myGlobalDataTO = patientToAdd.Read(dbConnection, pPatientDetails.tparPatients(0).PatientID)
                                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myPatientDS As New PatientsDS
                                    myPatientDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)
                                    If (myPatientDS.tparPatients.Rows.Count = 1) Then
                                        'Duplicated PatientID
                                        myGlobalDataTO.HasError = True
                                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_PATIENT_ID.ToString
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Add the new Patient
                            myGlobalDataTO = patientToAdd.Create(dbConnection, pPatientDetails)
                            If (Not myGlobalDataTO.HasError) Then
                                'Verify if in the active WS there are Orders defined by UPPER(SampleID) = PatientID created, and in this case,
                                'inform field PatientID and set to NULL field SampleID (update has to be done in table twksOrders and, when needed,
                                'in table twksWSRequiredElements (if the affected Patient Sample has been already sent to positioning)
                                myGlobalDataTO = UpdateOrdersByPatientID(dbConnection, pPatientDetails.tparPatients(0).PatientID)
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO.SetDatos = pPatientDetails

                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.Add", EventLogEntryType.Error, False)
            Finally
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if in the active WS there are Orders defined by UPPER(SampleID) = PatientID created, and in this case,
        ''' inform field PatientID and set to NULL field SampleID (update has to be done in table twksOrders and, when needed,
        ''' in table twksWSRequiredElements (if the affected Patient Sample has been already sent to positioning)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 04/06/2013
        ''' </remarks>
        Public Function UpdateOrdersByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '** NOTE: It is not possible to call the Delegate Classes in WorkSession Project due to circular references; DAO Classes are used

                        'Verify if there are Orders in the active WS with UPPER(SampleID) = PatientID, and in this case, update field PatientID 
                        'and set field SampleID to NULL (call function ModifyOrder for every Order)
                        Dim mytwksOrdersDAO As New TwksOrdersDAO

                        myGlobalDataTO = mytwksOrdersDAO.ReadBySampleID(dbConnection, pPatientID)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

                            Dim myAuxOrderDS As New OrdersDS
                            Dim mytwksWSReqElemDAO As New twksWSRequiredElementsDAO

                            For Each myOrder As OrdersDS.twksOrdersRow In myOrdersDS.twksOrders.Rows
                                myOrder.BeginEdit()
                                myOrder.PatientID = pPatientID
                                myOrder.SetSampleIDNull()
                                myOrder.EndEdit()

                                myAuxOrderDS.Clear()
                                myAuxOrderDS.twksOrders.ImportRow(myOrder)

                                myGlobalDataTO = mytwksOrdersDAO.Update(dbConnection, myAuxOrderDS)
                                If (myGlobalDataTO.HasError) Then Exit For

                                myGlobalDataTO = mytwksWSReqElemDAO.ChangeSampleIDToPatientID(dbConnection, pPatientID)
                                If (myGlobalDataTO.HasError) Then Exit For
                            Next
                        End If

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.UpdateOrdersByPatientID", EventLogEntryType.Error, False)
            Finally
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientList">Typed DataSet PatientsDS containing the list of Patients to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 16/06/2010 - Changes to fulfill the new DB template; changes to allow deleting
        '''                              several Patients
        '''              SA 13/09/2012 - Added call to the function that mark as closed the Patient in Historic Module when the Patient exists in it
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientList As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim patientToDelete As New TparPatientsDAO

                        For Each patientRow As PatientsDS.tparPatientsRow In pPatientList.tparPatients.Rows
                            myGlobalDataTO = patientToDelete.Delete(dbConnection, patientRow.PatientID)
                            If (myGlobalDataTO.HasError) Then Exit For

                            If (HISTWorkingMode) Then
                                'If the Patient exists in Historic Module it is marked as Closed
                                myGlobalDataTO = HIST_ClosePatient(dbConnection, patientRow.PatientID)
                                If (myGlobalDataTO.HasError) Then Exit For
                            End If
                        Next

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of patients that fulfill the informed searching criteria
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientsFilters">Typed DataSet PatientFilterCriteriaDS containing values of the informed 
        '''                                Patients' searching fields</param>
        ''' <returns>GlobalDataTO containing the list of Patients that fulfill the informed searching criteria</returns>
        ''' <remarks></remarks>
        Public Function GetListWithFilters(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientsFilters As PatientFilterCriteriaDS) _
                                           As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New TparPatientsDAO
                        resultData = myPatientsDAO.ReadAllWithFilters(dbConnection, pPatientsFilters)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.GetListWithFilters", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Patient 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with data of the specified Patient</returns>
        ''' <remarks></remarks>
        Public Function GetPatientData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New TparPatientsDAO
                        resultData = myPatientsDAO.Read(dbConnection, pPatientID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.GetPatientData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by: RH 02/12/2011
        ''' </remarks>
        Public Function GetPatientsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAppLang As String, _
                                             Optional ByVal pSelectedPatients As List(Of String) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New TparPatientsDAO
                        resultData = myPatientsDAO.GetPatientsForReport(dbConnection, pAppLang, pSelectedPatients)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.GetPatientsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified Patient
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the 
        '''                               specified Patient</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: GDS 03/05/2010 - Apply the new Delegate template
        '''              SA  16/06/2010 - Some errors fixed
        '''              SA  13/09/2012 - Added call to the function that update the Patient's data in Historic Module when the Patient exists in it
        ''' </remarks>
        Public Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim patientToUpdate As New TparPatientsDAO
                        myGlobalDataTO = patientToUpdate.Update(dbConnection, pPatientDetails)

                        If (HISTWorkingMode) Then
                            If (Not myGlobalDataTO.HasError) Then
                                'If the Patient exists in Historic Module, data is updated also in it
                                myGlobalDataTO = HIST_UpdateByPatientID(dbConnection, pPatientDetails)
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO.SetDatos = pPatientDetails

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the specified PatientID sent by an External LIMS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExternalPID">LIMS Patient Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with data of the specified Patient</returns>
        ''' <remarks>
        ''' Created by:  SA 06/09/2010 
        ''' </remarks>
        Public Function ReadByExternalPID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExternalPID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPatientsDAO As New TparPatientsDAO
                        resultData = myPatientsDAO.ReadByExternalPID(dbConnection, pExternalPID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.ReadByExternalPID", EventLogEntryType.Error, False)
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
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Patients 
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TparPatientsDAO
                        myGlobalDataTO = myDAO.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, pFlag, pUpdateForExcluded)

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' When a Patient is deleted in the application, if it exists in the corresponding table in Historics Module, then it is marked as 
        ''' closed by updating field ClosedPatient to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 20/02/2012
        ''' Modified by: SA 13/09/2012 - Method has been moved from class HisPatientsDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisPatients are called
        ''' </remarks>
        Private Function HIST_ClosePatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisPatientsDAO As New thisPatientsDAO
                        myGlobalDataTO = myHisPatientsDAO.ReadByPatientID(dbConnection, pPatientID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisPatientDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)
                            If (auxiliaryDS.thisPatients.Rows.Count > 0) Then
                                myGlobalDataTO = myHisPatientsDAO.ClosePatient(dbConnection, auxiliaryDS.thisPatients.First.HistPatientID)
                            End If
                        End If
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientsDelegate.HIST_ClosePatient", EventLogEntryType.Error, False)
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
        ''' Modified by: SA 13/09/2012 - Method has been moved from class HisPatientsDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisPatients are called
        ''' </remarks>
        Private Function HIST_UpdateByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDS As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisPatientsDAO As New thisPatientsDAO
                        myGlobalDataTO = myHisPatientsDAO.UpdateByPatientID(dbConnection, pPatientDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "PatientsDelegate.HIST_UpdateByPatientID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Methods for Create RSAT (encrypt/decrypt Patient First and Last Name)"
        ''' <summary>
        ''' For all Patients in the entry DS, replace fields FirstName and LastName for their original values 
        ''' This function does not fulfill the template for Delegate functions due to the quantity of data the DAO function has to update can be huge
        ''' </summary>
        ''' <param name="pPatientsToUpdateDS">Typed DataSet PatientsDS containing all Patients to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 11/02/2014 - BT #1506
        ''' </remarks>
        Public Function DecryptDataAfterRSAT(ByVal pPatientsToUpdateDS As PatientsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myPatientsDAO As New TparPatientsDAO
                myGlobalDataTO = myPatientsDAO.DecryptDataAfterRSAT(pPatientsToUpdateDS)
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.DecryptDataAfterRSAT", EventLogEntryType.Error, False)
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
                        Dim myPatientsDAO As New TparPatientsDAO
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
                myLogAcciones.CreateLogActivity(ex.Message, "PatientsDelegate.EncryptDataForRSAT", EventLogEntryType.Error, False)
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
        ''' <returns>GlobalDataTO containing a typed DataSet PatientsDS with all Patients having informed at least one of the name 
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
                        Dim myPatientsDAO As New TparPatientsDAO
                        resultData = myPatientsDAO.ReadAllForRSAT(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PatientsDelegate.GetAllForRSAT", EventLogEntryType.Error, False)
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
        ' ''' Created by: DL 15/11/2010
        ' ''' JC 11/06/2013 - Change parameter from datarow to datatable
        ' ''' AG 12/06/2013 - If no elements in datatable do not call DAO!!!
        ' ''' </remarks>
        'Public Function ModifyPatientsByID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS.tparPatientsDataTable) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim patientToUpdate As New TparPatientsDAO
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
        '        myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.ModifyByPatientID", EventLogEntryType.Error, False)
        '    Finally
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function


        ' ''' <summary>
        ' ''' Update data of the specified Patient
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pPatientDetails">Typed DataSet PatientsDS containing the updated data of the 
        ' '''                               specified Patient</param>
        ' ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ' ''' <remarks>
        ' ''' Created by: DL 15/11/2010
        ' ''' </remarks>
        'Public Function ModifyByPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientDetails As PatientsDS.tparPatientsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim patientToUpdate As New TparPatientsDAO
        '                myGlobalDataTO = patientToUpdate.UpdateByPatientID(dbConnection, pPatientDetails)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "PatientDelegate.ModifyByPatientID", EventLogEntryType.Error, False)
        '    Finally
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace


