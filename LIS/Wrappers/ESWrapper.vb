'Class creation 22/02/2013 AG
'Based on SysteLab demo code EmbeddedSynapse.vb class

Imports System
Imports System.ComponentModel
Imports System.Text
Imports System.Xml
Imports System.Runtime.InteropServices
Imports System.Diagnostics
Imports NteCommunicationCoreModuleLib

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports System.IO

Namespace Biosystems.Ax00.LISCommunications

#Region "Declarations Enums (SysteLab code but not used)"

    'Public Enum CHANNEL_STATUS
    '    CHANNEL_DISABLED = 0
    '    CHANNEL_ENABLED
    '    CHANNEL_ACCEPTED
    '    CHANNEL_REJECTED
    '    CHANNEL_CONNECTING_SKIPPED
    'End Enum

    'Public Enum CONNECTION_STATUS
    '    CHANNEL_CONNECTION = 0
    '    CHANNEL_NOCONNECTION
    'End Enum

#End Region

#Region "Declaration delegate (SysteLab code, event delegate - USED!!)"

    Public Delegate Sub OnNotificationHandlerType(channelName As String, priority As Integer, message As XmlDocument)

#End Region


    Partial Public Class ESWrapper

#Region "Fields & delegates"

        Public Const DEFAULT_MAX_NOTIFICATION_TIMEOUT As Integer = 1000

        Private xmlHelper As xmlHelper = Nothing
        Private driverManager As DriverMgr = Nothing
        Private applicationIDAttribute As String = String.Empty
        Private channelIDAttribute As String = String.Empty
        Private statusAttribute As String = GlobalEnumerates.LISStatus.unknown.ToString 'This attribute is informed when Control Information (status) is received

        Private storageAttribute As String = "0" ' XB 19/04/2013
        ' Values based on Systelab documentation: Value = 0 or 75 or 80 or 85 or 90 or 95 or 100
        ' If "full" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") then
        '    Value = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:value") '75 or 80 or 85 or 90 or 95 or 100
        ' If "normal" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:normal") then
        '    Value = 0
        ' If "overloaded" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") then
        '    Value = 100
        ' End If

        'SGM 26/02/2012
        Private ApplicationVersionAttr As String = String.Empty
        Private AnalyzerModelAttr As String = String.Empty
        Private AnalyzerSerialNumberAttr As String = String.Empty
        'end SGM 26/02/2013

        Private specimensWithNoResponseAttribute As New List(Of String) 'AG 22/07/2013 - list of specimens asked but with no response by LIS
        Private uploadResultsMessagesPendingNotificationAttribute As Integer = 0 'AG 07/03/2014 - #1533 count the number of upload results sent and pending to receive notification (DELIVERED, UNDELIVERED, UNRESPONDED)
        '                                                                                         the presentation refresh screen methods will be executed only when this flag arrives to 0 (1 time, not N times for N messages)

        ''' <summary>
        ''' Event fired when a new message (order download) has been received
        ''' </summary>
        ''' <remarks></remarks>
        Public Event OnLISMessage As OnNotificationHandlerType

        ''' <summary>
        ''' Event fired when a new notification (control information or query response) has been received
        ''' </summary>
        ''' <remarks></remarks>
        Public Event OnLISNotification As OnNotificationHandlerType


#End Region

#Region "Constructors & dispose"

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pApplicationID"></param>
        ''' <param name="pChannelID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pAnalyzerSN"></param>
        ''' <param name="pAppVersion"></param>
        ''' <remarks>
        ''' Modified by SGM 26/02/2013
        '''             XB  18/03/2013 - folder information becomes as a parameter instead of configsettings
        '''             XB  11/04/2013 - SetMessageStorage is emplaced inside CreateChannel to be available also from LIS settings Config screen 
        ''' </remarks>
        Public Sub New(ByVal pApplicationID As String, ByVal pChannelID As String, _
                       ByVal pAnalyzerModel As String, _
                       ByVal pAnalyzerSN As String, _
                       Optional ByVal pAppVersion As String = "")

            'SGM 26/02/2013
            MyClass.applicationIDAttribute = pApplicationID
            MyClass.channelIDAttribute = pChannelID
            MyClass.AnalyzerModelAttr = pAnalyzerModel
            MyClass.AnalyzerSerialNumberAttr = pAnalyzerSN
            If pAppVersion.Length > 0 Then MyClass.ApplicationVersionAttr = pAppVersion
            'end SGM 26/02/2013
            MyClass.specimensWithNoResponseAttribute.Clear()  'AG 22/07/2013
            MyClass.uploadResultsMessagesPendingNotificationAttribute = 0 'AG 07/03/2014 - #1533

            Me.xmlHelper = New xmlHelper("udc", "http://www.nte.es/schema/udc-interface-v1.0",
                                        "ci", "http://www.nte.es/schema/clinical-information-v1.0")

            Me.driverManager = New DriverMgr()
            AddHandler driverManager.Notification, AddressOf OnNotificationEventHandler
            'Validate if the storage directory does not exist then create it.

            ' XB 11/04/2013
            'Dim resultData As GlobalDataTO = Nothing
            'Dim sendFolder As String = "Storage"
            'Dim receiveFolder As String = "Storage"
            'Dim myParams As New SwParametersDelegate
            'Dim myParametersDS As New ParametersDS
            '' Read application name for LIS parameter
            'resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_STORAGE_TRANS_FOLDER.ToString, Nothing)
            'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '    myParametersDS = CType(resultData.SetDatos, ParametersDS)
            '    If myParametersDS.tfmwSwParameters.Count > 0 Then
            '        sendFolder = myParametersDS.tfmwSwParameters.Item(0).ValueText
            '    End If
            'End If
            'sendFolder = My.Application.Info.DirectoryPath & "\" & sendFolder

            '' Read channel ID for LIS parameter
            'resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_STORAGE_RECEPTION_FOLDER.ToString, Nothing)
            'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '    myParametersDS = CType(resultData.SetDatos, ParametersDS)
            '    If myParametersDS.tfmwSwParameters.Count > 0 Then
            '        receiveFolder = myParametersDS.tfmwSwParameters.Item(0).ValueText
            '    End If
            'End If
            'receiveFolder = My.Application.Info.DirectoryPath & "\" & receiveFolder

            'If Not Directory.Exists(sendFolder) Then
            '    Directory.CreateDirectory(sendFolder)
            'End If
            'If Not Directory.Exists(receiveFolder) Then
            '    Directory.CreateDirectory(receiveFolder)
            'End If

            'Me.SetMessageStorage(Nothing, 9999, sendFolder, 9999, receiveFolder)
            ' XB 11/04/2013

        End Sub

        Public Sub Dispose()
            MyClass.ClearQueueOfSpecimenNotResponded() 'AG 22/07/2013

            'Unsubscribe from object events to release references from the object
            RemoveHandler driverManager.Notification, AddressOf OnNotificationEventHandler

            'This will force the inmediate ReleaseRef of the object interface
            Marshal.ReleaseComObject(driverManager)
            driverManager = Nothing

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property ApplicationID As String
            Get
                Return applicationIDAttribute
            End Get
        End Property

        Public ReadOnly Property ChannelID As String
            Get
                Return channelIDAttribute
            End Get
        End Property

        Public Property Status As String
            Get
                Return statusAttribute
            End Get
            Set(ByVal value As String)
                statusAttribute = value
            End Set
        End Property

        Public Property Storage As String
            Get
                Return storageAttribute
            End Get
            Set(ByVal value As String)
                storageAttribute = value
            End Set
        End Property


        ''' <remarks>Created by SGM 26/02/2013</remarks>
        Private ReadOnly Property ApplicationVersion As String
            Get
                Return ApplicationVersionAttr
            End Get
        End Property

        ''' <remarks>Created by SGM 26/02/2013</remarks>
        Private ReadOnly Property AnalyzerModel As String
            Get
                Return AnalyzerModelAttr
            End Get
        End Property

        ''' <remarks>Created by SGM 26/02/2013</remarks>
        Private ReadOnly Property AnalyzerSerialNumber As String
            Get
                Return AnalyzerSerialNumberAttr
            End Get
        End Property

        Public ReadOnly Property specimensWithNoResponse As Integer
            Get
                Dim lockThis As New Object
                SyncLock lockThis
                    Return specimensWithNoResponseAttribute.Count
                End SyncLock
            End Get
        End Property
#End Region

