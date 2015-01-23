Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.DAL.DAO
    Public Class vcfgLISMappingsDAO
        Inherits DAOBase

#Region "Public Methods"

        ''' <summary>
        ''' Read all data on view vcfgLISMappings.
        ''' USer can load all data filter by specific language.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional pLanguage As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format(" SELECT ValueType, ValueID, LanguageID, LongName, LISValue FROM vcfgLISMappings ")

                        If Not pLanguage = "" Then
                            cmdText &= " WHERE LanguageID = '" & pLanguage & "' OR LanguageID IS NULL "
                        End If

                        Dim myLISMappingsDS As New LISMappingsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myLISMappingsDS.vcfgLISMapping)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myLISMappingsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vcfgLISMappinsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read by the ValueType and ValueID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pValueType"></param>
        ''' <param name="pValueID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 23/04/2013</remarks>
        Public Function GetLISValue(ByVal pDBConnection As SqlClient.SqlConnection, pValueType As String, _
                                                  pValueID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT LISValue FROM vcfgLISMappings "
                        cmdText &= "WHERE ValueType = '" & pValueType & "'"
                        cmdText &= "AND ValueID = '" & pValueID & "'"

                        Dim myResult As String = String.Empty
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myResult = CStr(dbCmd.ExecuteScalar)
                        End Using

                        myGlobalDataTO.SetDatos = myResult
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vcfgLISMappinsDAO.GetLISValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region



    End Class

End Namespace
