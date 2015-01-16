Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global

Public Class InstructionTO


#Region "Attributes"
    Private InstructionIDAttr As Integer
    Private SequenceAttr As Integer
    Private TimerAttr As Integer
    Private CodeAttr As String
    Private ParamsAttr As String
    Private TextAttr As String
    Private EnableEditionAttr As Boolean    ' XBC 17/11/2010 - There are Scripts not editables because its strong conditional to screen params
#End Region

#Region "Public Properties"

    Public Property InstructionID() As Integer
        Get
            Return InstructionIDAttr
        End Get
        Set(ByVal value As Integer)
            InstructionIDAttr = value
        End Set
    End Property

    Public Property Sequence() As Integer
        Get
            Return SequenceAttr
        End Get
        Set(ByVal value As Integer)
            SequenceAttr = value
        End Set
    End Property

    Public Property Timer() As Integer
        Get
            Return TimerAttr
        End Get
        Set(ByVal value As Integer)
            TimerAttr = value
            BuildText()
        End Set
    End Property

    Public Property Code() As String
        Get
            Return CodeAttr
        End Get
        Set(ByVal value As String)
            CodeAttr = value
            BuildText()
        End Set
    End Property

    Public Property Params() As String
        Get
            Return ParamsAttr
        End Get
        Set(ByVal value As String)
            ParamsAttr = value
            BuildText()
        End Set
    End Property

    Public ReadOnly Property Text() As String
        Get
            Return TextAttr
        End Get
    End Property

    Public Property EnableEdition() As Boolean
        Get
            Return EnableEditionAttr
        End Get
        Set(ByVal value As Boolean)
            EnableEditionAttr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        InstructionIDAttr = 0
        SequenceAttr = 1
        TimerAttr = 1
        CodeAttr = ""
        ParamsAttr = ""
        TextAttr = ""
        EnableEditionAttr = True
    End Sub
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Get a list of the dynamic parameters from the instruction
    ''' </summary>
    ''' <returns>List of dynamic parameters</returns>
    ''' <remarks>
    ''' Created by XBC 18/11/2010
    ''' </remarks>
    Public Function getFwScriptParams() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Dim returnValue As New List(Of String)
        Try
            Dim newStr As String
            Dim valor As String
            Dim k, x As Integer

            If Params.Contains("#") Then
                newStr = Params
                For i As Integer = 0 To Params.Length
                    k = InStr(newStr, "#")

                    If k = 0 Then Exit For

                    newStr = newStr.Substring(k, newStr.Length - k)

                    If newStr.Contains("#") Then
                        x = InStr(newStr, ".")
                        valor = "#" + newStr.Substring(0, x - 1)
                        newStr = newStr.Substring(x, newStr.Length - x)
                    Else
                        x = InStr(newStr, ".")
                        If x = 0 Then
                            valor = "#" + newStr.Substring(0, newStr.Length)
                        Else
                            valor = "#" + newStr.Substring(0, x - 1)
                            newStr = newStr.Substring(x, newStr.Length - x)
                        End If
                    End If
                    returnValue.Add(Trim(valor))
                    i += k
                Next

            End If

            myGlobalDataTO.SetDatos = returnValue

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "InstructionTO.getFwScriptParams", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

    Public Function Clone() As InstructionTO
        Try
            Dim myNewInstruction As New InstructionTO
            With Me
                myNewInstruction.Code = .Code
                myNewInstruction.EnableEdition = .EnableEdition
                myNewInstruction.InstructionID = .InstructionID
                myNewInstruction.Params = .Params
                myNewInstruction.Sequence = .Sequence
                myNewInstruction.Timer = .Timer
            End With
            Return myNewInstruction
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

#Region "Private Methods"
    Private Sub BuildText()
        If TimerAttr <> 0 And CodeAttr <> "" And ParamsAttr <> "" Then
            TextAttr = TimerAttr.ToString.Trim.PadLeft(6, CChar("0")) & ":" & CodeAttr.Trim & ":" & ParamsAttr.Trim
        Else
            TextAttr = ""
        End If
    End Sub
#End Region




End Class
