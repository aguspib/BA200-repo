﻿Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwVersionsDAO


#Region "CRUD Methods"

        ''' <summary>
        ''' Get version details of the specified Package
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPackageID">Package Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XBC 08/05/2012
        ''' </remarks>
        Public Function ReadByPackage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPackageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += " SELECT * " & vbCrLf
                        cmdText += " FROM   tfmwVersions" & vbCrLf
                        cmdText += " WHERE  PackageID = '" & pPackageID & "'" & vbCrLf

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myVersionsDS As New VersionsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myVersionsDS.tfmwVersions)

                        resultData.SetDatos = myVersionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwVersionsDAO.ReadByPackage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get version details
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: SGM 31/05/2012
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += " SELECT * " & vbCrLf
                        cmdText += " FROM   tfmwVersions"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myVersionsDS As New VersionsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myVersionsDS.tfmwVersions)

                        resultData.SetDatos = myVersionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwVersionsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update values for a Firmware Version
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPackageID">identifier of the version Package</param>
        ''' <param name="pFirmwareVersion">version of firmware</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by XBC 14/09/2012
        ''' </remarks>
        Public Function UpdateFirmware(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPackageID As String, ByVal pFirmwareVersion As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= " UPDATE tfmwVersions " & vbCrLf
                    cmdText &= " SET "

                    cmdText &= " Firmware = N'" & pFirmwareVersion.Replace("'", "''") & "'" & vbCrLf

                    cmdText &= " WHERE PackageID = '" & pPackageID & "'" & vbCrLf

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwVersionsDAO.UpdateFirmware", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the database version (DBSoftware) value 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPackageID"></param>
        ''' <param name="pDBSoftware"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 17/01/2013 v1.0.1
        ''' Modified by: IT 08/05/2015 - BA-2471
        ''' </remarks>
        Public Function UpdateDBSoftware(ByVal pDBConnection As SqlClient.SqlConnection,
                                         ByVal pPackageID As String, ByVal pDBSoftware As String, ByVal pDBCommonRevisionNumber As Integer, ByVal pDBDataRevisionNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    If (String.IsNullOrEmpty(pDBSoftware)) _
                        And ((pDBCommonRevisionNumber = 0) OrElse (Not IsDBVersionRevisable(pDBConnection))) _
                        And ((pDBDataRevisionNumber = 0) OrElse (Not IsDBVersionRevisable(pDBConnection))) Then
                        Return resultData
                    End If

                    Dim cmdText As String = ""
                    cmdText &= " UPDATE tfmwVersions " & vbCrLf
                    cmdText &= " SET "

                    If Not String.IsNullOrEmpty(pDBSoftware) Then
                        cmdText &= " DBSoftware = N'" & pDBSoftware & "',"
                    End If

                    'cmdText &= " DBRevisionDate = N'" & pDBRevisionDate & "'" & vbCrLf 'yyyy-MM-dd'

                    If (pDBCommonRevisionNumber >= 0 And IsDBVersionRevisable(pDBConnection)) Then
                        cmdText &= " DBCommonRevisionNumber = N'" & pDBCommonRevisionNumber & "',"
                    End If

                    If (pDBDataRevisionNumber >= 0 And IsDBVersionRevisable(pDBConnection)) Then
                        cmdText &= " DBDataRevisionNumber = N'" & pDBDataRevisionNumber & "'"
                    End If

                    cmdText = cmdText.TrimEnd(","c)
                    cmdText &= " WHERE PackageID = '" & pPackageID & "'" & vbCrLf

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwVersionsDAO.UpdateDBSoftware", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsDBVersionRevisable(ByVal pDBConnection As SqlClient.SqlConnection) As Boolean

            Dim script As String
            Dim ex As New Exception
            Dim result As Boolean = False

            Try

                script = " IF (SELECT COUNT(*) FROM sys.syscolumns sc INNER JOIN sys.sysobjects so ON sc.id = so.id WHERE so.xtype = 'U' AND so.name = 'tfmwVersions' AND sc.name = 'DBCommonRevisionNumber') = 1" & _
                         " AND (SELECT COUNT(*) FROM sys.syscolumns sc INNER JOIN sys.sysobjects so ON sc.id = so.id WHERE so.xtype = 'U' AND so.name = 'tfmwVersions' AND sc.name = 'DBDataRevisionNumber') = 1 " & _
                         " Select 'True' " & _
                         " ELSE " & _
                         " Select 'False' "

                result = DBManager.ExecuteBooleanScript(pDBConnection, GlobalBase.DatabaseName, script, ex)

            Catch ex
                GlobalBase.CreateLogActivity(ex)
                Return False
            End Try

            Return result

        End Function


#End Region

    End Class

End Namespace