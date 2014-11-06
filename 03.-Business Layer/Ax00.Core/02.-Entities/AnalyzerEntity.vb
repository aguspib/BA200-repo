Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerEntity
        Implements IAnalyzerEntity

        Private WithEvents _baseLine As IBaseLineEntity
        Private WithEvents _iseAnalyzer As IISEAnalyzerEntity

#Region "Properties"

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

        Property BaseLineTypeForCalculations As BaseLineType Implements IAnalyzerEntity.BaseLineTypeForCalculations
        Property BaseLineTypeForWellReject As BaseLineType Implements IAnalyzerEntity.BaseLineTypeForWellReject

        Property Model As String Implements IAnalyzerEntity.Model
            Get
                Return myAnalyzerModel
            End Get
            Set(value As String)
                myAnalyzerModel = value
            End Set
        End Property

#End Region

        'AG 30/10/2014 BA-2064 comment new code temporally
        'Property Calculations As CalculationsDelegate Implements IAnalyzerEntity.calculations

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            Me.New(assemblyName, analyzerModel)
            _baseLine = baseLine
        End Sub


#Region "Abstract methods"
        ''' <summary>
        ''' Get the urrent (last) base line ID. It must be override because different model may use a different type of base line (STATIC or DYNAMIC)
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWell"></param>
        ''' <param name="pBaseLineWithAdjust"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetCurrentBaseLineID(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean) As GlobalDataTO

#End Region
    End Class

End Namespace
