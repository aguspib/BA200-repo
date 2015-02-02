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
        Public Overrides Sub ResetWS()
            MyBase.InitStructures(True, False) 'Clear well base line parameters on RESET worksession
        End Sub

#End Region

    End Class

End Namespace

