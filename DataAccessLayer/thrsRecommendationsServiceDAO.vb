Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class thrsRecommendationsServiceDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create a new Recommendation Service
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pRecommendationsList"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 03/08/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRecommendationsList As SRVRecommendationsServiceDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pRecommendationsList Is Nothing) Then
                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True

                    For Each rowthrsRecommendationsService As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In pRecommendationsList.srv_thrsRecommendationsService.Rows
                        Dim cmdText As String = ""
                        cmdText &= "INSERT INTO srv_thrsRecommendationsService (ResultServiceID, RecommendationID) " & vbCrLf
                        cmdText &= "VALUES ( " & rowthrsRecommendationsService.ResultServiceID & vbCrLf
                        cmdText &= "       , '" & rowthrsRecommendationsService.RecommendationID & "')"

                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        recordOK = (resultData.AffectedRecords = 1)
                        i += 1

                        If (Not recordOK) Then Exit For
                    Next rowthrsRecommendationsService

                    If (recordOK) Then
                        resultData.HasError = False
                        resultData.AffectedRecords = i
                        resultData.SetDatos = pRecommendationsList
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        resultData.AffectedRecords = 0
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsRecommendationsServiceDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the existing recommendations that corresponds with the registered activities
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 03/08/2011</remarks>
        Public Function ReadByResultServiceID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim myLocalBase As New GlobalBase

                        cmdText &= "SELECT MR.ResourceText AS FixedItemDesc " & vbCrLf
                        cmdText &= "FROM srv_thrsRecommendationsService RS " & vbCrLf
                        cmdText &= "INNER JOIN srv_tfmwRecommendations R " & vbCrLf
                        cmdText &= "       ON RS.RecommendationID = R.RecommendationID " & vbCrLf
                        cmdText &= "INNER JOIN tfmwMultiLanguageResources MR " & vbCrLf
                        cmdText &= "       ON R.ResourceID = MR.ResourceID " & vbCrLf
                        cmdText &= "WHERE  RS.ResultServiceID = " & pResultServiceID.ToString() & vbCrLf
                        cmdText &= "  AND  MR.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf

                        Dim myDataSet As New SRVRecommendationsServiceDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.srv_thrsRecommendationsService)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsRecommendationsServiceDAO.ReadByResultServiceID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Activity 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultServiceID">Result report Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 04/08/2011
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As New SqlCommand

                    cmdText &= " DELETE FROM  srv_thrsRecommendationsService "
                    cmdText &= " WHERE ResultServiceID = " & pResultServiceID

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsRecommendationsServiceDAO.Delete", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace
