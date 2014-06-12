Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.IO 'AG 11/06/2014 #1661

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
        ''' Update the template (only DefaultTemplate field)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateName">Template Name</param>
        ''' <param name="pDefaultTemplate">True/False</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: TR 22/11/2011
        ''' AG 11/06/2014 - Make code easier: Change method name UpdateDefaultTemplateFieldByName instead of UpdateDefaultTemplateValueByTempltName
        ''' AG 11/06/2014 - #1661 always 1 template marked as Default (by orientation)</remarks>
        Public Function UpdateDefaultTemplateFieldByName(ByVal pDBConnection As SqlClient.SqlConnection, _
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
                            Dim orientation As String = "" '#1661

                            'Get template data to update
                            myGlobalDataTO = Read(dbConnection, pTemplateName)
                            If Not myGlobalDataTO.HasError Then
                                Dim myReportTemplateDS As New ReportTemplatesDS
                                myReportTemplateDS = DirectCast(myGlobalDataTO.SetDatos, ReportTemplatesDS)
                                If myReportTemplateDS.tcfgReportTemplates.Rows.Count = 1 Then
                                    orientation = myReportTemplateDS.tcfgReportTemplates(0).TemplateOrientation '#1661
                                End If
                            End If

                            If pDefaultTemplate Then
                                'If edited template is selected to be the default ... reset the current default!!
                                If orientation <> "" Then
                                    myGlobalDataTO = SetFalseDefaultTemplateByTempOrientation(dbConnection, orientation)
                                End If
                            End If

                            'Update the template (only defaultTemplate field)
                            myGlobalDataTO = myReportTemplateDAO.UpdateDefaultTemplateFieldByName(dbConnection, pTemplateName, pDefaultTemplate)

                            'AG 11/06/2014 - #1661 always 1 template marked as Default (by orientation)
                            If Not pDefaultTemplate Then
                                Dim requiredSetDefaultTemplateFlag As Boolean = False
                                'If orientation = "" mark as default the BOTH mastertemplates, else mark only the mastertemplate of current orientation
                                If orientation = "" Then
                                    myGlobalDataTO = Me.SetDefaultTemplateStatus(dbConnection, False, False)
                                    requiredSetDefaultTemplateFlag = True
                                Else
                                    'Read if the current orientation have a default template
                                    myGlobalDataTO = ReadAll(dbConnection)
                                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                        Dim reportsInDB As New ReportTemplatesDS
                                        reportsInDB = CType(myGlobalDataTO.SetDatos, ReportTemplatesDS)

                                        'Search if there is a template programmed as default for the current orientation
                                        Dim linqResults As List(Of ReportTemplatesDS.tcfgReportTemplatesRow)
                                        linqResults = (From a As ReportTemplatesDS.tcfgReportTemplatesRow In reportsInDB.tcfgReportTemplates _
                                                   Where a.DefaultTemplate = True And a.TemplateOrientation = orientation Select a).ToList

                                        If linqResults.Count = 0 Then
                                            requiredSetDefaultTemplateFlag = True
                                        End If
                                        linqResults = Nothing

                                    Else
                                        'If error mark as default the BOTH mastertemplates, else mark only the mastertemplate of current orientation
                                        myGlobalDataTO = Me.SetDefaultTemplateStatus(dbConnection, False, False)
                                        requiredSetDefaultTemplateFlag = True
                                    End If

                                End If

                                'If not default template programmed set the preloaed mastertemplates by orientation
                                If requiredSetDefaultTemplateFlag Then
                                    myGlobalDataTO = SetDefaultTemplateStatus(dbConnection, True, True, orientation)
                                End If
                            End If
                            'AG 11/06/2014 #1661

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
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateDefaultTemplateFieldByName", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the template and also rename it and his designer files (also updates the DefaultTemplate field)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewTemplate">Template Name</param>
        ''' <param name="pOldTemplate">Template Name</param>
        ''' <param name="pOrientation"></param>
        ''' <param name="pDefaultTemplate"></param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011
        ''' AG 11/06/2014 - Make code easier: Change method name UpdateRenamingTemplate instead of UpdateTemplateNameByOldName
        ''' AG 12/06/2014 #1661 - also update DefaultTemplate and always 1 template marked as Default (by orientation)</remarks>
        Public Function UpdateRenamingTemplate(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                    ByVal pNewTemplate As String, _
                                                    ByVal pOldTemplate As String, ByVal pOrientation As String, ByVal pDefaultTemplate As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                        'AG 12/06/2014 #1661 if the report to update is marked as default reset current default for the new report orientation
                        If pDefaultTemplate Then
                            myGlobalDataTO = SetFalseDefaultTemplateByTempOrientation(dbConnection, pOrientation)
                        End If
                        'AG 12/06/2014 #1661

                        myGlobalDataTO = myReportTemplateDAO.UpdateTemplateName(dbConnection, pNewTemplate, pOldTemplate, pDefaultTemplate)

                        'AG 11/06/2014 - #1661 always 1 template marked as Default (by orientation)
                        If Not pDefaultTemplate Then
                            Dim requiredSetDefaultTemplateFlag As Boolean = False
                            'Read if the current orientation have a default template
                            myGlobalDataTO = ReadAll(dbConnection)
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                Dim reportsInDB As New ReportTemplatesDS
                                reportsInDB = CType(myGlobalDataTO.SetDatos, ReportTemplatesDS)

                                'Search if there is a template programmed as default for the current orientation
                                Dim linqResults As List(Of ReportTemplatesDS.tcfgReportTemplatesRow)
                                linqResults = (From a As ReportTemplatesDS.tcfgReportTemplatesRow In reportsInDB.tcfgReportTemplates _
                                           Where a.DefaultTemplate = True And a.TemplateOrientation = pOrientation Select a).ToList

                                If linqResults.Count = 0 Then
                                    requiredSetDefaultTemplateFlag = True
                                End If
                                linqResults = Nothing

                            Else
                                'If error mark as default the BOTH mastertemplates, else mark only the mastertemplate of current orientation
                                myGlobalDataTO = Me.SetDefaultTemplateStatus(dbConnection, False, False)
                                requiredSetDefaultTemplateFlag = True
                            End If

                            'If not default template programmed set the preloaed mastertemplates by orientation
                            If requiredSetDefaultTemplateFlag Then
                                myGlobalDataTO = SetDefaultTemplateStatus(dbConnection, True, True, pOrientation)
                            End If
                        End If
                        'AG 11/06/2014 #1661


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
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateRenamingTemplate", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the all fields in database (used when edit changes orientation)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateRow">Template Name</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011
        ''' AG 11/06/2014 - Make code easier: Change method name UpdateComplete instead of UpdateDefaultTemplateByTempltName
        ''' AG 12/06/2014 - #1661 always 1 template marked as Default (by orientation)</remarks>
        Public Function UpdateComplete(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                          ByVal pTemplateRow As ReportTemplatesDS.tcfgReportTemplatesRow) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO

                        'AG 12/06/2014 #1661 if the report to update is marked as default reset current default for the new report orientation
                        If pTemplateRow.DefaultTemplate Then
                            myGlobalDataTO = SetFalseDefaultTemplateByTempOrientation(dbConnection, pTemplateRow.TemplateOrientation)
                        End If
                        'AG 12/06/2014 #1661

                        'Update the template (full update)
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myReportTemplateDAO.UpdateComplete(dbConnection, pTemplateRow)
                        End If

                        'AG 12/06/2014 - #1661 current orientation OK but check that there is 1 template marked as Default in the old orientation
                        If Not myGlobalDataTO.HasError Then
                            'Read if the other orientation have a default template
                            Dim requiredSetDefaultTemplateFlag As Boolean = False
                            Dim secondOrientationName As String = ""

                            myGlobalDataTO = ReadAll(dbConnection)
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                Dim reportsInDB As New ReportTemplatesDS
                                reportsInDB = CType(myGlobalDataTO.SetDatos, ReportTemplatesDS)

                                'Search if there is a template programmed as default for the current orientation
                                Dim linqResults As List(Of ReportTemplatesDS.tcfgReportTemplatesRow)
                                linqResults = (From a As ReportTemplatesDS.tcfgReportTemplatesRow In reportsInDB.tcfgReportTemplates _
                                           Where a.DefaultTemplate = True And a.TemplateOrientation <> pTemplateRow.TemplateOrientation Select a).ToList

                                If linqResults.Count = 0 Then
                                    requiredSetDefaultTemplateFlag = True

                                    'Get the other orientation name
                                    linqResults = (From a As ReportTemplatesDS.tcfgReportTemplatesRow In reportsInDB.tcfgReportTemplates _
                                                   Where a.TemplateOrientation <> pTemplateRow.TemplateOrientation Select a).ToList

                                    If linqResults.Count > 0 Then
                                        secondOrientationName = linqResults(0).TemplateOrientation
                                    End If
                                End If
                            Else
                                'If error mark as default the BOTH mastertemplates, else mark only the mastertemplate of current orientation
                                myGlobalDataTO = Me.SetDefaultTemplateStatus(dbConnection, False, False)
                                requiredSetDefaultTemplateFlag = True
                            End If

                            'If not default template programmed set the preloaed mastertemplates by orientation
                            If requiredSetDefaultTemplateFlag Then
                                myGlobalDataTO = SetDefaultTemplateStatus(dbConnection, True, True, secondOrientationName)
                            End If

                        End If
                        'AG 12/06/2014 #1661



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
                myLogAcciones.CreateLogActivity(ex.Message, "ReportTemplatesDelegate.UpdateComplete", EventLogEntryType.Error, False)

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
                                                myGlobalDataTO = UpdateDefaultTemplateFieldByName(dbConnection, myNewDefaultTemplate, True)

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

#Region "Others"

        ''' <summary>
        ''' Read all report templates defined as MasterTemplate = 0 and check if the designers exits or not on computer
        ''' Exits -> Leave them
        ''' No -> Remove from database
        ''' 
        ''' This method is called after load RSATs or restore points
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>AG 11/06/2014 Create - #1661</remarks>
        Public Function DeleteNonExistingReportTemplates(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tcfgReportTemplatesDAO

                        'Get all report templates defined in database just loaded
                        resultData = myDAO.ReadAll(dbConnection)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim reportsInDB As New ReportTemplatesDS
                            reportsInDB = CType(resultData.SetDatos, ReportTemplatesDS)

                            'Filter only the user defined templates (those with MasterTemplate as FALSE)
                            Dim linqResults As List(Of ReportTemplatesDS.tcfgReportTemplatesRow)
                            linqResults = (From a As ReportTemplatesDS.tcfgReportTemplatesRow In reportsInDB.tcfgReportTemplates _
                                       Where a.MasterTemplate = False Select a).ToList


                            Dim restoreDefaultTemplateValue As Boolean = True 'Set as default the preloaded ones

                            If linqResults.Count > 0 Then
                                Dim PathTemplates As String = GlobalBase.AppPath & GlobalBase.ReportPath
                                Dim templateName As String = String.Empty
                                restoreDefaultTemplateValue = False
                                For Each row As ReportTemplatesDS.tcfgReportTemplatesRow In linqResults
                                    templateName = row.TemplateName

                                    'If some designer's files are missing in local computer then remove record from database
                                    If Not File.Exists(PathTemplates & "\" & templateName & ".REPX") OrElse Not File.Exists(PathTemplates & "\" & templateName & ".GIF") Then
                                        'If a report saved as DefaultTemplate is deleted ... set as default the preloaded ones
                                        If row.DefaultTemplate AndAlso Not restoreDefaultTemplateValue Then restoreDefaultTemplateValue = True

                                        'Do not use variable dbConnection, use an unitary transaction!!!
                                        resultData = Me.Delete(Nothing, templateName)
                                    End If
                                Next
                            End If
                            linqResults = Nothing

                            'Set the MasterTemplates as default
                            If restoreDefaultTemplateValue Then
                                resultData = Me.SetDefaultTemplateStatus(Nothing, False, False) 'Reset field DefaultTemplate for all reports
                                resultData = Me.SetDefaultTemplateStatus(Nothing, True, True) 'Set to TRUE field DefaultTemplate only for MasterTemplate reports
                            End If

                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReportTemplatesDelegate.DeleteNonExistingReportTemplates", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Set DefaultTemplate value as False by the Template Orientation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pStatus">TRUE or FALSE</param>
        ''' <param name="pOnlyMasterTemplateFlag">TRUE means that DefaultTemplate will be updated only for the MasterTemplates / FALSE means that will be updated for ALL</param>
        ''' <param name="pTemplateOrientation">PORTRAIT or LANDSCAPE</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>AG 11/06/2014 - Create - #1661</remarks>
        Public Function SetDefaultTemplateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As Boolean, ByVal pOnlyMasterTemplateFlag As Boolean, _
                                                  Optional ByVal pTemplateOrientation As String = "") As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myReportTemplateDAO As New tcfgReportTemplatesDAO
                        myGlobalDataTO = myReportTemplateDAO.SetDefaultTemplateStatus(dbConnection, pStatus, pOnlyMasterTemplateFlag, pTemplateOrientation)
                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception

                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReportTemplatesDelegate.SetDefaultTemplateStatus", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace

