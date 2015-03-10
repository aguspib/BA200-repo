Imports System.Data.SqlClient
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities.Tests.Mock
    Public Class BA200AnalyzerMock
        Inherits AnalyzerManager

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine)
        End Sub

        Public Overrides Function GetCurrentBaseLineID(ByVal pdbConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean) As GlobalDataTO
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
