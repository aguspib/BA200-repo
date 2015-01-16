Option Strict On
Option Explicit On

'Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwMessagesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get details of the specified Message, including its type and text in the current Application Language
        ''' </summary>
        ''' <param name="pMsgID">Message ID</param>
        ''' <param name="pLanguageID">Optional parameter. Language Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MessagesDS with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by:  VR 27/11/2009
        ''' Modified by: BK 23/12/2009 - Change Function name to Read; changes in Query: 'FixedItemDesc AS MessageText'
        '''              SA 07/01/2010 - Changed the Exception area: error was not informed in the GlobalDataTO. Throw ex removed.
        '''              AG 08/10/2010 - Adapt to multilanguage
        '''              SA 04/11/2010 - Added optional parameter for LanguageID to allow get the message text
        '''                              when the Session object has not been initialized yet (for instance, when a
        '''                              wrong UserName is informed in the Login screen)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMsgID As String, _
                             Optional ByVal pLanguageID As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim cmdText As String = ""

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim cmdText As String = ""

                        If (pLanguageID = "") Then
                            Dim myLocalBase As New GlobalBase
                            pLanguageID = GlobalBase.GetSessionInfo.ApplicationLanguage
                        End If

                        'AG 08/10/2010
                        'cmdText = " SELECT MessageID, MessageType, FixedItemDesc AS MessageText " & _
                        '          " FROM   tfmwMessages " & _
                        '          " WHERE  MessageID = '" & pMsgID & "' "

                        cmdText = " SELECT M.MessageID, M.MessageType, MR.ResourceText AS MessageText " & _
                                  " FROM   tfmwMessages M INNER JOIN tfmwMultiLanguageResources MR ON M.ResourceID = MR.ResourceID " & _
                                  " WHERE  MR.LanguageID = '" & pLanguageID & "' " & _
                                  " AND    M.MessageID   = '" & pMsgID & "' "
                        'END AG 08/10/2010

                        Dim myMessages As New MessagesDS
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(cmdText, dbConnection)
                                dbDataAdapter.Fill(myMessages.tfmwMessages)
                            End Using
                        End Using
                        resultData.SetDatos = myMessages
                        resultData.HasError = False
                    End If
                End If

                'DL 09/03/2012. Begin
                'Catch ex As Exception
                '    resultData.HasError = True
                '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                '    resultData.ErrorMessage = ex.Message

                '    Dim myLogAcciones As New ApplicationLogManager()
                '    GlobalBase.CreateLogActivity(ex.Message, "tfmwMessagesDAO.Read", EventLogEntryType.Error, False)

            Catch ex As SqlException

                ' After restore DB, the first time to connect success the next error: 
                ' -2146232060 Error en el nivel de transporte al enviar la solicitud al servidor. (provider: Proveedor de 
                '             memoria compartida, error: 0 - No hay ningún proceso en el otro extremo de la canalización.)
                ' To Solve this BUG only need to try fill ds.

                If ex.ErrorCode = -2146232060 AndAlso ex.Errors(0).Number = 233 Then
                    Dim myMessages As New MessagesDS

                    'Fill the DataSet to return 
                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(cmdText, dbConnection)
                        dbDataAdapter.Fill(myMessages.tfmwMessages)
                    End Using

                    resultData.SetDatos = myMessages
                    resultData.HasError = False

                Else
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    resultData.ErrorMessage = ex.Message

                    Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity(ex.Message, "tfmwMessagesDAO.Read", EventLogEntryType.Error, False)
                End If
                'DL 03/09/2012. End

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all Message, including its type and text in the current Application Language
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet MessagesDS with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by:  SG 01/09/2010
        ''' Modified by: AG 08/10/2010 - adapt to multilanguage
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
                        Dim myLocalBase As New GlobalBase

                        'AG 08/10/2010
                        'cmdText = " SELECT MessageID, MessageType, FixedItemDesc AS MessageText " & _
                        '          " FROM   tfmwMessages "

                        cmdText = " SELECT M.MessageID, M.MessageType, MR.ResourceText AS MessageText " & _
                                  " FROM   tfmwMessages M INNER JOIN tfmwMultiLanguageResources MR ON M.ResourceID = MR.ResourceID " & _
                                  " WHERE  MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' "
                        'AG 08/10/2010

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myMessages As New MessagesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myMessages.tfmwMessages)

                        resultData.SetDatos = myMessages
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwMessagesDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace

