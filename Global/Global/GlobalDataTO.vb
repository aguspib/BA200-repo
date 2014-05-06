Option Strict On
Option Explicit On

Namespace Biosystems.Ax00.Global
    'Definition of the global TO used for Data Access 
    Public Class GlobalDataTO

#Region "Attributes"
        Private affectedRecordsAttribute As Integer
        Private hasErrrorAttribute As Boolean
        Private errorCodeAttribute As String
        Private errorMessageAttribute As String
        Private setDatosAttribute As Object
        Private setUserLevelAttribute As Object
#End Region

#Region "Properties"
        'Get/Set the number of records affected for an SQL command
        Public Property AffectedRecords() As Integer
            Get
                Return affectedRecordsAttribute
            End Get
            Set(ByVal value As Integer)
                affectedRecordsAttribute = value
            End Set
        End Property

        'Get/Set if an error has happened when an SQL Sentence is executed
        Public Property HasError() As Boolean
            Get
                Return hasErrrorAttribute
            End Get
            Set(ByVal value As Boolean)
                hasErrrorAttribute = value
            End Set
        End Property

        'When the execution of an SQL sentence generates an Error, 
        'Get/Set the ErrorCode (it can be an application error (controlled)
        'or a system/database error (uncontrolled)
        Public Property ErrorCode() As String
            Get
                Return errorCodeAttribute
            End Get
            Set(ByVal value As String)
                errorCodeAttribute = value
            End Set
        End Property

        'When the execution of an SQL sentence generates an Error, 
        'Get/Set the ErrorCode (it can be an application error (controlled)
        'or a system/database error (uncontrolled)
        Public Property ErrorMessage() As String
            Get
                Return errorMessageAttribute
            End Get
            Set(ByVal value As String)
                errorMessageAttribute = value
            End Set
        End Property

        'Get/Set the dataset containing the data to insert/update or delete
        Public Property SetDatos() As Object
            Get
                Return setDatosAttribute
            End Get
            Set(ByVal value As Object)
                setDatosAttribute = value
            End Set
        End Property

        'Get/Set the value of the UserLevel
        Public Property SetUserLevel() As Object
            Get
                Return setUserLevelAttribute
            End Get
            Set(ByVal value As Object)
                setUserLevelAttribute = value
            End Set
        End Property

#End Region

    End Class
End Namespace