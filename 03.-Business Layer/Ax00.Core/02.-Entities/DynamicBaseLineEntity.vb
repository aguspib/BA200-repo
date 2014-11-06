Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Entities

    Public Class DynamicBaseLineEntity
        Inherits BaseLineEntity

#Region "Public Methods"

#End Region

#Region "Private Methods"

#End Region

#Region "Overriden methods"
        Public Overrides Function GetCurrentAdjustBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Return MyBase.GetCurrentAdjustBaseLineValuesByType(pDBConnection, pAnalyzerID, BaseLineType.STATIC.ToString())
        End Function
#End Region

    End Class

End Namespace
