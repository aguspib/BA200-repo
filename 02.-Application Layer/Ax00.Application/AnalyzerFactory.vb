Imports Biosystems.Ax00.Core
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.App

    Public MustInherit Class AnalyzerFactory
        Implements IAnalyzerFactory

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="assemblyName"></param>
        ''' <param name="analyzerModel"></param>
        ''' <param name="startingApplication"></param>
        ''' <param name="workSessionIDAttribute"></param>
        ''' <param name="analyzerIDAttribute"></param>
        ''' <param name="fwVersionAttribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerManager Implements IAnalyzerFactory.CreateAnalyzer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="parameter"></param>
        ''' <param name="analyzerModel"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub LoadAnalyzerConfiguration(analyzer As IAnalyzerManager)

            Dim myGlobal As New GlobalDataTO
            Dim myParametersDS As New ParametersDS
            Dim myParams As New SwParametersDelegate
            Dim textValue As String = String.Empty

            myGlobal = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.BL_TYPE_FOR_CALCULATIONS.ToString(), analyzer.Model)
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                textValue = CStr(myGlobal.SetDatos)
                analyzer.BaseLineTypeForCalculations = DirectCast([Enum].Parse(GetType(BaseLineType), textValue), BaseLineType)
            End If

            myGlobal = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.BL_TYPE_FOR_WELLREJECT.ToString(), analyzer.Model)
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                textValue = CStr(myGlobal.SetDatos)
                analyzer.BaseLineTypeForWellReject = DirectCast([Enum].Parse(GetType(BaseLineType), textValue), BaseLineType)
            End If

        End Sub

#End Region

    End Class

End Namespace

