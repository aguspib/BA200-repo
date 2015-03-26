Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports System.Globalization
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.App.PresentationLayerListener
Imports Biosystems.Ax00.App.PresentationLayerListener.Requests

Namespace Biosystems.Ax00.App

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  IT 19/12/2014 - BA-2143
    ''' </remarks>
    Public NotInheritable Class AnalyzerController
        Implements IAnalyzerController

        Private Shared ReadOnly _instance As New Lazy(Of AnalyzerController)(Function() New AnalyzerController(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication)
        Private _factory As IAnalyzerFactory
        Private _rotorChangeServices As RotorChangeServices 'BA-2143
        Private _warmUpServices As WarmUpService


#Region "Properties"

        Public Property Analyzer As IAnalyzerManager Implements IAnalyzerController.Analyzer

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

        Public Shared Property PresentationLayerInterface As IPresentationLayerListener

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
        ''' <remarks>
        ''' AG 10/11/2014 BA-2082 remove parameter model and use analyzerModel that are read from database
        ''' </remarks>
        Public Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerManager Implements IAnalyzerController.CreateAnalyzer
            Select Case analyzerModel
                Case AnalyzerModelEnum.A200.ToString 'BA200
                    _factory = New BA200AnalyzerFactory()
                Case AnalyzerModelEnum.A400.ToString 'BA400
                    _factory = New BA400AnalyzerFactory()
            End Select

            If (Not _factory Is Nothing) Then
                Analyzer = _factory.CreateAnalyzer(assemblyName, analyzerModel, startingApplication, workSessionIDAttribute, analyzerIDAttribute, fwVersionAttribute)
            End If

            Return Analyzer

        End Function

#Region "WarmUp Service"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: IT 26/03/2015 - BA-2406
        ''' </remarks>
        Public Function StartWarmUpProcess(ByVal isInRecovering As Boolean, reuseRotorContentsForFlight As Boolean) As Boolean
            Try

                If (_warmUpServices Is Nothing) Then
                    _warmUpServices = New WarmUpService(Analyzer)
                End If
                _warmUpServices.ReuseRotorContentsForBaseLine = reuseRotorContentsForFlight
                If (Not isInRecovering) Then
                    Return _warmUpServices.StartService()
                Else
                    Return _warmUpServices.RecoverProcess()
                End If

            Catch ex As Exception
                Throw
            End Try

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="isInRecovering"></param>
        ''' <remarks></remarks>
        Public Sub StartWarmupProcess(ByVal isInRecovering As Boolean)
            UseRotorContentsForFLIGHT(
                Sub(result As Boolean)
                    StartWarmUpProcess(isInRecovering, result)
                End Sub)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub PauseWarmUpService()
            Try

                If _warmUpServices IsNot Nothing Then
                    _warmUpServices.PauseService()
                End If

            Catch ex As Exception
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReStartWarmUpService()
            Try

                If _warmUpServices IsNot Nothing Then
                    _warmUpServices.StartService()
                End If

            Catch ex As Exception
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified by:  IT 26/03/2014 - BA-2406
        ''' </remarks>
        Public Sub WarmUpCloseProcess()
            Try
                If _warmUpServices IsNot Nothing Then
                    _warmUpServices.Dispose()
                    _warmUpServices = Nothing
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Change Rotor Service"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by:  IT 19/12/2014 - BA-2143
        '''               IT 30/01/2015 - BA-2216
        ''' </remarks>
        Public Function ChangeRotorStartProcess() As Boolean
            Try
                If (_rotorChangeServices Is Nothing) Then
                    _rotorChangeServices = New RotorChangeServices(Analyzer, _warmUpServices)
                End If

                Return _rotorChangeServices.StartService()

            Catch ex As Exception
                Throw ex
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by:  IT 19/12/2014 - BA-2143
        '''               IT 30/01/2015 - BA-2216
        ''' </remarks>
        Public Function ChangeRotorContinueProcess(ByVal isInRecovering As Boolean) As Boolean
            Try
                If (_rotorChangeServices Is Nothing) Then
                    _rotorChangeServices = New RotorChangeServices(Analyzer, _warmUpServices)
                End If

                If (Not isInRecovering) Then
                    Return _rotorChangeServices.ContinueProcess()
                Else
                    Return _rotorChangeServices.RecoverProcess()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified by:  IT 19/12/2014 - BA-2143
        '''               IT 30/01/2015 - BA-2216
        ''' </remarks>
        Public Sub ChangeRotorRepeatDynamicBaseLineReadStep()
            Try
                If (_rotorChangeServices Is Nothing) Then
                    _rotorChangeServices = New RotorChangeServices(Analyzer, _warmUpServices)
                End If

                _rotorChangeServices.RepeatDynamicBaseLineReadStep()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified by:  IT 19/12/2014 - BA-2143
        '''               IT 30/01/2015 - BA-2216
        ''' </remarks>
        Public Sub ChangeRotorFinalizeProcess()
            Try
                If (_rotorChangeServices Is Nothing) Then
                    _rotorChangeServices = New RotorChangeServices(Analyzer, _warmUpServices)
                End If

                _rotorChangeServices.EmptyAndFinalizeProcess()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified by:  IT 19/12/2014 - BA-2143
        ''' </remarks>
        Public Sub ChangeRotorCloseProcess()
            Try
                If _rotorChangeServices IsNot Nothing Then
                    _rotorChangeServices.Dispose()
                    _rotorChangeServices = Nothing
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

        Public Sub UseRotorContentsForFLIGHT(responseHandler As Action(Of Boolean))

            If responseHandler Is Nothing Then Return

            If IsAnalyzerInstantiated = False Then
                responseHandler.Invoke(False)
            End If

            If BaseLineService.CanRotorContentsByDirectlyRead Then
                Dim question As New YesNoQuestion
                question.Text = "This is a question"
                question.OnAnswered = Sub() responseHandler.Invoke(question.Result = MsgBoxResult.Yes)
                PresentationLayerInterface.QueueRequest(question)
            Else
                responseHandler.Invoke(False)
            End If
        End Sub

#End Region

#Region "Private Methods"


#End Region

    End Class

End Namespace

