Option Explicit On
Option Strict On


Public Class ScreenTO

#Region "Properties"
    Public Property ScreenID() As String
        Get
            Return ScreenIDAttr
        End Get
        Set(ByVal value As String)
            ScreenIDAttr = value
        End Set
    End Property

    'all the Script ID's related to the Screen
    Public Property FwScriptIDs() As List(Of Integer)
        Get
            Return FwScriptIDsAttr
        End Get
        Set(ByVal value As List(Of Integer))
            FwScriptIDsAttr = value
        End Set
    End Property
#End Region

#Region "Attributes"
    Private ScreenIDAttr As String
    Private FwScriptIDsAttr As List(Of Integer)
#End Region

#Region "Constructor"
    Public Sub New()
        ScreenIDAttr = ""
        Me.FwScriptIDsAttr = New List(Of Integer)
    End Sub
#End Region

#Region "Public methods"
    Public Function Clone() As ScreenTO
        Try
            Dim myNewScreen As New ScreenTO
            With Me
                myNewScreen.FwScriptIDs = .FwScriptIDs
                myNewScreen.ScreenID = .ScreenID
            End With
            Return myNewScreen
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
End Class
