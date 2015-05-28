Option Explicit On
Option Strict On
Option Infer On
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Public Class TechniqueToSend
        Public Property ReagentID As Integer

        Public Property NumReplicates As Integer

        Public Sub New()
            ReagentID = 0
            NumReplicates = 0
        End Sub


    End Class
End Namespace