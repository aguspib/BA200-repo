Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IBaseLineEntity

#Region "Properties"

        WriteOnly Property fieldLimits() As FieldLimitsDS
        WriteOnly Property SwParameters() As ParametersDS
        WriteOnly Property Adjustments() As SRVAdjustmentsDS
        Property validALight() As Boolean
        ReadOnly Property exitRunningType() As Integer 'AG 04/06/2012
        ReadOnly Property existsAlightResults() As Boolean

#End Region

#Region "Public Methods"

        Function ControlAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pALineDS As BaseLinesDS) As GlobalDataTO

        Function ControlWellBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pClassInitialization As Boolean, _
                                            ByVal pWellBaseLine As BaseLinesDS) As GlobalDataTO

        Sub ResetWS()

        Function GetLatestBaseLines(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String) As GlobalDataTO

#End Region

#Region "Events definition"

        Event WellReactionsChanges(ByVal pReactionsRotorDS As ReactionsRotorDS)

#End Region

    End Interface

End Namespace

