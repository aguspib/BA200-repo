Imports System.Data.SqlClient
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Tests.Mock

    Public Class BaseLineEntityMock
        Implements IBaseLineEntity

        Public WriteOnly Property fieldLimits() As FieldLimitsDS Implements IBaseLineEntity.fieldLimits
            Set(ByVal value As FieldLimitsDS)
                Dim fieldLimitsAttribute = value
            End Set
        End Property

        Public WriteOnly Property SwParameters() As ParametersDS Implements IBaseLineEntity.SwParameters
            Set(ByVal value As ParametersDS)
                Dim swParametersAttribute = value
            End Set
        End Property

        Public WriteOnly Property Adjustments() As SRVAdjustmentsDS Implements IBaseLineEntity.Adjustments
            Set(ByVal value As SRVAdjustmentsDS)
                Dim instrumentAdjustments = value
            End Set
        End Property

        Public Property validALight() As Boolean Implements IBaseLineEntity.validALight
            Get                
                Return True
            End Get
            Set(ByVal value As Boolean)
                Dim validAlightAttribute = value
            End Set
        End Property

        Public ReadOnly Property exitRunningType() As Integer Implements IBaseLineEntity.exitRunningType
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property existsAlightResults() As Boolean Implements IBaseLineEntity.existsAlightResults
            Get
                Return True
            End Get

        End Property

        Public Property BaseLineTypeForWellReject() As GlobalEnumerates.BaseLineType Implements IBaseLineEntity.BaseLineTypeForWellReject

        Public Function ControlAdjustBaseLine(ByVal pDBConnection As SqlConnection, ByVal pALineDS As BaseLinesDS) As GlobalDataTO Implements IBaseLineEntity.ControlAdjustBaseLine
            Throw New NotImplementedException()
        End Function

        Public Function ValidateDynamicBaseLinesResults(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO Implements IBaseLineEntity.ValidateDynamicBaseLinesResults
            Throw New NotImplementedException()
        End Function

        Public Function ControlDynamicBaseLine(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pInitialWell As Integer) As GlobalDataTO Implements IBaseLineEntity.ControlDynamicBaseLine
            Throw New NotImplementedException()
        End Function

        Public Function ControlWellBaseLine(ByVal pDBConnection As SqlConnection, ByVal pClassInitialization As Boolean, ByVal pWellBaseLine As BaseLinesDS, ByVal pType As GlobalEnumerates.BaseLineType) As GlobalDataTO Implements IBaseLineEntity.ControlWellBaseLine
            Throw New NotImplementedException()
        End Function

        Public Function GetLatestBaseLines(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String) As GlobalDataTO Implements IBaseLineEntity.GetLatestBaseLines
            Throw New NotImplementedException()
        End Function

        Public Sub ResetWS() Implements IBaseLineEntity.ResetWS
            Throw New NotImplementedException()
        End Sub

        Public Sub Initialize() Implements IBaseLineEntity.Initialize
            Throw New NotImplementedException()
        End Sub

        Public Event WellReactionsChanges(ByVal pReactionsRotorDS As ReactionsRotorDS, ByVal pFromDynamicBaseLineProcessingFlag As Boolean) Implements IBaseLineEntity.WellReactionsChanges
    End Class
End Namespace
