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
        Function GetCompatibleGlobalDataTo() As GlobalDataTO
            Dim GDT As New GlobalDataTO()
            GDT.AffectedRecords = AffectedRecords
            GDT.ErrorCode = ErrorCode
            GDT.ErrorMessage = ErrorMessage
            GDT.HasError = HasError
            GDT.SetDatos = TryCast(SetDatos, Object)
            GDT.SetUserLevel = SetUserLevel
            Return GDT
        End Function
#End Region
    End Class

    Public Class GlobalDataTO
        Inherits TypedGlobalDataTo(Of Object)
    End Class
End Namespace