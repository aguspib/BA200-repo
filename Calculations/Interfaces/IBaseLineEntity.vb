﻿Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IBaseLineEntity

#Region "Properties"

        WriteOnly Property fieldLimits() As FieldLimitsDS
        WriteOnly Property SwParameters() As ParametersDS
        WriteOnly Property Adjustments() As SRVAdjustmentsDS
        Property validALight() As Boolean
        Property validFLight() As Boolean
        ReadOnly Property exitRunningType() As Integer 'AG 04/06/2012
        ReadOnly Property existsAlightResults() As Boolean
        Property BaseLineTypeForWellReject As BaseLineType 'AG 11/11/2014 BA-2065

#End Region

#Region "Public Methods"

        'Treat ALIGHT results
        Function ControlAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pALineDS As BaseLinesDS) As GlobalDataTO

        Function ValidateDynamicBaseLinesResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO 'AG 26/11/2014 BA-2081 (validate the FLIGHT results)
        Function ControlDynamicBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pInitialWell As Integer) As GlobalDataTO 'BA-2065 Treat FLIGHT results (only when validated)

        'Implements the well rejections algorithm
        'AG 14/11/2014 BA-2065 add parameter pType (STATIC or DYNAMIC)
        Function ControlWellBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pClassInitialization As Boolean, _
                                            ByVal pWellBaseLine As BaseLinesDS, ByVal pType As GlobalEnumerates.BaseLineType) As GlobalDataTO

        Function GetLatestBaseLines(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String) As GlobalDataTO

        Sub ResetWS()
        Sub Initialize()

#End Region

#Region "Events definition"

        Event WellReactionsChanges(ByVal pReactionsRotorDS As ReactionsRotorDS, ByVal pFromDynamicBaseLineProcessingFlag As Boolean)

#End Region

    End Interface

End Namespace

