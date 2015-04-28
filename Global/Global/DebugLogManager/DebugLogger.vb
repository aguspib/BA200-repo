Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Concurrent

Public Class DebugMessage
    Private ReadOnly _message As String
    Private ReadOnly _prefix As String
    Private ReadOnly _dateLogged As DateTime

    Public ReadOnly Property Message() As String
        Get
            Return _message
        End Get
    End Property

    Public ReadOnly Property Prefix() As String
        Get
            Return _prefix
        End Get
    End Property

    Public ReadOnly Property DateLogged() As DateTime
        Get
            Return _dateLogged
        End Get
    End Property

    Public Sub New(ByVal message As String, Optional ByVal prefix As String = Nothing)
        _message = message
        _prefix = prefix
        _dateLogged = DateTime.Now
    End Sub
End Class

Public Class DebugLogger

#If DEBUG Then
#Region "Attributes"

    Private Const Extension = ".log"
    Private Const FolderName = "DebugLog\"

    Private ReadOnly _myThread As New Threading.Thread(AddressOf ActivatePassiveWaiting)
    Private ReadOnly _myListener As New Threading.AutoResetEvent(True)
    Private ReadOnly _message As New ConcurrentQueue(Of DebugMessage)

    Public Property DieFlag As Boolean = False

#End Region
#Region "Private Methods"
    Private Sub ActivatePassiveWaiting()
        While True
            'We're waiting:
            _myListener.WaitOne()

            'We've been woken up:
            If DieFlag Then
                Exit Sub
            Else
                ProcessQueue()
            End If
        End While
    End Sub


    Private Sub ProcessQueue()
        While _message.IsEmpty = False
            Dim item As DebugMessage = Nothing
            If _message.TryDequeue(item) AndAlso Not item Is Nothing Then
                WriteLog(item)
            End If
        End While
    End Sub


    Private Shared Sub WriteLog(ByRef messageToWrite As DebugMessage)

        Dim fullFile As String = GetFileNameAndPath(messageToWrite)
        Dim fileFs As New FileStream(fullFile, FileMode.Append, FileAccess.Write)

        If Not fileFs Is Nothing And fileFs.CanWrite Then
            Dim objStream As New StreamWriter(fileFs)
            objStream.WriteLine("[" & messageToWrite.DateLogged.ToString & "]: " + messageToWrite.Message)
            objStream.Close()
            fileFs.Close()
        End If
    End Sub

    Private Shared Function GetFileNameAndPath(ByRef messageToWrite As DebugMessage) As String
        Dim solutionPath = Path.GetFullPath(Application.StartupPath & "\..\..\..\..\")
        If Not Directory.Exists(solutionPath + FolderName) Then
            Directory.CreateDirectory(solutionPath + FolderName)
        End If
        Return solutionPath & FolderName & If(Not messageToWrite.Prefix Is Nothing, messageToWrite.Prefix & "_", "") & messageToWrite.DateLogged.ToString("yyyyMMdd") & Extension
    End Function
#End Region
#End If

#Region "Public Methods"

    ''' <summary>
    ''' Ctor, only active in debug mode
    ''' </summary>
    Sub New()
#If DEBUG Then
        _myThread.IsBackground = True
        _myThread.Start()
#End If
    End Sub

    ''' <summary>
    ''' Add a new log, only active in debug mode
    ''' </summary>
    Public Sub AddLog(message As String, Optional ByVal prefix As String = Nothing)
#If DEBUG Then
        _message.Enqueue(New DebugMessage(message, prefix))
        Try
            _myListener.Set()
        Catch : End Try
#End If
    End Sub

    ''' <summary>
    ''' End the thread, only active in debug mode
    ''' </summary>
    Public Sub Kill()
#If DEBUG Then
        DieFlag = True
        _myListener.Set()
#End If
    End Sub
#End Region

End Class




