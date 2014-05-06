
Option Explicit On
Option Strict On

#Region "Libraries Used by this Class"
Imports System
#End Region

Namespace Biosystems.Ax00.Global.TO

    '''AUTOMATICALLY GENERATED CODE  

    Public Class SpecialSolutionTO


#Region "Attributes"


        Private specialSolucionCodeAttribute As String

        Private specialSolucionDescriptionAttribute As String

#End Region

#Region "Properties"

        Public Property SpecialSolucionCode() As String
            Get
                Return specialSolucionCodeAttribute
            End Get

            Set(ByVal Value As String)
                specialSolucionCodeAttribute = Value
            End Set

        End Property


        Public Property SpecialSolucionDescription() As String
            Get
                Return specialSolucionDescriptionAttribute
            End Get

            Set(ByVal Value As String)
                specialSolucionDescriptionAttribute = Value
            End Set

        End Property


#End Region

#Region "Constructor"

        Public Sub New()
            SpecialSolucionCode = ""
            SpecialSolucionDescription = ""

        End Sub

#End Region

#Region "Methods"

        Public Overrides Function ToString() As String
            Return "SpecialSolucionCode=" & SpecialSolucionCode _
              & Environment.NewLine & "SpecialSolucionDescription = " & SpecialSolucionDescription

        End Function

#End Region

    End Class

End Namespace


