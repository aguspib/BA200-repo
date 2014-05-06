Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwMultilanguageResourcesDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Get data of the informed MultiLanguage Resource in the specified Language
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResourceID">Resource Identifier</param>
        ''' <param name="pLanguageID">Language Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MultiLanguageDS with data of the informed Multilanguage 
        '''          Resource in the specified Language</returns>
        ''' <remarks>
        ''' Created by: PG 05/10/10
        ''' Modified by RH 15/07/2011 - Introduce the Using statement.
        '''                             Remove unnneeded "New" instruction. Introduce Short circuit evaluation.
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResourceID As String, _
                             ByVal pLanguageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = String.Format(" SELECT ResourceText FROM tfmwMultilanguageResources " & _
                                                " WHERE  ResourceID='{0}' AND LanguageID = '{1}'", pResourceID, pLanguageID)

                        Dim resourceDataDS As New MultiLanguageDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resourceDataDS.tfmwMultiLanguageResources)
                            End Using
                        End Using

                        resultData.SetDatos = resourceDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwMultilanguageResourcesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all MultiLanguage Resources in the specified Language
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLanguageID">Language Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MultiLanguageDS with data of all MultiLanguage Resources 
        '''          in the specified Language
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 02/05/2011
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLanguageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format( _
                            "SELECT ResourceID, ResourceText FROM tfmwMultilanguageResources " + _
                            "WHERE  LanguageID = '{0}'", pLanguageID)

                        Dim resourceDataDS As New MultiLanguageDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resourceDataDS.tfmwMultiLanguageResources)
                            End Using
                        End Using

                        resultData.SetDatos = resourceDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwMultilanguageResourcesDAO.Read", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

    End Class
End Namespace

