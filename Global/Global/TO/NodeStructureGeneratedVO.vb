
    Option Explicit On
    Option Strict On
  
    #Region "Libraries Used by this Class"
    Imports System
    #End Region
  
Namespace Biosystems.Ax00.Global.TO

    '''AUTOMATICALLY GENERATED CODE  

    Public Class NodeStructureTO



#Region "Attributes"


        Private tubeContentAttribute As String

        Private elementTypeAttribute As String

        Private elementDescAttribute As String

        Private positionIDAttribute As Integer

        Private identificationAttribute As String

        Private elementNumberAttribute As Integer

        Private lastPositionIDAttribute As String

        Private sTATFlagAttribute As Boolean

        Public status As String

#End Region

#Region "Properties"

        Public Property TubeContent() As String
            Get
                Return tubeContentAttribute
            End Get

            Set(ByVal Value As String)
                tubeContentAttribute = Value
            End Set

        End Property


        Public Property ElementType() As String
            Get
                Return elementTypeAttribute
            End Get

            Set(ByVal Value As String)
                elementTypeAttribute = Value
            End Set

        End Property


        Public Property ElementDesc() As String
            Get
                Return elementDescAttribute
            End Get

            Set(ByVal Value As String)
                elementDescAttribute = Value
            End Set

        End Property


        Public Property PositionID() As Integer
            Get
                Return positionIDAttribute
            End Get

            Set(ByVal Value As Integer)
                positionIDAttribute = Value
            End Set

        End Property


        Public Property Identification() As String
            Get
                Return identificationAttribute
            End Get

            Set(ByVal Value As String)
                identificationAttribute = Value
            End Set

        End Property


        Public Property ElementNumber() As Integer
            Get
                Return elementNumberAttribute
            End Get

            Set(ByVal Value As Integer)
                elementNumberAttribute = Value
            End Set

        End Property


        Public Property LastPositionID() As String
            Get
                Return lastPositionIDAttribute
            End Get

            Set(ByVal Value As String)
                lastPositionIDAttribute = Value
            End Set

        End Property


        Public Property STATFlag() As Boolean
            Get
                Return sTATFlagAttribute
            End Get

            Set(ByVal Value As Boolean)
                sTATFlagAttribute = Value
            End Set

        End Property


#End Region

#Region "Constructor"

        Public Sub New()
            TubeContent = ""
            ElementType = ""
            ElementDesc = ""
            PositionID = 0
            Identification = ""
            ElementNumber = 0
            LastPositionID = ""
            STATFlag = Nothing

        End Sub

#End Region

#Region "Methods"

        Public Overrides Function ToString() As String
            Return "TubeContent=" & TubeContent _
              & Environment.NewLine & "ElementType = " & ElementType _
              & Environment.NewLine & "ElementDesc = " & ElementDesc _
              & Environment.NewLine & "PositionID = " & PositionID _
              & Environment.NewLine & "Identification = " & Identification _
              & Environment.NewLine & "ElementNumber = " & ElementNumber _
              & Environment.NewLine & "LastPositionID = " & LastPositionID _
              & Environment.NewLine & "STATFlag = " & STATFlag

        End Function

#End Region

    End Class

End Namespace




    