Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class ReportTemplatesDelegate

#Region "CRUD"
        ''' <summary>
        ''' Create one or serverar records on tcfgReportTemplates table.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReportTemplatesDS">Data set containing the new Templates to create</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function CreateReportTemplate(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pReportTemplatesDS As ReportTemplatesDS) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                        If pReportTemplatesDS.tcfgReportTemplates.Rows.Count > 0 Then
                            'Validate if new Template will be the default 
                            If pReportTemplatesDS.tcfgReportTemplates.Where(Function(a) a.DefaultTemplate).Count > 0 Then
                                myGlobalDataTO = SetFalseDefaultTemplateByTempOrientation(dbConnection, _
                                                 pReportTemplatesDS.tcfgReportTemplates.Where(Function(a) _
                                                 a.DefaultTemplate).First().TemplateOrientation)
                            End If

                            myGlobalDataTO = myReportTemplateDAO.Create(dbConnection, pReportTemplatesDS)

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
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.CreateReportTemplate", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Defaul Template value by the Template Name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateName">Template Name</param>
        ''' <param name="pDefaultTemplate">True/False</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: TR 22/11/2011</remarks>
        Public Function UpdateDefaultTemplateValueByTempltName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                               ByVal pTemplateName As String, ByVal pDefaultTemplate As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If Not pTemplateName = String.Empty Then
                            Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                            If pDefaultTemplate Then
                                'Get template data to get 
                                myGlobalDataTO = Read(dbConnection, pTemplateName)
                                If Not myGlobalDataTO.HasError Then
                                    Dim myReportTemplateDS As New ReportTemplatesDS
                                    myReportTemplateDS = DirectCast(myGlobalDataTO.SetDatos, ReportTemplatesDS)
                                    If myReportTemplateDS.tcfgReportTemplates.Rows.Count = 1 Then
                                        myGlobalDataTO = SetFalseDefaultTemplateByTempOrientation(dbConnection, myReportTemplateDS.tcfgReportTemplates(0).TemplateOrientation)
                                    End If
                                End If

                            End If

                            myGlobalDataTO = myReportTemplateDAO.UpdateDefaultTemplateValueByTempltName(dbConnection, pTemplateName, pDefaultTemplate)
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
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateDefaultTemplateValueByTempltName", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Defaul Template by the Template Name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewTemplate">Template Name</param>
        ''' <param name="pOldTemplate">Template Name</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011</remarks>
        Public Function UpdateTemplateNameByOldName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                    ByVal pNewTemplate As String, _
                                                    ByVal pOldTemplate As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                        myGlobalDataTO = myReportTemplateDAO.UpdateTemplateName(dbConnection, pNewTemplate, pOldTemplate)

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
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateTemplateNameByOldName", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the Defaul Template by the Template Name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateRow">Template Name</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011</remarks>
        Public Function UpdateDefaultTemplateByTempltName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                          ByVal pTemplateRow As ReportTemplatesDS.tcfgReportTemplatesRow) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                        myGlobalDataTO = myReportTemplateDAO.UpdateDefaultTemplateByTempltName(dbConnection, pTemplateRow)

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
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateDefaultTemplateByTempltName", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set DefaultTemplate value as False by the Template Orientation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateOrientation">Landscape/Portrait</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Private Function SetFalseDefaultTemplateByTempOrientation(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                                  ByVal pTemplateOrientation As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If Not pTemplateOrientation = String.Empty Then
                            Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                            myGlobalDataTO = myReportTemplateDAO.SetFalseDefaultTemplateByTempOrientation(dbConnection, pTemplateOrientation)
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
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.SetFlaseDefaultTemplateByTempOrientation", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the tcfgReportTemplates by the TemplateName.
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pTemplateName">Template Name</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function Read(ByVal pDbConnection As SqlClient.SqlConnection, _
                             ByVal pTemplateName As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                        If pTemplateName <> String.Empty Then
                            myGlobalDataTO = myReportTemplateDAO.Read(dbConnection, pTemplateName)
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete by Template Name.
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pTemplateName">Template name</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function Delete(ByVal pDbConnection As SqlClient.SqlConnection, _
                               ByVal pTemplateName As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If Not pTemplateName = String.Empty Then
                            Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                            'Validate if the template is mark as default
                            myGlobalDataTO = Read(dbConnection, pTemplateName)
                            If Not myGlobalDataTO.HasError Then
                                Dim myReportTemplateDS As New ReportTemplatesDS
                                myReportTemplateDS = DirectCast(myGlobalDataTO.SetDatos, ReportTemplatesDS)
                                If myReportTemplateDS.tcfgReportTemplates.Rows.Count > 0 Then
                                    'Validate if is master Template 
                                    If myReportTemplateDS.tcfgReportTemplates(0).DefaultTemplate Then 'If is Default Template 
                                        'SELECT A Master Template whit the same Template orientation and set it default template.
                                        myGlobalDataTO = myReportTemplateDAO.ReadByTemplateOrientation(dbConnection, _
                                                         myReportTemplateDS.tcfgReportTemplates(0).TemplateOrientation, True)
                                        If Not myGlobalDataTO.HasError Then
                                            myReportTemplateDS = DirectCast(myGlobalDataTO.SetDatos, ReportTemplatesDS)

                                            If myReportTemplateDS.tcfgReportTemplates.Rows.Count > 0 Then
                                                Dim myNewDefaultTemplate As String = String.Empty
                                                'Select a diferent template name 
                                                myNewDefaultTemplate = myReportTemplateDS.tcfgReportTemplates.Where(Function(a) a.TemplateName <> pTemplateName).First().TemplateName

                                                'Select first element and set it as default.
                                                myGlobalDataTO = UpdateDefaultTemplateValueByTempltName(dbConnection, myNewDefaultTemplate, True)

                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            'Remove the template
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myReportTemplateDAO.Delete(dbConnection, pTemplateName)
                            End If

                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

#End Region

#Region "Read By"

        ''' <summary>
        ''' Read all records from tcfgReportTemplates.
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function ReadAll(ByVal pDbConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                        myGlobalDataTO = myReportTemplateDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Returns the Default Template from tcfgReportTemplates.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by: RH 30/11/2011</remarks>
        Public Function GetDefaultTemplate(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pTemplateOrientation As GlobalEnumerates.Orientation) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDbConnection)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                        myGlobalDataTO = myReportTemplateDAO.GetDefaultTemplate(dbConnection, pTemplateOrientation)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.GetDefaultTemplate", EventLogEntryType.Error, False)

            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace

