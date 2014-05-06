Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class ApplicationLogTO

#Region "Attributes"
        Private LogDateAttribute As DateTime
        Private LogMessageAttribute As String
        Private LogModuleAttribute As String
        Private LogTypeAttribute As EventLogEntryType
#End Region

#Region "Properties"

        ''' <summary>
        ''' Use to indicate the Date tima of Action.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LogDate() As DateTime
            Get
                Return LogDateAttribute
            End Get
            Set(ByVal value As DateTime)
                LogDateAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Message to be safe at the XML File.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LogMessage() As String
            Get
                Return LogMessageAttribute
            End Get
            Set(ByVal value As String)
                LogMessageAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Method or class where the action occur.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LogModule() As String
            Get
                Return LogModuleAttribute
            End Get
            Set(ByVal value As String)
                LogModuleAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' indicate the log type.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LogType() As EventLogEntryType
            Get
                Return LogTypeAttribute
            End Get
            Set(ByVal value As EventLogEntryType)
                LogTypeAttribute = value
            End Set
        End Property

#End Region

#Region "Constructor"

        ''' <summary>
        ''' Initialize the object with empty values
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            LogDateAttribute = DateTime.MinValue
            LogMessageAttribute = ""
            LogModuleAttribute = ""
            LogTypeAttribute = EventLogEntryType.Information
        End Sub

        ''' <summary>
        ''' Recive the values to initialize the object.
        ''' </summary>
        ''' <param name="MyLogDate">Date</param>
        ''' <param name="MyLogMessage">Message to be safe</param>
        ''' <param name="MyLogModule">Module from action occur</param>
        ''' <param name="MyLogType">Log Type</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal MyLogDate As DateTime, ByVal MyLogMessage As String, ByVal MyLogModule As String, ByVal MyLogType As EventLogEntryType)
            LogDateAttribute = MyLogDate
            LogMessageAttribute = MyLogMessage
            LogModuleAttribute = MyLogModule
            LogTypeAttribute = MyLogType
        End Sub

#End Region

    End Class

End Namespace
