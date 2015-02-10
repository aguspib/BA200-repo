Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwInfoDocumentsDAO
          

#Region "CRUD Methods"
       
        Public Function ReadDocumentPath(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String, ByVal pAppPageID As String, _
                             Optional ByVal pLanguageID As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If (pLanguageID = "") Then
                            pLanguageID = GlobalBase.GetSessionInfo.ApplicationLanguage
                        End If



                        cmdText = " SELECT AnalyzerModel,ApplicationPage, Language, DocumentPath, VideoPath, Expandable " & _
                                  " FROM   srv_tfmwInfoDocuments " & _
                                  " WHERE  AnalyzerModel = '" & pAnalyzerModel & "' " & _
                                  " AND    ApplicationPage   = '" & pAppPageID & "' " & _
                                  " AND    Language= '" & pLanguageID & "' "


                        Dim myDocuments As New SRVInfoDocumentsDS
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDocuments.srv_tfmwInfoDocuments)
                            End Using
                        End Using
                        resultData.SetDatos = myDocuments
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwInfoDocumentsDAO.ReadDocumentPath", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

       
#End Region

    End Class

End Namespace

