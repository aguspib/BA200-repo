Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Entities

    Public Class StaticBaseLineEntity
        Inherits BaseLineEntity

#Region "Public Methods"

#End Region

#Region "Private Methods"

#End Region

#Region "Overriden methods"
        Public Overrides Function GetCurrentAdjustBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Return MyBase.GetCurrentAdjustBaseLineValuesByType(pDBConnection, pAnalyzerID, BaseLineTypeForWellReject.ToString) 'AG 11/11/2014 BA-2065
        End Function
#End Region

    End Class

End Namespace

