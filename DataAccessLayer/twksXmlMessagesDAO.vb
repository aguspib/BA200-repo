Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports System.Xml

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksXmlMessagesDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Saves a new XML into twksXMLMessages table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pxmlDoc">XML Message</param>
        ''' <param name="pMsgStatus">Message Status</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2013
        ''' Modified by: XB 28/02/2013 - Complete implementation
        '''              SA 02/04/2013 - Changed the SQL Query to insert also new field TS_DateTime with current date and time
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, ByVal pxmlDoc As XmlDocument, ByVal pMsgStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim nonqueryCommand As SqlCommand = pDBConnection.CreateCommand()
                    nonqueryCommand.CommandText = " INSERT INTO twksXMLMessages (MessageID, XMLMessage, Status, TS_DateTime) VALUES (@MessageID, @XMLMessage, @Status, GETDATE()) "

                    nonqueryCommand.Parameters.Add("@MessageID", SqlDbType.VarChar, 40)
                    nonqueryCommand.Parameters.Add("@XMLMessage", SqlDbType.Xml, -1)
                    nonqueryCommand.Parameters.Add("@Status", SqlDbType.VarChar, 15)

                    nonqueryCommand.Prepare()

                    nonqueryCommand.Parameters("@MessageID").Value = pMessageID
                    nonqueryCommand.Parameters("@XMLMessage").Value = pxmlDoc.InnerXml
                    nonqueryCommand.Parameters("@Status").Value = pMsgStatus


                    'Execute the SQL sentence 
                    Using nonqueryCommand
                        resultData.AffectedRecords = nonqueryCommand.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified XML Message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Identifier of the Message to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksXMLMessages " & vbCrLf & _
                                            " WHERE  MessageID = '" & pMessageID & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the specified XML Message in table twksXmlMessages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <returns>GlobalDataTO containing an XMLMessageTO with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myXMLMessage As New XMLMessagesTO
                Dim myTempXMLMessage As New XMLMessagesTO

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTSQL As String = " SELECT MessageID, Status FROM twksXMLMessages " & _
                                               " WHERE  MessageID = '" & pMessageID & "' "

                        Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()

                                If (Not dbDataReader.IsDBNull(0)) Then
                                    myTempXMLMessage.MessageID = dbDataReader.Item("MessageID").ToString
                                    myTempXMLMessage.Status = dbDataReader.Item("Status").ToString
                                End If

                            End If
                            dbDataReader.Close()
                        End Using

                        If myTempXMLMessage.MessageID.Length > 0 Then
                            Dim myTSQL2 As String = " SELECT XMLMessage FROM twksXMLMessages " & _
                                                    " WHERE  MessageID = '" & pMessageID & "' "

                            Using dbCmd2 As New SqlClient.SqlCommand(myTSQL2, dbConnection)
                                Dim dbDataXmlReader As XmlReader = dbCmd2.ExecuteXmlReader()
                                If (dbDataXmlReader.Read) Then
                                    myXMLMessage.MessageID = myTempXMLMessage.MessageID
                                    myXMLMessage.Status = myTempXMLMessage.Status
                                    myXMLMessage.XMLMessage = New XmlDocument
                                    myXMLMessage.XMLMessage.Load(dbDataXmlReader)
                                End If
                                dbDataXmlReader.Close()
                            End Using
                        End If

                        resultData.SetDatos = myXMLMessage
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all XML Messages in table twksXMLMessages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a list of XMLMessagesTO, which is an structure containing MessageID, Status and XML</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2013
        ''' Modified by: XB 28/02/2013 - Complete implementation
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myXMLMessages As New List(Of XMLMessagesTO)
                Dim myTempXMLMessages As New List(Of XMLMessagesTO)
                Dim myRowXMLMessages As XMLMessagesTO

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTSQL As String = " SELECT MessageID, Status FROM twksXMLMessages "

                        Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                While (dbDataReader.Read())
                                    If (Not dbDataReader.IsDBNull(0)) Then
                                        myRowXMLMessages = New XMLMessagesTO

                                        myRowXMLMessages.MessageID = dbDataReader.Item("MessageID").ToString
                                        myRowXMLMessages.Status = dbDataReader.Item("Status").ToString

                                        myTempXMLMessages.Add(myRowXMLMessages)
                                    End If
                                End While
                            End If
                            dbDataReader.Close()
                        End Using

                        For i As Integer = 0 To myTempXMLMessages.Count - 1
                            Dim myTSQL2 As String = " SELECT XMLMessage FROM twksXMLMessages " & _
                                                    " WHERE  MessageID = '" & myTempXMLMessages(i).MessageID & "' "

                            Using dbCmd2 As New SqlClient.SqlCommand(myTSQL2, dbConnection)
                                Dim dbDataXmlReader As XmlReader = dbCmd2.ExecuteXmlReader()
                                If (dbDataXmlReader.Read) Then
                                    myRowXMLMessages = New XMLMessagesTO

                                    myRowXMLMessages.MessageID = myTempXMLMessages(i).MessageID
                                    myRowXMLMessages.Status = myTempXMLMessages(i).Status
                                    myRowXMLMessages.XMLMessage = New XmlDocument
                                    myRowXMLMessages.XMLMessage.Load(dbDataXmlReader)

                                    myXMLMessages.Add(myRowXMLMessages)
                                End If
                                dbDataXmlReader.Close()
                            End Using
                        Next

                        resultData.SetDatos = myXMLMessages
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Delete All XML Messages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksXMLMessages " 

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete specified XML Messages depending on Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Status that should have the Messages to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function DeleteByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksXMLMessages " & vbCrLf & _
                                            " WHERE  Status = '" & pStatus & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.DeleteByStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all XML Messages in table twksXMLMessages having the informed status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Status that should have the XML Messages to read</param>
        ''' <returns>GlobalDataTO containing a list of XMLMessagesTO, which is an structure containing MessageID, Status and XML</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2013
        ''' Modified by: XB 28/02/2013 - Completed implementation
        '''              SA 03/04/2013 - Changed the SQL Query by adding Order By clause by new field TS_DateTime
        '''              SA 31/03/2014 - BT #1564 ==> Changed the SQL Query to save in each XMLMessagesTO to return the Message Date and Time 
        '''                                           contained in field TS_DateTime 
        ''' </remarks>
        Public Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myXMLMessages As New List(Of XMLMessagesTO)
                Dim myTempXMLMessages As New List(Of XMLMessagesTO)
                Dim myRowXMLMessages As XMLMessagesTO

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTSQL As String = " SELECT MessageID, Status, TS_DateTime " & vbCrLf & _
                                               " FROM   twksXMLMessages " & vbCrLf & _
                                               " WHERE  Status = '" & pStatus.Trim & "' " & vbCrLf & _
                                               " ORDER BY TS_DateTime " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                While (dbDataReader.Read())
                                    If (Not dbDataReader.IsDBNull(0)) Then
                                        myRowXMLMessages = New XMLMessagesTO

                                        myRowXMLMessages.MessageID = dbDataReader.Item("MessageID").ToString
                                        myRowXMLMessages.Status = dbDataReader.Item("Status").ToString
                                        myRowXMLMessages.MsgDateTime = CDate(dbDataReader.Item("TS_DateTime"))
                                        myTempXMLMessages.Add(myRowXMLMessages)
                                    End If
                                End While
                            End If
                            dbDataReader.Close()
                        End Using

                        For i As Integer = 0 To myTempXMLMessages.Count - 1
                            Dim myTSQL2 As String = " SELECT XMLMessage FROM twksXMLMessages " & _
                                                    " WHERE  MessageID = '" & myTempXMLMessages(i).MessageID & "' "

                            Using dbCmd2 As New SqlClient.SqlCommand(myTSQL2, dbConnection)
                                Dim dbDataXmlReader As XmlReader = dbCmd2.ExecuteXmlReader()
                                If (dbDataXmlReader.Read) Then
                                    myRowXMLMessages = New XMLMessagesTO

                                    myRowXMLMessages.MessageID = myTempXMLMessages(i).MessageID
                                    myRowXMLMessages.Status = myTempXMLMessages(i).Status
                                    myRowXMLMessages.MsgDateTime = myTempXMLMessages(i).MsgDateTime
                                    myRowXMLMessages.XMLMessage = New XmlDocument
                                    myRowXMLMessages.XMLMessage.Load(dbDataXmlReader)

                                    myXMLMessages.Add(myRowXMLMessages)
                                End If
                                dbDataXmlReader.Close()
                            End Using
                        Next

                        resultData.SetDatos = myXMLMessages
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.ReadByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of all XML Messages having the specified previous status 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreviousStatus">Previous Status</param>
        ''' <param name="pStatus">New Status</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 14/03/2013
        ''' Modified By: TR 21/03/2013 - Changed the SQL Query: instead of updating an specific Message (by MessageID), update all Messages 
        '''                              having the specified previous Status
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, pPreviousStatus As String, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText = " UPDATE twksXMLMessages "
                    cmdText &= " SET Status = '" & pStatus & "' "
                    cmdText &= " WHERE  Status = '" & pPreviousStatus & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.UpdateStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  Update the status of the specified Message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageId">Identifier of the Message to update</param>
        ''' <param name="pStatus">New status for the specified Message</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: SGM 22/03/2013
        ''' Modified by: AG 27/03/2013 - Fixed issue (is N' instead of 'N)</remarks>
        Public Function UpdateStatusByMessageId(ByVal pDBConnection As SqlClient.SqlConnection, pMessageId As String, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText = "  UPDATE  twksXMLMessages "
                    cmdText &= " SET     Status = '" & pStatus & "' "
                    cmdText &= " WHERE   MessageId = N'" & pMessageId & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksXmlMessagesDAO.UpdateStatusByMessageId", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
