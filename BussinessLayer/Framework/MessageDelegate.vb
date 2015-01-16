Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL



Namespace Biosystems.Ax00.BL

    Public Class MessageDelegate


#Region "Methods"

        ''' <summary>
        ''' Get details of the specified Message, including its type and text in the current Application Language
        ''' </summary>
        ''' <param name="pMsgID">Message ID</param>
        ''' <param name="pLanguageID">Optional parameter. Language Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MessagesDS with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by:  VR 27/11/2009
        ''' Modified by: BK 23/12/2009 - Change in call method ReadByMessageID to Read
        '''              SA 07/01/2010 - Changed validation to determine if a Master Data Missing Error has to be returned
        '''              SA 04/11/2010 - Added optional parameter for LanguageID to allow get the message text
        '''                              when the Session object has not been initialized yet (for instance, when a
        '''                              wrong UserName is informed in the Login screen)
        ''' </remarks>
        Public Function GetMessageDescription(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMsgID As String, _
                                              Optional ByVal pLanguageID As String = "") As GlobalDataTO
            Dim messageData As New MessagesDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myMesagesDAO As New tfmwMessagesDAO
                        resultData = myMesagesDAO.Read(dbConnection, pMsgID, pLanguageID)
                        'TR 23/12/2011 -Validate the has error property to avoid exection error.
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            messageData = CType(resultData.SetDatos, MessagesDS)
                            If (messageData.tfmwMessages.Rows.Count = 0) Then
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MessageDelegate.GetMessageDescription", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details all Message, including its type and text in the current Application Language
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet MessagesDS with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by:  SG 01/08/2010
        ''' </remarks>
        Public Function GetAllMessageDescription(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim messageData As New MessagesDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myMesagesDAO As New tfmwMessagesDAO
                        resultData = myMesagesDAO.ReadAll(dbConnection)

                        If (Not resultData.SetDatos Is Nothing) Then
                            messageData = CType(resultData.SetDatos, MessagesDS)
                            If (messageData.tfmwMessages.Rows.Count = 0) Then
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MessageDelegate.GetAllMessageDescription", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace