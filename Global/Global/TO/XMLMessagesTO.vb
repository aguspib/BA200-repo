Option Explicit On
Option Strict On

Imports System.Xml

Public Class XMLMessagesTO

    'SA 31/03/2014
    'BT #1564 - Added the Message Date and Time (Attribute and Property) to identify old messages and to not confuse them with Rerun requests

#Region "Attributes"
    Private MessageIDAttribute As String
    Private XMLMessageAttribute As XmlDocument
    Private StatusAttribute As String
    Private MsgDateTimeAttribute As Date
#End Region

#Region "Properties"
    Public Property MessageID() As String
        Get
            Return MessageIDAttribute
        End Get
        Set(ByVal value As String)
            MessageIDAttribute = value
        End Set
    End Property

    Public Property XMLMessage() As XmlDocument
        Get
            Return XMLMessageAttribute
        End Get
        Set(ByVal value As XmlDocument)
            XMLMessageAttribute = value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return StatusAttribute
        End Get
        Set(ByVal value As String)
            StatusAttribute = value
        End Set
    End Property

    Public Property MsgDateTime() As Date
        Get
            Return MsgDateTimeAttribute
        End Get
        Set(value As Date)
            MsgDateTimeAttribute = value
        End Set
    End Property
#End Region

End Class
