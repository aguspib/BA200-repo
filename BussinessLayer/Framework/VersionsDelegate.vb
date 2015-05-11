Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class VersionsDelegate

#Region "Methods"

        ''' <summary>
        ''' Get details of the versions of an specified Package
        ''' </summary>
        ''' <param name="pPackageID">Package Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VersionsDS with all data of the specified</returns>
        ''' <remarks>
        ''' Created by:  XBC 08/05/2012
        ''' </remarks>
        Public Function GetVersionsByPackage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPackageID As String) As GlobalDataTO
            Dim versionData As New VersionsDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVersionsDAO As New tfmwVersionsDAO
                        resultData = myVersionsDAO.ReadByPackage(dbConnection, pPackageID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            versionData = CType(resultData.SetDatos, VersionsDS)
                            If (versionData.tfmwVersions.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "VersionsDelegate.GetVersionsByPackage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of the versions
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet VersionsDS with all data of the specified</returns>
        ''' <remarks>
        ''' Created by:  SGM 31/05/2012
        ''' </remarks>
        Public Function GetVersionsData(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim versionData As New VersionsDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVersionsDAO As New tfmwVersionsDAO
                        resultData = myVersionsDAO.ReadAll(dbConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            versionData = CType(resultData.SetDatos, VersionsDS)
                            If (versionData.tfmwVersions.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "VersionsDelegate.GetVersionsData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values for a Firmware Version
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPackageID">identifier of the version Package</param>
        ''' <param name="pFirmwareVersion">version of firmware</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 14/09/2012
        ''' </remarks>
        Public Function SaveFirmwareVersion(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPackageID As String, ByVal pFirmwareVersion As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myVersionsDAO As New tfmwVersionsDAO
                        myGlobalDataTO = myVersionsDAO.UpdateFirmware(dbConnection, pPackageID, pFirmwareVersion)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "VersionsDelegate.SaveFirmwareVersion", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the database version (DBSoftware) value.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPackageID">Version package Id.</param>
        ''' <param name="pDBSoftware">Database Software Version.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Create by:   TR 17/01/2013
        ''' Modified by: IT 08/05/2015 - BA-2471
        ''' </remarks>
        Public Function SaveDBSoftwareVersion(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPackageID As String, ByVal pDBSoftware As String, ByVal pDBCommonRevisionNumber As String, ByVal pDBDataRevisionNumber As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myVersionsDAO As New tfmwVersionsDAO

                        myGlobalDataTO = myVersionsDAO.UpdateDBSoftware(dbConnection, pPackageID, pDBSoftware, pDBCommonRevisionNumber, pDBDataRevisionNumber) 'IT 08/05/2015 - BA-2471

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "VersionsDelegate.SaveDBSoftwareVersion", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace
