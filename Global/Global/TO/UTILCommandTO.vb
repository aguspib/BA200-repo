Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Global.TO

    Public Class UTILCommandTO


#Region "Attributes"
        Private ActionTypeAttr As UTILInstructionTypes
        Private TanksActionTypeAttr As UTILIntermediateTanksTestActions
        Private CollisionTestActionTypeAttr As UTILCollisionTestActions
        Private SaveSerialActionAttr As UTILSaveSerialNumberActions
        Private SerialNumberToSaveAttr As String
#End Region

#Region "Properties"
        Public Property ActionType() As UTILInstructionTypes
            Get
                Return ActionTypeAttr
            End Get
            Set(ByVal value As UTILInstructionTypes)
                ActionTypeAttr = value
            End Set
        End Property

        Public Property TanksActionType() As UTILIntermediateTanksTestActions
            Get
                Return TanksActionTypeAttr
            End Get
            Set(ByVal value As UTILIntermediateTanksTestActions)
                TanksActionTypeAttr = value
            End Set
        End Property

        Public Property CollisionTestActionType() As UTILCollisionTestActions
            Get
                Return CollisionTestActionTypeAttr
            End Get
            Set(ByVal value As UTILCollisionTestActions)
                CollisionTestActionTypeAttr = value
            End Set
        End Property

        Public Property SaveSerialAction() As UTILSaveSerialNumberActions
            Get
                Return SaveSerialActionAttr
            End Get
            Set(ByVal value As UTILSaveSerialNumberActions)
                SaveSerialActionAttr = value
            End Set
        End Property

        Public Property SerialNumberToSave() As String
            Get
                Return SerialNumberToSaveAttr
            End Get
            Set(ByVal value As String)
                SerialNumberToSaveAttr = value
            End Set
        End Property

#End Region

#Region "Constructor"
        Public Sub New()
            ActionTypeAttr = UTILInstructionTypes.None
            TanksActionTypeAttr = UTILIntermediateTanksTestActions.NothingToDo
            CollisionTestActionTypeAttr = UTILCollisionTestActions.Disable
            SaveSerialActionAttr = UTILSaveSerialNumberActions.NothingToDo
            SerialNumberToSaveAttr = ""
        End Sub
#End Region

    End Class

End Namespace