#Region "Public methods"

        ''' <summary>
        ''' Channel creation between ES library and BAx00 application
        ''' 
        ''' SysteLab documentation: 
        ''' The CreateChannel (…) method creates new communication channel between the application and systems or devices. 
        ''' The creation of a channel only implies that the physical resources required by the channel are started, 
        ''' but data cannot be sent until the channel is in the connected state. Parameters required are the application ID, 
        ''' a unique value for each application that uses Embedded Synapse, and the xml to create the channel
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        '''          XB 11/04/2013 - SetMessageStorage is emplaced inside CreateChannel to be available also from LIS settings Config screen.
        '''          TR 24/05/2013 - Validate if the LIS Trace level is configure in other case then create it and set by default the medium level.
        ''' </remarks>
        Public Function CreateChannel(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'TR 24/05/2013
                        Dim myUtilities As New Utilities
                        resultData = myUtilities.GetLISTraceLevel()
                        If Not resultData.HasError Then
                            If resultData.SetDatos.ToString() = "NONE" Then
                                'Set the default trace level (MEDIUM) in the registry.
                                resultData = myUtilities.SetLISTraceLevel("MEDIUM")
                                If Not resultData.HasError Then
                                    resultData.SetDatos = "MEDIUM"
                                End If
                            End If
                            'Save on tcfgUserSettings the saved value.
                            Dim myUserSettingsDelegate As New UserSettingsDelegate
                            resultData = myUserSettingsDelegate.Update(Nothing, UserSettingsEnum.LIS_TRACE_LEVEL.ToString(), resultData.SetDatos.ToString())
                        End If
                        'TR 24/05/2013 -END 
                        ' XB 11/04/2013
                        Dim sendFolder As String = "Storage"
                        Dim receiveFolder As String = "Storage"
                        Dim myParams As New SwParametersDelegate
                        Dim myParametersDS As New ParametersDS
                        ' Read application name for LIS parameter
                        resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_STORAGE_TRANS_FOLDER.ToString, Nothing)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myParametersDS = CType(resultData.SetDatos, ParametersDS)
                            If myParametersDS.tfmwSwParameters.Count > 0 Then
                                sendFolder = myParametersDS.tfmwSwParameters.Item(0).ValueText
                            End If
                        End If
                        sendFolder = My.Application.Info.DirectoryPath & "\" & sendFolder

                        ' Read channel ID for LIS parameter
                        resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_STORAGE_RECEPTION_FOLDER.ToString, Nothing)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myParametersDS = CType(resultData.SetDatos, ParametersDS)
                            If myParametersDS.tfmwSwParameters.Count > 0 Then
                                receiveFolder = myParametersDS.tfmwSwParameters.Item(0).ValueText
                            End If
                        End If
                        receiveFolder = My.Application.Info.DirectoryPath & "\" & receiveFolder

                        If Not Directory.Exists(sendFolder) Then
                            Directory.CreateDirectory(sendFolder)
                        End If
                        If Not Directory.Exists(receiveFolder) Then
                            Directory.CreateDirectory(receiveFolder)
                        End If

                        resultData = Me.SetMessageStorage(Nothing, 9999, sendFolder, 9999, receiveFolder)
                        If resultData.HasError Then
                            Exit Try
                        End If
                        ' XB 11/04/2013


                        'Get the xml message
                        Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber)
                        resultData = translator.GetCreateChannel(dbConnection, applicationIDAttribute)
                        'resultData = translator.GetCreateChannelSTLServer(dbConnection, applicationIDAttribute)
                        'resultData = translator.GetCreateChannelSTLClient(dbConnection, applicationIDAttribute)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myCommand As String = String.Empty
                            myCommand = CType(resultData.SetDatos, String)

                            'Call method in ES library
                            Me.driverManager.CreateChannel(applicationIDAttribute, myCommand)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.CreateChannel", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Connection establishment with ES library
        ''' 
        ''' SysteLab documentation:
        ''' The Connect (…) and Disconnect (…) allows the application to start/close a session required for the 
        ''' transmission of data. When the channel is disconnected, data cannot be transferred through a channel.
        ''' </summary>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function Connect() As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Me.driverManager.Connect(channelIDAttribute)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.Connect", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Disconnection from ES library
        ''' 
        ''' SysteLab documentation:
        ''' The Connect (…) and Disconnect (…) allows the application to start/close a session required for the 
        ''' transmission of data. When the channel is disconnected, data cannot be transferred through a channel.
        ''' </summary>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function Disconnect() As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Me.driverManager.Disconnect(channelIDAttribute)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.Disconnect", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Remove all existing channels
        ''' 
        ''' SysteLab documentation:
        ''' The ReleaseChannel (…) and ReleaseAllChannels () methods remove one or all channels synchronously. In addition, 
        ''' there are methods to release the channels asynchronously. Keep in mind that when releasing a channel asynchronously 
        ''' you have to call Begin_ReleaseChannel (…) to start the release of the channel, execute any other code and call 
        ''' Finish_ReleaseChannel(…) to wait until the channel is released. If the channel is released this function will return 
        ''' immediately, otherwise it will wait until it is closed.
        ''' </summary>
        ''' <param name="pTimeout"></param>
        ''' <remarks></remarks>
        Public Function ReleaseAllChannels(ByVal pTimeout As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Me.driverManager.ReleaseAllChannels(pTimeout)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.ReleaseAllChannels", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the current pending messages in queue to be sent to LIS
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function GetPendingMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then


                        'Get the xml message
                        Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                        resultData = translator.GetPendingMessages(dbConnection)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myCommand As String = String.Empty
                            myCommand = CType(resultData.SetDatos, String)

                            'Send command to ES library
                            Me.driverManager.SendCommand(myCommand)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.GetPendingMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Delete all messages pending to be sent to LIS (Clean storage)
        ''' 
        ''' SysteLab documentation:
        ''' This commands allows the application to cancel the execution of a pending command or delete an unsent message. 
        ''' This command event allows the hosting application to empty the entire message queue. This is can be useful since 
        ''' is the only way the user have to purge all messages from the storage. 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function DeleteAllMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then


                        'Get the xml message
                        Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                        resultData = translator.GetDeleteAllMessages(dbConnection)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myCommand As String = String.Empty
                            myCommand = CType(resultData.SetDatos, String)

                            'Send command to ES library
                            Me.driverManager.SendCommand(myCommand)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.DeleteAllMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Delete a specific message pending to be sent to LIS
        ''' 
        ''' SysteLab documentation:
        ''' This commands allows the application to cancel the execution of a pending command or delete an unsent message.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pMessageID">Message Identifier to be delete</param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function DeleteMessage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then


                        'Get the xml message
                        Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                        resultData = translator.GetDeleteMessage(dbConnection, pMessageID)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myCommand As String = String.Empty
                            myCommand = CType(resultData.SetDatos, String)

                            'Send command to ES library
                            Me.driverManager.SendCommand(myCommand)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.DeleteMessage", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Delete the incoming LIS -> ES queue
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Public Function DeleteIncomingMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'ES Library offers this functionality or not????

                        'Get the xml message
                        'Dim translator As New ESxmlTranslator
                        'resultData = translator.GetDeleteIncomingMessages(dbConnection, ChannelIDAttribute)
                        '
                        'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        '   Dim myCommand As String = String.Empty
                        '   myCommand = CType(resultData.SetDatos, String)
                        '
                        '   'Send command to ES library
                        '   Me.driverManager.SendCommand(myCommand)
                        'End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.DeleteIncomingMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Sent a query all request to LIS
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' </remarks>
        Public Function QueryAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the messageID
                        Dim myUtils As New Utilities
                        resultData = myUtils.GetNewGUID
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                            resultData = translator.GetQueryAll(dbConnection, msgId)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myCommand As String = String.Empty
                                myCommand = CType(resultData.SetDatos, String)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.QueryAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Sent a host query request to LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSpecimenList">List of SpecimenIDs</param>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013
        ''' Modified by: AG 21/03/2013 - MessageID is generated by wrapper
        ''' AG 22/07/2013 - Add the specimens asked into a internal queue of specimen asked but not responded
        ''' </remarks>
        Public Function HostQuery(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSpecimenList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the messageID
                        Dim myUtils As New Utilities

                        resultData = myUtils.GetNewGUID
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties

                            resultData = translator.GetHostQuery(dbConnection, msgId, pSpecimenList)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myCommand As String =  CType(resultData.SetDatos, String)

                                'AG 22/07/2013
                                Dim lockThis As New Object
                                SyncLock lockThis
                                    For Each item As String In pSpecimenList
                                        If Not specimensWithNoResponseAttribute.Contains(item) Then
                                            specimensWithNoResponseAttribute.Add(item)
                                        End If
                                    Next
                                End SyncLock
                                'AG 22/07/2013

                                'AG 02/01/2014 - BT #1433 add new log traces (v211 patch2)
                                Dim myLogAcciones As New ApplicationLogManager()
                                myLogAcciones.CreateLogActivity("Send host query message: " & msgId, "ESWrapper.HostQuery", EventLogEntryType.Information, False)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)

                                'Update THE MessageId: to relate specimen with messageID (for status ASKING)
                                'Do not use dbConnection, use Nothing
                                Dim barcodeDelegate As BarcodePositionsWithNoRequestsDelegate = New BarcodePositionsWithNoRequestsDelegate()
                                For Each sPeciment In pSpecimenList
                                    resultData = barcodeDelegate.UpdateMessageIDBySpecimenID(Nothing, MyClass.AnalyzerSerialNumber, "SAMPLES", sPeciment, msgId)
                                Next
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.HostQuery", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Sent a awos accepts message to LIS - CANCELLED v1.1.0
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAwosToAcceptList"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' </remarks>
        Public Function AcceptAwos(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosToAcceptList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'CANCELLED v1.1.0

                        ''Get the messageID
                        'Dim myUtils As New Utilities
                        'resultData = myUtils.GetNewGUID
                        'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        '    Dim msgId As String = CType(resultData.SetDatos, String)

                        '    'Get the xml message
                        '    Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber)
                        '    resultData = translator.GetAwosReject(dbConnection, pMessageID, pAwosToAcceptList, False)

                        '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        '        Dim myCommand As String = String.Empty
                        '        myCommand = CType(resultData.SetDatos, String)

                        '        'Send command to ES library
                        '        Me.driverManager.SendCommand(myCommand)
                        '    End If
                        'End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.AcceptAwos", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Sent a complete order acceptance message to LIS after reception and save every orderdownload message
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <remarks>
        ''' Creation AG 08/03/2013
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' </remarks>
        Public Function AcceptCompleteOrder(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Get the messageID
                        Dim myUtils As New Utilities
                        resultData = myUtils.GetNewGUID
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber)

                            Dim emptyList As New List(Of String)
                            emptyList.Clear()
                            resultData = translator.GetAwosAccept(dbConnection, msgId, emptyList, True)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myCommand As String = String.Empty
                                myCommand = CType(resultData.SetDatos, String)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.AcceptAwos", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Sent an awos rejection message to LIS (during the quick message validation after message reception)
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAwosToRejectList"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' </remarks>
        Public Function RejectAwos(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosToRejectList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Get the messageID
                        Dim myUtils As New Utilities
                        resultData = myUtils.GetNewGUID
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                            resultData = translator.GetAwosReject(dbConnection, msgId, pAwosToRejectList)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myCommand As String = String.Empty
                                myCommand = CType(resultData.SetDatos, String)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.RejectAwos", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Sent an awos rejection message to LIS (during the validation before add it to the current worksession)
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAwosToRejectList"></param>
        ''' <remarks>
        ''' Creation AG 13/03/2013
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' </remarks>
        Public Function RejectAwosDelayed(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAwosToRejectList As OrderTestsLISInfoDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Get the messageID
                        Dim myUtils As New Utilities
                        resultData = myUtils.GetNewGUID
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber)
                            resultData = translator.GetAwosRejectDelayed(dbConnection, msgId, pAwosToRejectList)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myCommand As String = String.Empty
                                myCommand = CType(resultData.SetDatos, String)

                                'AG 02/01/2014 - BT #1433 add new log traces (v211 patch2)
                                Dim myLogAcciones As New ApplicationLogManager()
                                myLogAcciones.CreateLogActivity("Send message rejecting awos: " & msgId, "ESWrapper.RejectAwosDelayed", EventLogEntryType.Information, False)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.RejectAwosDelayed", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Sent orders results message to LIS
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pToUploadDS">Results to upload</param>
        ''' <param name="pHistoricalFlag">True when exportation is executed from historical results</param>
        ''' <param name="pTestMappingDS"></param>
        ''' <param name="pConfigMappingDS"></param>
        ''' <param name="pResultsDS">Informed when historicalflag FALSE</param>
        ''' <param name="pResultAlarmsDS">Informed when historicalflag FALSE</param>
        ''' <param name="pHistDataDS">Only informed when called from historical results. Contains the current data in screen with the selected filters</param>
        ''' <returns>GlobalDataTo (data = integer that counts the results not sent)</returns>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' AG 27/02/2013 - use new parameter 'ByVal pToUploadDS As ExecutionsDS' instead of 'ByVal pAwosToUploadResultList As List(Of String)'
        ''' AG 19/03/2013 - remove parameters pCurrentAnalyzerID and pCurrentWorkSessionID
        '''                 add parameters pTestMappingDS, pConfigMappingDS, pResultsDS, pResultAlarmsDS
        ''' AG 21/03/2013 - messageID is generated by wrapper
        ''' SG 11/04/2013 - Create an auxiliary ResultsDS and inform all affected results - Call method UpdateLISMessageID
        ''' Modified by: DL 25/04/2013 Create an auxiliary HisWSResultsDS and inform all affected results and Call method UpdateLISMessageID in Hisresultsdelegate
        ''' AG 29/09/2014 - BA-1440 part1 - After send message to LIS: mark results in message as SENDING and MsgID informed // results not included in message (not mapped) as NOTSENT and MsgID = ""
        '''                 BA-1440 return the number of results NOTSENT
        ''' </remarks>
        Public Function UploadOrdersResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pToUploadDS As ExecutionsDS, _
                                            ByVal pHistoricalFlag As Boolean, _
                                            ByVal pTestMappingDS As AllTestsByTypeDS, _
                                            ByVal pConfigMappingDS As LISMappingsDS, _
                                            Optional ByVal pResultsDS As ResultsDS = Nothing, _
                                            Optional ByVal pResultAlarmsDS As ResultsDS = Nothing, _
                                            Optional ByVal pHistDataDS As HisWSResultsDS = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495
                Dim StartTime As DateTime = Now
                Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Get the messageID
                        Dim myUtils As New Utilities
                        resultData = myUtils.GetNewGUID
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim msgId As String = CType(resultData.SetDatos, String)

                            'Get the xml message for manual orders
                            Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                            resultData = translator.GetOrdersResults(dbConnection, msgId, pToUploadDS, pHistoricalFlag, _
                                                                     pTestMappingDS, pConfigMappingDS, _
                                                                     pResultsDS, pResultAlarmsDS, pHistDataDS)


                            'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            '    Dim myCommand As String = String.Empty
                            '    myCommand = CType(resultData.SetDatos, String)

                            '    Debug.Print("Send LIS Message: " & msgId & " - " & Now.ToString("HH:mm:ss:fff")) 'SG 15/04/2012 - send LIS Message
                            '    'Send command to ES library
                            '    Me.driverManager.SendCommand(myCommand)

                            'SGM 27/06/2013 - in case of no service tag is created, not to send anything and leave the message as NOTSENT
                            Dim myExportStatus As String = "NOTSENT"
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myCommand As String = String.Empty
                                myCommand = CType(resultData.SetDatos, String)

                                Debug.Print("Send LIS Message: " & msgId & " - " & Now.ToString("HH:mm:ss:fff")) 'SG 15/04/2012 - send LIS Message

                                'AG 02/01/2014 - BT #1433 add new log traces (v211 patch2)
                                'Dim myLogAcciones As New ApplicationLogManager()    ' XB 12/02/2014 - Task #1495
                                myLogAcciones.CreateLogActivity("Send message uploading results: " & msgId, "ESWrapper.UploadOrdersResults", EventLogEntryType.Information, False)

                                'Send command to ES library
                                Me.driverManager.SendCommand(myCommand)

                                myExportStatus = "SENDING"

                                'AG 07/03/2014 - #1533 increment the number of upload results pending to receive delivered/undelived/unrespon notification
                                Dim lockThis As New Object
                                SyncLock lockThis
                                    uploadResultsMessagesPendingNotificationAttribute += 1
                                End SyncLock
                                'AG 07/03/2014 - #1533
                            End If


                            If myExportStatus = "NOTSENT" Then
                                msgId = String.Empty
                            End If
                            Dim countNotSentResults As Integer = 0 'AG 30/09/2014 - BA-1440


                            'Updates the relationship between xml messageID and his results (order test, rerun)
                            'Mark results in toUploadDS as SENDING/NOTSENT
                            'SGM 10/04/2013
                            Dim myResultsSendingDS As New ResultsDS
                            If Not pHistoricalFlag Then
                                'For Each dr As ResultsDS.vwksResultsRow In pResultsDS.vwksResults
                                For Each dr As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions
                                    Dim myRow As ResultsDS.twksResultsRow = myResultsSendingDS.twksResults.NewtwksResultsRow
                                    With myRow
                                        .BeginEdit()
                                        .OrderTestID = dr.OrderTestID
                                        .RerunNumber = dr.RerunNumber

                                        'AG 29/09/2014 - BA-1440
                                        '.LISMessageID = msgId
                                        '.ExportStatus = myExportStatus 'SGM 27/06/2013
                                        If myExportStatus = "NOTSENT" Then 'Any message sent (all results will be mark NOTSENT with empty LISmessageID
                                            .ExportStatus = myExportStatus 'SGM 27/06/2013
                                            .LISMessageID = String.Empty
                                        Else
                                            'MyExportStatus is SENDING and msgID informed
                                            'Correction, when message is sent mark as SENDING with LISMessageID informed only those results included in message
                                            .ExportStatus = IIf(dr.LISMappingError, "NOTSENT".ToString, "SENDING".ToString)
                                            .LISMessageID = IIf(dr.LISMappingError, String.Empty, msgId.ToString)
                                        End If
                                        'AG 29/09/2014 - BA-1440

                                        .EndEdit()
                                    End With
                                    myResultsSendingDS.twksResults.AddtwksResultsRow(myRow)
                                Next
                                myResultsSendingDS.AcceptChanges()

                                'AG 30/09/2014 - BA-1440 count the results not sent
                                countNotSentResults = (From a As ResultsDS.twksResultsRow In myResultsSendingDS.twksResults Where a.ExportStatus = "NOTSENT" Select a).Count

                                Dim myResultsDelegate As New ResultsDelegate
                                resultData = myResultsDelegate.UpdateLISMessageID(Nothing, myResultsSendingDS)

                            Else
                                'DL 25/04/2013. BEGIN
                                Dim myHISWSResultsDS As New HisWSResultsDS
                                For Each myWSExecutionRow As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions
                                    Dim myHisResultRow As HisWSResultsDS.thisWSResultsRow = myHISWSResultsDS.thisWSResults.NewthisWSResultsRow
                                    With myHisResultRow
                                        .BeginEdit()
                                        .HistOrderTestID = myWSExecutionRow.OrderTestID

                                        'AG 29/09/2014 - BA-1440
                                        '.LISMessageID = msgId
                                        '.ExportStatus = myExportStatus 'SGM 27/06/2013
                                        If myExportStatus = "NOTSENT" Then 'Any message sent (all results will be mark NOTSENT with empty LISmessageID
                                            .ExportStatus = myExportStatus 'SGM 27/06/2013
                                            .LISMessageID = String.Empty
                                        Else
                                            'MyExportStatus is SENDING and msgID informed
                                            'Correction, when message is sent mark as SENDING with LISMessageID informed only those results included in message
                                            .ExportStatus = IIf(myWSExecutionRow.LISMappingError, "NOTSENT".ToString, "SENDING".ToString)
                                            .LISMessageID = IIf(myWSExecutionRow.LISMappingError, String.Empty, msgId.ToString)
                                        End If
                                        'AG 29/09/2014 - BA-1440


                                        .EndEdit()
                                    End With
                                    myHISWSResultsDS.thisWSResults.AddthisWSResultsRow(myHisResultRow)
                                Next
                                myResultsSendingDS.AcceptChanges()

                                'AG 30/09/2014 - BA-1440 count the results not sent
                                countNotSentResults = (From a As HisWSResultsDS.thisWSResultsRow In myHISWSResultsDS.thisWSResults Where a.ExportStatus = "NOTSENT" Select a).Count

                                Dim myHisResultsDelegate As New HisWSResultsDelegate
                                resultData = myHisResultsDelegate.UpdateLISMessageID(Nothing, myHISWSResultsDS)
                                'DL 25/04/2013. END
                            End If
                            'end SGM 10/04/2013

                            resultData.SetDatos = countNotSentResults 'AG 30/09/2014 - BA-1440

                        End If
                    End If

                End If
                'End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495 
                myLogAcciones.CreateLogActivity("UploadOrdersResults Method: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "ESWrapper.UploadOrdersResults", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495 

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.UploadOrdersResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Clears the internal queue that keeps the list of specimens asked to LIS but not responded
        ''' </summary>
        ''' <remarks>AG 22/07/2013</remarks>
        Public Sub ClearQueueOfSpecimenNotResponded()
            Dim resultData As GlobalDataTO = Nothing
            Try
                Dim lockThis As New Object
                SyncLock lockThis
                    specimensWithNoResponseAttribute.Clear()
                End SyncLock
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.ClearQueueOfSpecimenNotResponded", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Call every time a new orders download message is received this method search for all specimens in message and 
        ''' remove them from the queue of specimens asked but not responded
        ''' </summary>
        ''' <param name="pXmlDoc"></param>
        ''' <returns>GlobalDataTo (list of String)</returns>
        ''' <remarks>AG 23/07/2013</remarks>
        Public Function UpdateSpecimensNotResponded(ByVal pXmlDoc As XmlDocument) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim auxReceivedSpecimenList As New List(Of String)
                Dim myXmlDlg As New xmlMessagesDelegate
                resultData = myXmlDlg.ExtractSpecimensFromMessage(pXmlDoc)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    auxReceivedSpecimenList = DirectCast(resultData.SetDatos, List(Of String))
                End If

                If Not resultData.HasError Then
                    Dim lockThis As New Object
                    SyncLock lockThis
                        For Each item As String In auxReceivedSpecimenList
                            If specimensWithNoResponseAttribute.Contains(item) Then
                                specimensWithNoResponseAttribute.Remove(item)
                            End If
                        Next
                        resultData.SetDatos = specimensWithNoResponseAttribute
                    End SyncLock
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.UpdateSpecimensNotResponded", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Decrements the number of results messages pending receive the DELIVERED/UNDELIVERED/UNRESPONDED notification
        ''' Finally returns the current value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>AG 07/03/2014 - #1533</remarks>
        Public Function DecrementUploadMessagesWithOutNotification() As Integer
            Dim value As Integer = 0
            Try
                Dim lockThis As New Object
                SyncLock lockThis
                    If uploadResultsMessagesPendingNotificationAttribute > 0 Then
                        uploadResultsMessagesPendingNotificationAttribute -= 1
                        value = uploadResultsMessagesPendingNotificationAttribute
                    End If
                End SyncLock
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.DecrementUploadMessagesWithOutNotification", EventLogEntryType.Error, False)
            End Try
            Return value
        End Function


        ''' <summary>
        ''' Resets the number of results messages pending receive the DELIVERED/UNDELIVERED/UNRESPONDED notification
        ''' </summary>
        ''' <remarks>AG 25/03/2014 - #1533</remarks>
        Public Sub ResetUploadMessagesWithOutNotification()
            Try
                Dim lockThis As New Object
                SyncLock lockThis
                    uploadResultsMessagesPendingNotificationAttribute = 0
                End SyncLock
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.ResetUploadMessagesWithOutNotification", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Private methods and event fires"

        ''' <summary>
        ''' Private method for message storage establishment. It has to be called after channel creation
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pMaxSendMessages"></param>
        ''' <param name="pSendFolder"></param>
        ''' <param name="pMaxReceivMessages"></param>
        ''' <param name="pReceiveFolder"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Creation by AG 22/02/2013
        ''' Modified by AG 06/03/2013 - add parameter pMaxRecivMessages (after meeting with J Orozco 05/03/2013)
        '''             XB 18/03/2013 - folder information becomes as a parameter instead of configsettings
        ''' </remarks>
        Private Function SetMessageStorage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMaxSendMessages As Integer, _
                                           ByVal pSendFolder As String, ByVal pMaxReceivMessages As Integer, ByVal pReceiveFolder As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim maxSendMessages As Integer = pMaxSendMessages
                        Dim maxReceivMessages As Integer = pMaxReceivMessages

                        'Get the required information from database configuration: MaxMessages, Send Folder, Receive Folder
                        'if not found in database use the default values in parameters
                        Dim userSettings As New UserSettingsDelegate
                        resultData = userSettings.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_STORAGE_TRANS_MAX_MSG.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            maxSendMessages = CType(resultData.SetDatos, Integer)
                        End If

                        resultData = userSettings.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_STORAGE_RECEPTION_MAX_MSG.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            maxReceivMessages = CType(resultData.SetDatos, Integer)
                        End If

                        'Get the xml message
                        Dim translator As New ESxmlTranslator(MyClass.ChannelID, MyClass.AnalyzerModel, MyClass.AnalyzerSerialNumber) 'SGM 26/02/2013 - inform properties
                        resultData = translator.GetMessageStorage(dbConnection, maxSendMessages, pSendFolder, maxReceivMessages, pReceiveFolder)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myCommand As String = String.Empty
                            myCommand = CType(resultData.SetDatos, String)

                            'Send command to ES library
                            Me.driverManager.SendCommand(myCommand)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.SetMessageStorage", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Captures the incoming ES library events (LIS -> ES -> BAx00 application)
        ''' 
        ''' </summary>
        ''' <param name="channelName"></param>
        ''' <param name="priority"></param>
        ''' <param name="notification"></param>
        ''' <remarks>
        ''' Creation AG 22/02/2013
        ''' </remarks>
        Private Sub OnNotificationEventHandler(channelName As String, priority As Integer, notification As String)
            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                Dim myLogAcciones As New ApplicationLogManager()
                '' pending to comment ! only for test !!!
                'myLogAcciones.CreateLogActivity("LIS reception event - ", "ESWrapper.OnNotificationEventHandler", EventLogEntryType.Information, False)
                '' pending to comment ! only for test !!!
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.LoadXml(notification)

                Dim xmlNotification As XmlNode = xmlDoc.DocumentElement
                Dim commandType As String = xmlHelper.QueryStringValue(xmlNotification, "@type")

                Dim isNotificationFlag As Boolean = False
                Select Case (commandType)
                    Case "message"
                        'xml tree
                        '<header><metadata><container><action>....

                        'action value = 'program' is a Query Response FireNotification
                        'action value = 'request' is a Order Download FireMessage

                        'Use xmlDoc or xmlNotification
                        If xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:header/udc:metadata/udc:container/udc:action") = "request" Then
                            FireMessage(channelName, priority, xmlNotification.OwnerDocument)
                        Else
                            FireNotification(channelName, priority, xmlNotification.OwnerDocument)
                            isNotificationFlag = True
                        End If

                    Case "controlInformation"
                        'Types: Channel status, Delivered notification, Undelivered notification, Unresponded notification
                        '       Get pending messages response, delete message response, delete all messages response, storage notification

                        FireNotification(channelName, priority, xmlNotification.OwnerDocument)
                        isNotificationFlag = True

                    Case "directive"
                    Case Else
                        'AG 22/02/2013 - This comment is from SysteLab. I do not remove it!
                        'TODO
                        'Directives are currently used by LAS drivers. Still not supported
                End Select

                If isNotificationFlag Then
                    ' XB 06/03/2014 - i better catch the notification fired on IAx00MainMDI.SetLISLedStatusColor
                    ''*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    'myLogAcciones.CreateLogActivity("LIS reception event - notification fired: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "ESWrapper.OnNotificationEventHandler", EventLogEntryType.Information, False)
                    ''*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    ' XB 06/03/2014
                Else
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    myLogAcciones.CreateLogActivity("LIS reception event - message fired: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "ESWrapper.OnNotificationEventHandler", EventLogEntryType.Information, False)
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESWrapper.OnNotificationEventHandler", EventLogEntryType.Error, False)
            End Try

        End Sub


        ''' <summary>
        ''' Saves the order download xml message (with status PENDING) into database and then fires the new message event
        ''' Try ... Catch not added for speed improvements
        ''' </summary>
        ''' <param name="channelName"></param>
        ''' <param name="priority"></param>
        ''' <param name="xmlMessage"></param>
        ''' <remarks></remarks>
        Private Sub FireMessage(channelName As String, priority As Integer, xmlMessage As XmlDocument)
            Dim resultData As New GlobalDataTO

            'Save the xml into database
            Dim myUtils As New Utilities
            resultData = myUtils.GetNewGUID

            '' pending to comment ! only for test !!!
            'Dim myLogAcciones As New ApplicationLogManager()
            'myLogAcciones.CreateLogActivity("LIS reception event - message fired: ", "ESWrapper.FireMessage", EventLogEntryType.Information, False)
            '' pending to comment ! only for test !!!

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                Dim myMsgId As String = CType(resultData.SetDatos, String)
                Dim xmlMessageDgt As New xmlMessagesDelegate
                resultData = xmlMessageDgt.AddMessage(Nothing, myMsgId, xmlMessage, "PENDING")

                '' pending to comment ! only for test !!!
                'myLogAcciones.CreateLogActivity("LIS reception event - message saved: ", "ESWrapper.FireMessage", EventLogEntryType.Information, False)
                '' pending to comment ! only for test !!!

                'AG 08/03/2013 - send the complete order acceptance message
                If Not resultData.HasError Then
                    resultData = AcceptCompleteOrder(Nothing)

                    '' pending to comment ! only for test !!!
                    'myLogAcciones.CreateLogActivity("LIS reception event - message answered: ", "ESWrapper.FireMessage", EventLogEntryType.Information, False)
                    '' pending to comment ! only for test !!!
                End If
                'AG 08/03/2013

            End If

            If (Not resultData.HasError) AndAlso (OnLISMessageEvent IsNot Nothing) Then
                'We have at least one subscriber
                RaiseEvent OnLISMessage(channelName, priority, xmlMessage)
            End If

        End Sub

        ''' <summary>
        ''' Fires new notification event (control information or query response)
        ''' Try ... Catch not added for speed improvements
        ''' </summary>
        ''' <param name="channelName"></param>
        ''' <param name="priority"></param>
        ''' <param name="xmlMessage"></param>
        ''' <remarks></remarks>
        Private Sub FireNotification(channelName As String, priority As Integer, xmlMessage As XmlDocument)

            If (OnLISNotificationEvent IsNot Nothing) Then
                'We have at least one subscriber
                RaiseEvent OnLISNotification(channelName, priority, xmlMessage)
            End If

        End Sub

#End Region



    End Class

End Namespace
