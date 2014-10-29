Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerEntity
        Implements IAnalyzerEntity

        Private WithEvents _baseLine As IBaseLineEntity
        Private WithEvents _iseAnalyzer As IISEAnalyzerEntity

        Property BaseLine As IBaseLineEntity Implements IAnalyzerEntity.BaseLine
            Get
                Return _baseLine
            End Get
            Set(value As IBaseLineEntity)
                _baseLine = value
            End Set
        End Property

        Property ISEAnalyzer As IISEAnalyzerEntity Implements IAnalyzerEntity.ISEAnalyzer
            Get
                Return _iseAnalyzer
            End Get
            Set(value As IISEAnalyzerEntity)
                _iseAnalyzer = value
            End Set
        End Property


        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            Me.New(assemblyName, analyzerModel)
            _baseLine = baseLine
        End Sub


#Region "Abstract methods"
        Public MustOverride Function GetCurrentBaseLineID(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                          ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean) As GlobalDataTO

#End Region
    End Class

End Namespace
