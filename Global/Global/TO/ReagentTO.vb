
Option Explicit On
Option Strict On

#Region "Libraries Used by this Class"
Imports System
#End Region

Namespace Biosystems.Ax00.Global.TO

    '''AUTOMATICALLY GENERATED CODE  

    Public Class ReagentTO


#Region "Attributes"


        Private reagentIDAttribute As String

        Private reagentNameAttribute As String

#End Region

#Region "Properties"

        Public Property ReagentID() As String
            Get
                Return reagentIDAttribute
            End Get

            Set(ByVal Value As String)
                reagentIDAttribute = Value
            End Set

        End Property


        Public Property ReagentName() As String
            Get
                Return reagentNameAttribute
            End Get

            Set(ByVal Value As String)
                reagentNameAttribute = Value
            End Set

        End Property


#End Region

#Region "Constructor"

        Public Sub New()
            ReagentID = ""
            ReagentName = ""

        End Sub

#End Region

#Region "Methods"

        Public Overrides Function ToString() As String
            Return "ReagentID=" & ReagentID _
              & Environment.NewLine & "ReagentName = " & ReagentName

        End Function

#End Region

    End Class

End Namespace


