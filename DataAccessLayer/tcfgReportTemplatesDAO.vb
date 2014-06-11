Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tcfgReportTemplatesDAO
        Inherits DAOBase


        ''' <summary>
        ''' Create one or serverar records on tcfgReportTemplates table.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReportTemplatesDS"></param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 21/11/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pReportTemplatesDS As ReportTemplatesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdStringBuilder As New Text.StringBuilder
                    For Each ReportTemplateRow As ReportTemplatesDS.tcfgReportTemplatesRow In pReportTemplatesDS.tcfgReportTemplates.Rows
                        cmdStringBuilder.Append(" INSERT INTO tcfgReportTemplates (" & Environment.NewLine)
                        cmdStringBuilder.Append(" TemplateName, MasterTemplate, TemplateOrientation, TemplateFileName, DefaultTemplate, TS_User, TS_DateTime )" & Environment.NewLine)
                        cmdStringBuilder.Append(" VALUES (" & Environment.NewLine)
                        cmdStringBuilder.Append(" N'" & ReportTemplateRow.TemplateName.Replace("'", "''") & "'" & Environment.NewLine)
                        cmdStringBuilder.Append(", '" & ReportTemplateRow.MasterTemplate & "'" & Environment.NewLine)
                        cmdStringBuilder.Append(", '" & ReportTemplateRow.TemplateOrientation & "'" & Environment.NewLine)
                        cmdStringBuilder.Append(", N'" & ReportTemplateRow.TemplateFileName.Replace("'", "''") & "'" & Environment.NewLine)
                        cmdStringBuilder.Append(", '" & ReportTemplateRow.DefaultTemplate & "'" & Environment.NewLine)

                        If ReportTemplateRow.IsTS_UserNull Then
                            cmdStringBuilder.Append(", N'" & myGlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "' ")
                        Else
                            cmdStringBuilder.Append(", '" & ReportTemplateRow.TS_User & "'" & Environment.NewLine)
                        End If

                        If ReportTemplateRow.IsTS_DateTimeNull Then
                            cmdStringBuilder.Append(", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & Environment.NewLine)
                        Else
                            cmdStringBuilder.Append(", '" & ReportTemplateRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' " & Environment.NewLine)
                        End If

                        cmdStringBuilder.Append(")" & Environment.NewLine)
                    Next

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdStringBuilder.ToString

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the tcfgReportTemplates by the TemplateName.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateName">Template Name.</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTemplateName As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT TemplateName, MasterTemplate, TemplateOrientation, TemplateFileName, DefaultTemplate " & Environment.NewLine
                        cmdText &= " FROM tcfgReportTemplates" & Environment.NewLine
                        cmdText &= " WHERE TemplateName = N'" & pTemplateName.Replace("'", "''") & "'" & Environment.NewLine

                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Dim myReportTemplatesDS As New ReportTemplatesDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReportTemplatesDS.tcfgReportTemplates)
                                myGlobalDataTO.SetDatos = myReportTemplatesDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the Defaul Template 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReportTemplateRow">Template name</param>
        ''' <returns>GlobalaDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011
        ''' AG 11/06/2014 - Make code easier: Change method name UpdateComplete instead of UpdateDefaultTemplateByTempltName</remarks>
        Public Function UpdateComplete(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                          ByVal pReportTemplateRow As ReportTemplatesDS.tcfgReportTemplatesRow) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty

                    cmdText &= "UPDATE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= "   SET TemplateName = N'" & pReportTemplateRow.TemplateName.Replace("'", "''") & "'" & Environment.NewLine
                    cmdText &= "     , MasterTemplate = '" & pReportTemplateRow.MasterTemplate & "'" & Environment.NewLine
                    cmdText &= "     , TemplateOrientation = '" & pReportTemplateRow.TemplateOrientation & "'" & Environment.NewLine
                    cmdText &= "     , TemplateFileName =  N'" & pReportTemplateRow.TemplateFileName.Replace("'", "''") & "'" & Environment.NewLine
                    cmdText &= "     , DefaultTemplate = '" & pReportTemplateRow.MasterTemplate & "'" & Environment.NewLine
                    cmdText &= "     , TS_User = '" & myGlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "' " & Environment.NewLine
                    cmdText &= "     , TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & Environment.NewLine
                    cmdText &= " WHERE TemplateName = '" & pReportTemplateRow.TemplateName & "' " & Environment.NewLine


                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.UpdateComplete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the Defaul Template 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewName">Template name</param>
        ''' <param name="pOldName">Template name</param>
        ''' <returns>GlobalaDataTO</returns>
        ''' <remarks>CREATED BY: DL 25/11/2011</remarks>
        Public Function UpdateTemplateName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pNewName As String, _
                                           ByVal pOldName As String) As GlobalDataTO


            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty

                    cmdText &= "UPDATE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= "   SET TemplateName = N'" & pNewName.Replace("'", "''") & "'" & Environment.NewLine
                    cmdText &= "     , TemplateFileName = N'" & pNewName.Replace("'", "''") & ".REPX'" & Environment.NewLine
                    cmdText &= " WHERE TemplateName = N'" & pOldName.Replace("'", "''") & "'"

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.UpdateTemplateName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Defaul Template value by the Template Name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateName">Template name</param>
        ''' <param name="pDefaultTemplate">True/False</param>
        ''' <returns>GlobalaDataTO</returns>
        ''' <remarks>CREATED BY: TR 23/11/2011
        ''' AG 11/06/2014 - Make code easier: Change method name UpdateDefaultTemplateByName instead of UpdateDefaultTemplateValueByTempltName</remarks>
        Public Function UpdateDefaultTemplateByName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                       ByVal pTemplateName As String, ByVal pDefaultTemplate As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty

                    cmdText &= " UPDATE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= " SET DefaultTemplate = '" & pDefaultTemplate & "', " & Environment.NewLine
                    cmdText &= " TS_User = '" & myGlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "', " & Environment.NewLine
                    cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & Environment.NewLine
                    cmdText &= " WHERE TemplateName = N'" & pTemplateName.Replace("'", "''") & "'" & Environment.NewLine

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.UpdateDefaultTemplateByName", _
                                                EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Set DefaultTemplate value as False by the Template Orientation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateOrientation">Template Orientation to update</param>
        ''' <returns>GlobalaDataTO</returns>
        ''' <remarks>CREATED BY: TR 22/11/2011</remarks>
        Public Function SetFalseDefaultTemplateByTempOrientation(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                              ByVal pTemplateOrientation As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty

                    cmdText &= " UPDATE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= " SET DefaultTemplate = 'False', " & Environment.NewLine
                    cmdText &= " TS_User = '" & myGlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "', " & Environment.NewLine
                    cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & Environment.NewLine
                    cmdText &= " WHERE TemplateOrientation = '" & pTemplateOrientation & "' " & Environment.NewLine
                    cmdText &= " AND DefaultTemplate = 'True' " & Environment.NewLine

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.SetFlaseDefaultValueByTempOrientation", _
                                                EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a record by the Template Name.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateName"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pTemplateName As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText &= " DELETE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= " WHERE TemplateName = N'" & pTemplateName & "'" & Environment.NewLine
                    cmdText &= " AND  MasterTemplate = 'False'" & Environment.NewLine


                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all records from tcfgReportTemplates.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/11/2011</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT TemplateName, MasterTemplate, TemplateOrientation, TemplateFileName, DefaultTemplate, TS_User, TS_DateTime  " & Environment.NewLine
                        cmdText &= "  FROM tcfgReportTemplates"

                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Dim myReportTemplatesDS As New ReportTemplatesDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReportTemplatesDS.tcfgReportTemplates)
                                myGlobalDataTO.SetDatos = myReportTemplatesDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read table by the Template Orientation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTemplateOrientation">Template Orientation</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: 22/11/2011</remarks>
        Public Function ReadByTemplateOrientation(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                  ByVal pTemplateOrientation As String, _
                                                  Optional ByVal pMasterTemplate As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT * " & Environment.NewLine
                        cmdText &= " FROM tcfgReportTemplates " & Environment.NewLine
                        cmdText &= " WHERE TemplateOrientation = '" & pTemplateOrientation & "'" & Environment.NewLine

                        'Validate if only wnat the master template
                        If pMasterTemplate Then
                            cmdText &= " AND MasterTemplate = '" & pMasterTemplate & "'" & Environment.NewLine
                        End If

                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Dim myReportTemplatesDS As New ReportTemplatesDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReportTemplatesDS.tcfgReportTemplates)
                                myGlobalDataTO.SetDatos = myReportTemplatesDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.ReadByTemplateOrientation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Returns the Default Master Template from tcfgReportTemplates.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by: RH 30/11/2011</remarks>
        Public Function GetDefaultTemplate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTemplateOrientation As GlobalEnumerates.Orientation) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format( _
                                " SELECT * FROM tcfgReportTemplates" & _
                                " WHERE DefaultTemplate = 1 AND TemplateOrientation = '{0}'", pTemplateOrientation)

                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            'Fill the DataSet to return 
                            Dim myReportTemplatesDS As New ReportTemplatesDS

                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReportTemplatesDS.tcfgReportTemplates)
                                myGlobalDataTO.SetDatos = myReportTemplatesDS
                            End Using
                        End Using
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.GetDefaultMasterTemplate", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Set DefaultTemplate value as False by the Template Orientation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pStatus">TRUE or FALSE</param>
        ''' <param name="pOnlyMasterTemplateFlag">TRUE means that DefaultTemplate will be updated only for the MasterTemplates / FALSE means that will be updated for ALL</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>AG 11/06/2014 - Create - #1661</remarks>
        Public Function SetDefaultTemplateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As Boolean, ByVal pOnlyMasterTemplateFlag As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty

                    cmdText &= " UPDATE tcfgReportTemplates " & Environment.NewLine
                    cmdText &= " SET DefaultTemplate =  " & CStr(IIf(pStatus, 1, 0)) & Environment.NewLine

                    If pOnlyMasterTemplateFlag Then
                        cmdText &= " WHERE MasterTemplate = " & CStr(IIf(pOnlyMasterTemplateFlag, 1, 0))
                    End If

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.SetDefaultTemplateStatus", _
                                                EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

    End Class

End Namespace

