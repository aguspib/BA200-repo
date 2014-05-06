Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class ContaminationsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' According the informed Contamination Type, delete all Contaminations defined (CUVETTES case) or all Contaminations
        ''' in which the specified ReagentID acts as Contaminator.  After that, create all new defined Contaminations
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminationType">Type of Contamination</param>
        ''' <param name="pContaminationsDS">Typed DataSet ContaminationsDS containing the Contaminations to save</param>
        ''' <param name="pReagentContaminatorID">Optional parameter, needed when the Contamination Type is R1 or R2</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/07/2010
        ''' Modified by: AG 15/12/2010 - Changed the function used to delete Cuvette Contaminations
        ''' </remarks>
        Public Function SaveContaminations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationType As String, _
                                           ByVal pContaminationsDS As ContaminationsDS, Optional ByVal pReagentContaminatorID As Integer = -1) _
                                           As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsDAO As New tparContaminationsDAO
                        Select Case (pContaminationType)
                            Case "R1", "R2"
                                'Delete all  Contaminations defined for the informed Contaminator Reagent
                                resultData = myContaminationsDAO.Delete(dbConnection, pReagentContaminatorID, pContaminationType)

                            Case "CUVETTES"
                                'New screen has to delete Cuvette Contamination by ReagentID
                                resultData = myContaminationsDAO.DeleteCuvettes(dbConnection, pReagentContaminatorID, pContaminationType)
                        End Select

                        If (Not resultData.HasError) Then
                            'Add all the defined Contaminations
                            For Each contaminationRow As ContaminationsDS.tparContaminationsRow In pContaminationsDS.tparContaminations.Rows
                                resultData = myContaminationsDAO.Create(dbConnection, contaminationRow)
                                If (resultData.HasError) Then Exit For
                            Next
                        End If

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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.SaveContaminations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Reagent, delete all Contaminations and Contamination Washings in which it is involved
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' MODIFIED BY: TR 10/12/2013 -BT #1415 Remove the implementation of ContaminationWashing delegate is use no more. Table has been delete.
        ''' </remarks>
        Public Function DeleteCascadeContaminationByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsDAO As New tparContaminationsDAO
                        'load the contaminations
                        myGlobalDataTO = myContaminationsDAO.ReadAllContaminationsByReagentID(dbConnection, pReagentID)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myContaminationsDS As New ContaminationsDS
                            myContaminationsDS = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)


                            If (Not myGlobalDataTO.HasError) Then
                                'Delete all contaminations in which the specified Reagent acts as contaminator or contaminated
                                myGlobalDataTO = myContaminationsDAO.DeleteByReagentContaminatorOrContaminated(dbConnection, pReagentID)
                            End If
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.DeleteCascadeContaminationByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all the Contaminations of the specified type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminationType">Type of Contamination</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations of the specified type</returns>
        ''' <remarks>
        ''' Created by:  DL 10/02/2010
        ''' Modified by: SA 22/02/2010 - Parameter ContaminationType was changed from Integer to String
        ''' </remarks>
        Public Function GetContaminationsByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadByContaminationType(dbConnection, pContaminationType)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationsByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Contaminations currently defined (both types R1 and CUVETTES)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all Contaminations (R1 and CUVETTES)
        '''          currently defined</returns>
        ''' <remarks>
        ''' Created by:  TR 30/03/2011
        ''' </remarks>
        Public Function GetAllContaminations(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetAllContaminations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search if there is an R1 Contaminamination between the Reagents informed as Contaminator and Contaminated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentContaminatorID">Identifier of the Contaminator Reagent</param>
        ''' <param name="pReagentContaminatedID">Identifier of the Contaminated Reagent</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with the Contamination between the informed Reagents</returns>
        ''' <remarks>
        ''' Created by:  TR 30/03/2011
        ''' </remarks>
        Public Function GetContaminationByContaminatorIDContaminatedID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentContaminatorID As Integer, _
                                                                       ByVal pReagentContaminatedID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadByContaminatorIDContaminatedID(dbConnection, pReagentContaminatorID, pReagentContaminatedID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationByContaminatorIDContaminatedID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all defined Cuvette Contaminations
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsAuxCuDS with all the defined CuvetteContaminations</returns>
        ''' <remarks>
        ''' Created by:  DL 21/12/2010
        ''' Modified by: SA 14/05/2012 - Multilanguage description has to be informed in fields Step1Desc and Step2Desc, not in Step1 and Step2,
        '''                              which are the fields used for codes 
        ''' </remarks>
        Public Function GetTestContaminatorsCuv(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadTestContaminatorsCuv(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myContaminationsDataDS As ContaminationsAuxCuDS = DirectCast(resultData.SetDatos, ContaminationsAuxCuDS)

                            'Get the current aplication language
                            Dim currentLanguageGlobal As New GlobalBase
                            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

                            'Get the multilanguage description of the needed Washing Solutions
                            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                            For Each contaminationRow As ContaminationsAuxCuDS.tparContaminationsRow In myContaminationsDataDS.tparContaminations.Rows
                                If (Not contaminationRow.IsStep1Null) Then
                                    contaminationRow.Step1Desc = myMultiLangResourcesDelegate.GetResourceText(Nothing, GetResourceID(contaminationRow.Step1), currentLanguage)
                                End If
                                If (Not contaminationRow.IsStep2Null) Then
                                    contaminationRow.Step2Desc = myMultiLangResourcesDelegate.GetResourceText(Nothing, GetResourceID(contaminationRow.Step2), currentLanguage)
                                End If
                            Next
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetTestContaminatorsCuv", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all defined Reagent Contaminations 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsAuxR1DS with all the defined Reagent Contaminations</returns>
        ''' <remarks>
        ''' Created by:  DL 21/12/2010
        ''' </remarks>
        Public Function GetTestContaminatorsR1(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadTestContaminatorsR1(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationsByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Contaminations (of all types) defined for an specific Standard Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations (all types) defined
        '''          for the specified Standard Test</returns>
        ''' <remarks>
        ''' Created by:  SA 29/11/2010
        ''' </remarks>
        Public Function GetTestRNAsContaminator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadTestRNAsContaminator(dbConnection, pTestID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetTestRNAsContaminator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Standard Tests that contaminate the specified Standard Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all found Contaminations</returns>
        ''' <remarks>
        ''' Created by:  SA 29/11/2010
        ''' </remarks>
        Public Function GetTestRNAsContaminated(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadTestRNAsContaminated(dbConnection, pTestID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetTestRNAsContaminated", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Contaminations defined for the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/12/2010
        ''' MODIFIED BY: TR 10/12/2013 -BT #1415 Remove the implementation of ContaminationWashing delegate is use no more. Table has been delete.
        ''' </remarks>
        Public Function DeleteAllByTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete all R1, R2 and/or Contaminations defined for the selected Test
                        If (Not resultData.HasError) Then
                            Dim myContaminationsDAO As New tparContaminationsDAO
                            resultData = myContaminationsDAO.DeleteAllByTest(dbConnection, pTestID)
                        End If

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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.DeleteAllByTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is a R1 Contamination between the informed Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminatorTestID">Identifier of the Contaminator Test</param>
        ''' <param name="pContaminatedTestID">Identifier of the Contaminated Test</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the Reagent of the Contaminator Test contaminates 
        '''          the Reagent of the Contaminated Test</returns>
        ''' <remarks>
        ''' Created by:  SA 24/10/2011
        ''' </remarks>
        Public Function GetContaminationsBetweenTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorTestID As Integer, _
                                                       ByVal pContaminatedTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsDAO As New tparContaminationsDAO
                        resultData = myContaminationsDAO.ReadContaminationsBetweenTests(dbConnection, pContaminatorTestID, pContaminatedTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestReagentsDS As TestReagentsDS = DirectCast(resultData.SetDatos, TestReagentsDS)

                            resultData.SetDatos = (myTestReagentsDS.tparTestReagents.Rows.Count > 0)
                            resultData.HasError = False
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationsBetweenTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get Contaminations By the testid
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 13/12/2011</remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myContaminationsDAO As New tparContaminationsDAO
                        resultData = myContaminationsDAO.ReadByTestID(dbConnection, pTestID)


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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Returns Contaminations info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="AppLang">Application Language</param>
        ''' <remarks>
        ''' Created by: RH 21/12/2011
        '''        
        ''' </remarks>
        Public Function GetContaminationsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsDAO As New tparContaminationsDAO
                        resultData = myContaminationsDAO.GetContaminationsForReport(dbConnection, AppLang)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationsForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get the info by the contaminator Reagent Name and the contaminated reagent name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pContaminatorReagtName"></param>
        '''  ''' <param name="pContaminatedReagName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 11/02/2013
        ''' </remarks>
        Public Function ReadByReagentsName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagtName As String, _
                                          pContaminatedReagName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myContaminationsDAO As New tparContaminationsDAO
                        resultData = myContaminationsDAO.ReadByReagentsName(dbConnection, pContaminatorReagtName, pContaminatedReagName)
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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.ReadByReagentsName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the info by the TestName (TestContaminaCuvette).
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 12/02/2013
        ''' </remarks>
        Public Function ReadByTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myContaminationsDAO As New tparContaminationsDAO
                        resultData = myContaminationsDAO.ReadByTestName(dbConnection, pTestName)


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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the number of contaminations for a given pWashingSolution in a WS
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWashingSolution"></param>
        ''' <returns></returns>
        ''' <remarks>JV #1358 08/11/2013</remarks>
        Public Function VerifyWashingSolutionR2(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pWashingSolution As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.VerifyWashingSolutionR2(dbConnection, pWorkSessionID, pWashingSolution)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.VerifyWashingSolutionR2", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region


#Region "Private Method"

        ''' <summary>
        ''' Get the resource id by the item id
        ''' </summary>
        ''' <param name="pCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 14/12/2011
        ''' </remarks>
        Private Function GetResourceID(ByVal pCode As String) As String
            Dim myResourceId As String = ""
            Try
                Select Case pCode
                    Case "WASHSOL1"
                        myResourceId = "PMD_WASHING_SOLUTIONS_WASHSOL1"
                        Exit Select
                    Case "WASHSOL2"
                        myResourceId = "PMD_WASHING_SOLUTIONS_WASHSOL2"
                        Exit Select
                    Case "WASHSOL4"
                        myResourceId = "PMD_WASHING_SOLUTIONS_WASHSOL3"
                        Exit Select
                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetResourceID", EventLogEntryType.Error, False)
            End Try
            Return myResourceId
        End Function
#End Region

#Region "TO REVIEW - DELETE - USED FOR THE PREVIOUS FORM OR NOT USED"
        ''' <summary>
        ''' USED FOR THE PREVIOUS CONTAMINATIONS FORM
        ''' According the informed Contamination Type, delete all Contaminations defined (CUVETTES case) or all Contaminations
        ''' in which the specified ReagentID acts as Contaminator.  After that, create all new defined Contaminations
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminationType">Type of Contamination</param>
        ''' <param name="pContaminationsDS">Typed DataSet ContaminationsDS containing the Contaminations to save</param>
        ''' <param name="pReagentContaminatorID">Optional parameter, needed when the Contamination Type is R1 or R2</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/07/2010
        ''' </remarks>
        Public Function DeleteContaminations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationType As String, _
                                             ByVal pContaminationsDS As ContaminationsDS, Optional ByVal pReagentContaminatorID As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsDAO As New tparContaminationsDAO
                        Select Case (pContaminationType)
                            Case "R1", "R2"
                                'Delete all  Contaminations defined for the informed Contaminator Reagent
                                resultData = myContaminationsDAO.Delete(dbConnection, pReagentContaminatorID, pContaminationType)

                            Case "CUVETTES"
                                'Delete all Cuvettes Contaminations
                                resultData = myContaminationsDAO.DeleteByContaminationType(dbConnection, pContaminationType)
                        End Select

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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.DeleteContaminations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' USED FOR THE PREVIOUS CONTAMINATIONS FORM
        ''' Search all the Contaminations of the informed type in which the specified Reagent acts as contaminator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentContaminatorID"></param>
        ''' <param name="pContaminationType">Type of Contaminations</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations of the informed type 
        '''          in which the specified Reagent acts as contaminator</returns>
        ''' <remarks>
        ''' Created by:  DL 10/02/2010
        ''' Modified by: SA 22/02/2010 - Parameter ContaminationType was changed from Integer to String
        ''' </remarks>
        Public Function GetContaminationsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentContaminatorID As Integer, _
                                                     ByVal pContaminationType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim contaminationsDataDAO As New tparContaminationsDAO
                        resultData = contaminationsDataDAO.ReadByContaminatorIDAndType(dbConnection, pReagentContaminatorID, pContaminationType)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationsByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pContaminationDataRow"></param>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: TR 30/03/2011</remarks>
        'Public Function DeletebyContaminatorIDContamintedID(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                                   ByVal pContaminationDataRow As ContaminationsDS.tparContaminationsRow) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myContaminationsDAO As New tparContaminationsDAO
        '                Select Case (pContaminationDataRow.ContaminationType)
        '                    Case "R1", "R2"
        '                        'Delete all  Contaminations defined for the informed Contaminator Reagent
        '                        resultData = myContaminationsDAO.DeletebyContaminatorIDContamintedID(dbConnection, _
        '                                                               pContaminationDataRow.ReagentContaminatorID, _
        '                                                               pContaminationDataRow.ReagentContaminatedID)

        '                    Case "CUVETTES"
        '                        ''Delete all Cuvettes Contaminations
        '                        resultData = myContaminationsDAO.DeleteByContaminationTypeAndCuvetteID(dbConnection, _
        '                                                                pContaminationDataRow.TestContaminaCuvetteID, _
        '                                                                        pContaminationDataRow.ContaminationType)
        '                End Select

        '                If (Not resultData.HasError) Then
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
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.DeleteContaminations", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pTestContaminaCuvette"></param>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: TR 30/03/2011</remarks>
        'Public Function GetContaminationByTestContaminaCuvette(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                                       ByVal pTestContaminaCuvette As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim contaminationsDataDAO As New tparContaminationsDAO
        '                resultData = contaminationsDataDAO.ReadByTestContaminaCuvetteID(dbConnection, pTestContaminaCuvette)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.GetContaminationByTestContaminaCuvette", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace


