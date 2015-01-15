Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class ISEDelegate

#Region "Constructor"
        Public Sub New()

        End Sub
#End Region

#Region "Database Management"

        ''' <summary>
        ''' Creates a new ISE info master data with the informed dataset 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function CreateNewMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.CreateNewMasterData(dbConnection, pISEInfoDS)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.CreateNewMasterData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets all ISE Information (settings) by Analyzer Identifier
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function ReadAllInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.ReadAll(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.ReadAllInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets all ISE Information (settings)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XB 10/05/2013</remarks>
        Public Function ReadAllInfo(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.ReadAllInfo (2)", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets a specific ISE Information setting by Analyzer Identifier
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pISESettingID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function ReadInfoItemValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pISESettingID As GlobalEnumerates.ISEModuleSettings) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.ReadISESetting(dbConnection, pAnalyzerID, pISESettingID.ToString)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.ReadInfoItemValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates ISE information related to the informed Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pISEInfoDS"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function UpdateISEInfo(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pAnalyzerID As String, _
                                          ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.Update(dbConnection, pAnalyzerID, pISEInfoDS)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.UpdateISEInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Update specified value
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer identifier</param>
        ''' <param name="pSettingID">Setting identifier</param>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by: XB 10/05/2013</remarks>
        Public Function UpdateISEInfo(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pAnalyzerID As String, _
                                      ByVal pSettingID As String, _
                                      ByVal pValue As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISESettingsDAO As New tinfoISEDAO
                        resultData = myISESettingsDAO.Update(dbConnection, pAnalyzerID, pSettingID, pValue)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEInfoDelegate.UpdateISEInfo (2)", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

#End Region


    End Class

End Namespace
