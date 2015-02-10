
Option Explicit On
Option Strict On

#Region "Libraries Used by this Class"
Imports System
Imports Biosystems.Ax00.Global.GlobalEnumerates

#End Region

Namespace Biosystems.Ax00.Global.TO

    '''AUTOMATICALLY GENERATED CODE  

    Public Class ApplicationInfoSessionTO


#Region "Attributes"

        Private applicationNameAttribute As String
        Private applicationLanguageAttribute As String
        Private userNameAttribute As String
        Private userLevelAttribute As String
        Private applicationVersionAttribute As String
        Private databaseVersionAttribute As String
        Private activateSystemLogAttribute As Boolean
        Private applicationIconPathAttribute As String
        Private maxTestsNumberAttribute As Integer 'TR 29/03/2012
        Private userLevelEnumAttribute As USER_LEVEL 'IT 18/09/2014 #BA-1946
#End Region

#Region "Properties"
        Public Property ApplicationIconPath() As String
            Get
                Return applicationIconPathAttribute
            End Get
            Set(ByVal value As String)
                applicationIconPathAttribute = value
            End Set
        End Property


        Public Property ApplicationName() As String
            Get
                Return applicationNameAttribute
            End Get
            Set(ByVal Value As String)
                applicationNameAttribute = Value
            End Set
        End Property


        Public Property ApplicationLanguage() As String
            Get
                Return applicationLanguageAttribute
            End Get

            Set(ByVal Value As String)
                applicationLanguageAttribute = Value
            End Set

        End Property


        Public Property UserName() As String
            Get
                Return userNameAttribute
            End Get

            Set(ByVal Value As String)
                userNameAttribute = Value
            End Set

        End Property


        Public Property UserLevel() As String
            Get
                Return userLevelAttribute
            End Get

            Set(ByVal Value As String)
                userLevelAttribute = Value
            End Set

        End Property

        'IT 18/09/2014 #BA-1946 - INI
        Public Property UserLevelEnum() As USER_LEVEL
            Get
                Return userLevelEnumAttribute
            End Get

            Set(ByVal Value As USER_LEVEL)
                userLevelEnumAttribute = Value
            End Set

        End Property
        'IT 18/09/2014 #BA-1946 - FIN

        Public Property ApplicationVersion() As String
            Get
                Return applicationVersionAttribute
            End Get

            Set(ByVal Value As String)
                applicationVersionAttribute = Value
            End Set

        End Property


        Public Property DatabaseVersion() As String
            Get
                Return databaseVersionAttribute
            End Get

            Set(ByVal Value As String)
                databaseVersionAttribute = Value
            End Set

        End Property


        Public Property ActivateSystemLog() As Boolean
            Get
                Return activateSystemLogAttribute
            End Get

            Set(ByVal Value As Boolean)
                activateSystemLogAttribute = Value
            End Set

        End Property

        Public Property MaxTestsNumber() As Integer
            Get
                Return maxTestsNumberAttribute
            End Get
            Set(ByVal value As Integer)
                maxTestsNumberAttribute = value
            End Set
        End Property


#End Region

#Region "Constructor"

        Public Sub New()
            ApplicationName = ""
            ApplicationLanguage = "ENG"
            UserName = "Admin"
            UserLevel = ""
            ApplicationVersion = ""
            DatabaseVersion = ""
            ActivateSystemLog = Nothing
            MaxTestsNumber = -1
        End Sub

#End Region

#Region "Methods"

        Public Overrides Function ToString() As String
            Return "ApplicationName=" & ApplicationName _
              & Environment.NewLine & "ApplicationLanguage = " & ApplicationLanguage _
              & Environment.NewLine & "UserName = " & UserName _
              & Environment.NewLine & "UserLevel = " & UserLevel _
              & Environment.NewLine & "ApplicationVersion = " & ApplicationVersion _
              & Environment.NewLine & "DatabaseVersion = " & DatabaseVersion _
              & Environment.NewLine & "ActivateSystemLog = " & ActivateSystemLog _
               & Environment.NewLine & "MaxTestsNumber = " & ActivateSystemLog

        End Function

#End Region

    End Class

End Namespace


