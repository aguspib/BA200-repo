Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.App

    Public NotInheritable Class AnalyzerController
        Implements IAnalyzerController

        Private Shared ReadOnly _instance As New Lazy(Of AnalyzerController)(Function() New AnalyzerController(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication)
        Private _factory As IAnalyzerFactory

        Private Sub New()
        End Sub

#Region "Properties"

        Public Property Analyzer As IAnalyzerEntity Implements IAnalyzerController.Analyzer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance() As AnalyzerController
            Get
                Return _instance.Value
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property IsAnalyzerInstantiated() As Boolean
            Get
                Return (Not _instance.Value.Analyzer Is Nothing)
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="assemblyName"></param>
        ''' <param name="analyzerModel"></param>
        ''' <param name="startingApplication"></param>
        ''' <param name="workSessionIDAttribute"></param>
        ''' <param name="analyzerIDAttribute"></param>
        ''' <param name="fwVersionAttribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateAnalyzer(model As AnalyzerModelEnum, assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerEntity Implements IAnalyzerController.CreateAnalyzer
            Select Case model
                Case AnalyzerModelEnum.BA200
                    _factory = New BA200AnalyzerFactory()
                Case AnalyzerModelEnum.BA400
                    _factory = New BA400AnalyzerFactory()
            End Select

            If (Not _factory Is Nothing) Then
                Analyzer = _factory.CreateAnalyzer(assemblyName, analyzerModel, startingApplication, workSessionIDAttribute, analyzerIDAttribute, fwVersionAttribute)
            End If

            Return Analyzer

        End Function

#End Region

    End Class

End Namespace

