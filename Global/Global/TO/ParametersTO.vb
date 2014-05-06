Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global.TO

    Public Class ParametersTO

#Region "Attributes"
        Private ParameterIDAttribute As String
        Private ParameterValuesAttribute As String
#End Region

#Region "Properties"

        ''' <summary>
        ''' Parameter Identification
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ParameterID() As String
            Get
                Return ParameterIDAttribute
            End Get
            Set(ByVal value As String)
                ParameterIDAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' List of ParametersValues
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ParameterValues() As String
            Get
                Return ParameterValuesAttribute
            End Get
            Set(ByVal value As String)
                ParameterValuesAttribute = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()
            ParameterIDAttribute = ""
            ParameterValuesAttribute = ""
        End Sub

#End Region

    End Class
End Namespace


