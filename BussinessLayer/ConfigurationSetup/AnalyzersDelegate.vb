Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class AnalyzersDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get all distinct Analyzers defined in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSAnalyzerAlarmsDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  IR 04/10/2012
        ''' </remarks>
        Public Function GetDistinctAnalyzers(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mythisWSAnalyzerAlarmsDAO As New thisWSAnalyzerAlarmsDAO
                        returnedData = mythisWSAnalyzerAlarmsDAO.ReadAllDistinctAnalyzers(dbConnection)
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetDistinctAnalyzers", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function



        ''' <summary>
        ''' Get all the Analyzers defined in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  JB 28/09/2012
        ''' </remarks>
        Public Function GetAllAnalyzers(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO
                        returnedData = mytcfgAnalyzersDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAllAnalyzers", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Get the active Analyzers defined in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: SA 11/01/2010 - Changed the way of opening the DB Conection to fulfill the new template.
        '''                              Added code to write in the application log in case of error
        ''' Modified by: XBC 06/06/2012 - Add functionality to get the information corresponding with the Analyzer connected
        ''' </remarks>
        Public Function GetAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        ' XBC 06/06/2012
                        'returnedData = mytcfgAnalyzersDAO.ReadAll(dbConnection)
                        returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerActive(dbConnection)

                        ' PDT TODO !!!! GetAnalyzer would be take from Analyzer Manager (Analyzer Layer) instead from Database everytime !
                        ' XBC 06/06/2012

                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        '''' <summary>
        '''' Get an specified Analyzer previously stored in the DB
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Anayzer identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        '''' <remarks>
        '''' Created by XBC : 30/09/2011 - Add AnalyzerID received from the Instrument
        '''' Deleted by XBC : 06/06/2012 - Not used
        '''' </remarks>
        'Public Function GetAnalyzerByID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

        '    Dim returnedData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO
        '                returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerID(dbConnection, pAnalyzerID)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        returnedData.HasError = True
        '        returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        returnedData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAnalyzerByID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return returnedData
        'End Function

        ''' <summary>
        ''' Get all values of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the Analyzer information</returns>
        ''' <remarks>
        ''' Created by:  DL 26/02/2010
        ''' </remarks>
        Public Function GetAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerID(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAnalyzerModel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Check Analyzer connected with the existing into DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  XBC 06/06/2012 - Add functionality to get the information corresponding with the Analyzer connected
        ''' Modified by: TR: 05/11/2013 -BT #1374 -Make sure to select the active analyzer, if there are several analyzers.
        ''' </remarks>
        Public Function CheckAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      Optional ByVal pAnalyzer As AnalyzersDS.tcfgAnalyzersRow = Nothing) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myLogAccionesAux As New ApplicationLogManager()
            Try
                returnedData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        If pAnalyzer Is Nothing Then
                            ' CASE 1 : CHECK INFO ANALYZERS FROM DB WITHOUT COMMUNICATIONS

                            ' First check if exists any Analyzer previously connected
                            returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerGeneric(dbConnection, False)
                            If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                                Dim myAnalyzerData As New AnalyzersDS
                                myAnalyzerData = DirectCast(returnedData.SetDatos, AnalyzersDS)
                                If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                                    ' There are Last connected AnalyzerID and use it
                                    'TR 05/11/2013 -BT 1374
                                    For Each analyzerInfo As AnalyzersDS.tcfgAnalyzersRow In myAnalyzerData.tcfgAnalyzers.Rows
                                        If Not analyzerInfo.Active Then
                                            analyzerInfo.Delete() ' if not the active analyzer then remove form dataset.
                                        End If
                                    Next
                                    'TR 05/11/2013 -BT 1374 -END
                                    myAnalyzerData.AcceptChanges()
                                Else
                                    ' Get Generic AnalyzerID
                                    returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerGeneric(dbConnection, True)
                                End If
                            End If
                            ' CASE 1
                        Else
                            ' CASE 2 : INFO ANALYZER RECEIVED FROM THE CONNECTION WITH THE INSTRUMENT 
                            Dim myGlobal As New GlobalDataTO
                            returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerID(dbConnection, pAnalyzer.AnalyzerID)
                            If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                                Dim myAnalyzerData As New AnalyzersDS
                                myAnalyzerData = DirectCast(returnedData.SetDatos, AnalyzersDS)
                                If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                                    ' Current Analyzer connected is already inserted in Database !
                                    ' Not isNew !!!

                                    GlobalBase.CreateLogActivity("(Analyzer Change) Update Analyzer Active [" & pAnalyzer.AnalyzerID & " ]", "AnalyzersDelegate.CheckAnalyzer", EventLogEntryType.Information, False)
                                    myGlobal = mytcfgAnalyzersDAO.UpdateAnalyzerActive(dbConnection, pAnalyzer.AnalyzerID)
                                    If myGlobal.HasError Then
                                        GlobalBase.CreateLogActivity("Update Analyzer Active Error !", "AnalyzersDelegate.CheckAnalyzer", EventLogEntryType.Information, False)
                                        returnedData.HasError = True
                                    End If

                                Else
                                    ' isNew !!!

                                    Dim myAnalyzersDS As New AnalyzersDS
                                    Dim myAnalyzersRow As AnalyzersDS.tcfgAnalyzersRow
                                    myAnalyzersRow = myAnalyzersDS.tcfgAnalyzers.NewtcfgAnalyzersRow

                                    GlobalBase.CreateLogActivity("(Analyzer Change) Insert New Analyzer Active [" & pAnalyzer.AnalyzerID & " ]", "AnalyzersDelegate.CheckAnalyzer", EventLogEntryType.Information, False)
                                    myAnalyzersRow.AnalyzerID = pAnalyzer.AnalyzerID
                                    myAnalyzersRow.AnalyzerModel = pAnalyzer.AnalyzerModel
                                    myAnalyzersRow.FirmwareVersion = pAnalyzer.FirmwareVersion
                                    myAnalyzersRow.Generic = False
                                    myAnalyzersRow.Active = True
                                    myAnalyzersRow.IsNew = True
                                    myAnalyzersDS.tcfgAnalyzers.Rows.Add(myAnalyzersRow)

                                    returnedData.SetDatos = myAnalyzersDS
                                    returnedData.HasError = False

                                End If

                            End If
                            ' CASE 2
                        End If


                        If Not returnedData.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.CheckAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Insert the new Analyzer connected
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  XBC 13/06/2012
        ''' </remarks>
        Public Function InsertAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, _
                                       Optional ByVal pAnalyzer As AnalyzersDS.tcfgAnalyzersRow = Nothing) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myLogAccionesAux As New ApplicationLogManager()
            Try
                returnedData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim myAnalyzersDS As New AnalyzersDS
                        Dim myAnalyzerData As New AnalyzersDS
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerID(dbConnection, pAnalyzer.AnalyzerID)
                        If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                            myAnalyzerData = DirectCast(returnedData.SetDatos, AnalyzersDS)

                            If (myAnalyzerData.tcfgAnalyzers.Rows.Count = 0) Then
                                'Create new connected Analyzer
                                GlobalBase.CreateLogActivity("(Analyzer Change) Create New Analyzer Active [" & pAnalyzer.AnalyzerID & " ]", "AnalyzersDelegate.CheckAnalyzer", EventLogEntryType.Information, False)
                                returnedData = mytcfgAnalyzersDAO.Create(dbConnection, pAnalyzer)
                                If (Not returnedData.HasError) Then
                                    ' If there are another Analyzers previously connected set Active field as False
                                    ' Just the current Analyzer connected has Active fiels as True

                                    GlobalBase.CreateLogActivity("(Analyzer Change) Update Analyzer Active [" & pAnalyzer.AnalyzerID & " ]", "AnalyzersDelegate.InsertAnalyzer", EventLogEntryType.Information, False)
                                    returnedData = mytcfgAnalyzersDAO.UpdateAnalyzerActive(dbConnection, pAnalyzer.AnalyzerID)
                                    If Not returnedData.HasError Then
                                        Dim myAnalyzersRow As AnalyzersDS.tcfgAnalyzersRow
                                        myAnalyzersRow = myAnalyzerData.tcfgAnalyzers.NewtcfgAnalyzersRow

                                        myAnalyzersRow.AnalyzerID = pAnalyzer.AnalyzerID
                                        myAnalyzersRow.AnalyzerModel = pAnalyzer.AnalyzerModel
                                        myAnalyzersRow.FirmwareVersion = pAnalyzer.FirmwareVersion
                                        myAnalyzersRow.Generic = False
                                        myAnalyzersRow.Active = True
                                        myAnalyzerData.tcfgAnalyzers.Rows.Add(myAnalyzersRow)
                                    End If
                                End If

                            End If
                        End If

                        If Not returnedData.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            returnedData.SetDatos = myAnalyzerData
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.InsertAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Get the Analyzer defined as Generic in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  XBC 11/06/2012
        ''' </remarks>
        Public Function GetAnalyzerGeneric(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        returnedData = mytcfgAnalyzersDAO.ReadByAnalyzerGeneric(dbConnection, True)

                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAnalyzerGeneric", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' There Can Be Only One Active Connected Analyzer and the rest must be eliminated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' </remarks>
        Public Function DeleteConnectedAnalyzersNotActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                returnedData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzersDAO As New tcfgAnalyzersDAO

                        returnedData = mytcfgAnalyzersDAO.DeleteConnectedAnalyzersNotActive(dbConnection)

                        If Not returnedData.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.DeleteConnectedAnalyzersNotActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function
#End Region

#Region "TO REVIEW-DELETE"
        '''' <summary>
        '''' Get current values of the Communication Settings defined for the specified Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CommunicationConfigDS with the Communication Settings 
        ''''          currently defined for the specified Analyzer</returns>
        '''' <remarks>
        '''' Created by:  VR 29/04/2010 - Tested: OK
        '''' </remarks>
        'Public Function GetAnalyzerSettings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim returnedData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
        '                returnedData = myAnalyzerSettingsDAO.ReadAnalyzerSettings(dbConnection, pAnalyzerID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        returnedData.HasError = True
        '        returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        returnedData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAnalyzerDetails", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return returnedData
        'End Function

        '''' <summary>
        '''' Create or Update values of Communication Settings for an specific Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pComm">Typed DataSet CommunicationConfigDS containing the data to create or update</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by:  VR 28/04/2010 - Tested OK
        '''' Modified by: VR 29/04/2010 - Tested OK
        ''''              TR 03/05/2010 - Implement the Delegate Pattern
        '''' </remarks>
        'Public Function SetAnalyzerSettings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pComm As CommunicationConfigDS) As GlobalDataTO
        '    Dim returnedData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection()

        '    Try
        '        returnedData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Validate if there is any record on the table for the specified Analyzer
        '                Dim communicationSettings As New tcfgAnalyzerSettingsDAO
        '                returnedData = communicationSettings.ReadAnalyzerSettings(dbConnection, pComm.tcfgAnalyzerSettings(0).AnalyzerID)
        '                If (Not returnedData.HasError) Then
        '                    Dim commConfigDS As New CommunicationConfigDS
        '                    commConfigDS = CType(returnedData.SetDatos, CommunicationConfigDS)

        '                    If (commConfigDS.tcfgAnalyzerSettings.Rows.Count = 0) Then
        '                        'Add the Communication Settings
        '                        returnedData = communicationSettings.Create(dbConnection, pComm)
        '                    Else
        '                        'Updates values of the Communication Settings
        '                        returnedData = communicationSettings.Update(dbConnection, pComm)
        '                    End If
        '                End If

        '                If (Not returnedData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'Addition was not possible
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        returnedData.HasError = True
        '        returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        returnedData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.AnalyzerSettings", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return returnedData
        'End Function

        '''' <summary>
        '''' Save values of Analyzer Communication Settings and Session Settings
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerSettings">Typed DataSet CommunicationConfigDS containing values of the Analyzer Communication Settings</param>
        '''' <param name="pSessionSettings">Typed DataSet UserSettings containing each Session Setting with its assigned value</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 05/07/2010
        '''' </remarks>
        'Public Function SaveUserSettings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerSettings As CommunicationConfigDS, _
        '                                 ByVal pSessionSettings As UserSettingDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection()
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Save the Analyzer Communication Settings
        '                Dim commConfigToSave As New AnalyzersDelegate
        '                resultData = commConfigToSave.SetAnalyzerSettings(dbConnection, pAnalyzerSettings)

        '                If (Not resultData.HasError) Then
        '                    'Save the Session Settings
        '                    Dim myUserSettingsDelegate As New UserSettingsDelegate
        '                    For Each userSetting As UserSettingDS.tcfgUserSettingsRow In pSessionSettings.tcfgUserSettings.Rows
        '                        resultData = myUserSettingsDelegate.Update(dbConnection, userSetting.SettingID, userSetting.CurrentValue)
        '                        If (resultData.HasError) Then Exit For
        '                    Next
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.SaveUserSettings", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function


        'returnedData = mytcfgAnalyzersDAO.ReadAll(dbConnection)
        '                    If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
        'Dim myAnalyzerData As New AnalyzersDS
        '                        myAnalyzerData = DirectCast(returnedData.SetDatos, AnalyzersDS)

        ''Get the Last connected Analyzers registered into DB
        'Dim listAnalyzers As New List(Of AnalyzersDS.tcfgAnalyzersRow)

        '                        listAnalyzers = (From a In myAnalyzerData.tcfgAnalyzers _
        '                                         Where a.Generic = False _
        '                                         Select a).ToList()

        '                        pAnalyzer = myAnalyzerData.tcfgAnalyzers.NewtcfgAnalyzersRow
        '                        If listAnalyzers.Count > 0 Then
        '' Return data of NOT GENERIC Analyzer - means the last connected Analyzer
        '                            pAnalyzer.AnalyzerID = listAnalyzers.First().AnalyzerID
        '                            pAnalyzer.AnalyzerModel = listAnalyzers.First().AnalyzerModel
        '                            pAnalyzer.FirmwareVersion = listAnalyzers.First().FirmwareVersion
        '                        Else
        '' Return data of the GENERIC Analyzer

        '                            listAnalyzers = (From a In myAnalyzerData.tcfgAnalyzers _
        '                                             Where a.Generic = True _
        '                                             Select a).ToList()

        '                            If listAnalyzers.Count > 0 Then
        '                                pAnalyzer.AnalyzerID = listAnalyzers.First().AnalyzerID
        '                                pAnalyzer.AnalyzerModel = listAnalyzers.First().AnalyzerModel
        '                                pAnalyzer.FirmwareVersion = listAnalyzers.First().FirmwareVersion
        '                            End If
        '                        End If

        '                    End If
#End Region
    End Class
End Namespace