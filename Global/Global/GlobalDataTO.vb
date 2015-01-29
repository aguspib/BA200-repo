Option Strict On
Option Explicit On

Namespace Biosystems.Ax00.Global
    'Definition of the global TO used for Data Access 

    Public Class TypedGlobalDataTo(Of T)

#Region "Properties"
        'Get/Set the number of records affected for an SQL command
        Public Property AffectedRecords As Integer

        'Get/Set if an error has happened when an SQL Sentence is executed
        Public Property HasError As Boolean

        'When the execution of an SQL sentence generates an Error, 
        'Get/Set the ErrorCode (it can be an application error (controlled)
        'or a system/database error (uncontrolled)
        Public Property ErrorCode As String

        'When the execution of an SQL sentence generates an Error, 
        'Get/Set the ErrorCode (it can be an application error (controlled)
        'or a system/database error (uncontrolled)
        Public Property ErrorMessage As String

        'Get/Set the dataset containing the data to insert/update or delete
        Public Overridable Property SetDatos As T

        'Get/Set the value of the UserLevel
        Public Property SetUserLevel As String

#End Region

#Region "Methods"
        ''' <summary>
        ''' This function creates a clone of this TypedGlobalDataTo as an older untyped GlobalDataTo. Use this for compatibility with older code.
        ''' </summary>
        ''' <returns>A GlobalDataTo.</returns>
        ''' <remarks></remarks>
        Overridable Function CloneUntyped() As GlobalDataTO
            Dim globalDataTo As New GlobalDataTO()
            globalDataTo.AffectedRecords = AffectedRecords
            globalDataTo.ErrorCode = ErrorCode
            globalDataTo.ErrorMessage = ErrorMessage
            globalDataTo.HasError = HasError
            globalDataTo.SetDatos = TryCast(SetDatos, Object)
            globalDataTo.SetUserLevel = SetUserLevel
            Return globalDataTo
        End Function

#End Region

    End Class

    Public NotInheritable Class GlobalDataTO
        Inherits TypedGlobalDataTo(Of Object)
    End Class

End Namespace