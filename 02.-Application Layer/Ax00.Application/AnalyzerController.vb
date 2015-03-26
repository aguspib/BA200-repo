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

        Private Sub New()
            AsyncService.AppListener = New CoreListener
        End Sub

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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: IT 01/12/2014 - BA-2075
        ''' </remarks>
        Public Function StartWarmUpProcess(ByVal isInRecovering As Boolean) As Boolean
            Try

                If (_warmUpServices Is Nothing) Then
                    _warmUpServices = New WarmUpService(Analyzer)
                End If

                If (Not isInRecovering) Then
                    Return _warmUpServices.StartService()
                Else
                    Return _warmUpServices.RecoverProcess()
                End If

            Catch ex As Exception
                Throw ex
            End Try


            'Dim myGlobal As New GlobalDataTO
            'Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            'If (IsAnalyzerInstantiated) Then
            '    Analyzer.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound

            '    'Dim activateButtonsFlag As Boolean = False
            '    'Dim myCurrentAlarms As List(Of AlarmEnumerates.Alarms)
            '    'myCurrentAlarms = Analyzer.Alarms

            '    Analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True
            '    'DL 17/05/2012

            '    If (CheckStatusWarmUp()) Then
            '        Analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.StartInstrument)

            '        'If (Not myGlobal.HasError) Then
            '        If Analyzer.Connected Then
            '            'Start instrument instruction send OK (initialize wup UI flags)
            '            myGlobal = InitializeStartInstrument()
            '        Else
            '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = ""
            '        End If
            '    End If
            'End If

            'Return myGlobal

        End Function

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

        Public Sub ReuseRotorContentsForFLIGHT(responseHandler As Action(Of Boolean))
            If BaseLineService.CanRotorContentsByDirectlyRead Then
                Dim question As New YesNoQuestion
                question.Text = "This is a question"
                question.OnAnswered =
                    Sub()
                        responseHandler.Invoke(question.Result = MsgBoxResult.Yes)
                    End Sub
            Else
                responseHandler.Invoke(False)
            End If
        End Sub

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

#Region "Private Methods"

        ' ''' <summary>
        ' ''' 
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' Modified by XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
        ' '''             IT 01/12/2014 - BA-2075
        ' ''' </remarks>
        'Private Function InitializeStartInstrument() As GlobalDataTO
        '    Dim myGlobal As GlobalDataTO = Nothing
        '    'Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        'DL 09/09/2011
        '        'Set Enable (or set visible meeting 12/09/2011 ??) frame time W-Up in common Monitor
        '        'IMonitor.bsWamUpGroupBox.Enabled = True
        '        Dim swParams As New SwParametersDelegate

        '        ' Read W-Up full time configuration
        '        myGlobal = swParams.ReadByAnalyzerModel(Nothing, Analyzer.Model)

        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            Dim myParametersDS As New ParametersDS

        '            myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
        '            Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)

        '            myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
        '                      Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString) _
        '                      Select a).ToList

        '            Dim WUPFullTime As Single
        '            If myList.Count > 0 Then WUPFullTime = myList(0).ValueNumeric '= DateTime.Now.ToString("yyyyMMdd hh-mm") & "_" & myList(0).ValueText
        '        End If

        '        ' Save initial states when press over w-up
        '        If Not myGlobal.HasError Then
        '            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
        '            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

        '            'WUPCOMPLETEFLAG
        '            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '            With myAnalyzerSettingsRow
        '                .AnalyzerID = Analyzer.ActiveAnalyzer
        '                .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
        '                .CurrentValue = "0"
        '            End With
        '            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '            'WUPCOMPLETEFLAG
        '            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '            With myAnalyzerSettingsRow
        '                .AnalyzerID = Analyzer.ActiveAnalyzer
        '                .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
        '                ''.CurrentValue = Now.ToString 'AG + SA 05/10/2012
        '                '.CurrentValue = Now.ToString("yyyy/MM/dd HH:mm:ss")
        '                .CurrentValue = Now.ToString(CultureInfo.InvariantCulture)
        '            End With
        '            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '            Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
        '            myGlobal = myAnalyzerSettings.Save(Nothing, Analyzer.ActiveAnalyzer, myAnalyzerSettingsDS, Nothing)

        '        End If

        '        AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True 'SGM 13/04/2012 

        '    Catch ex As Exception
        '        Throw ex
        '    End Try

        '    Return myGlobal
        'End Function

        ' ''' <summary>
        ' ''' 
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' Created by: IT 01/12/2014 - BA-2075
        ' ''' </remarks>
        'Private Function CheckStatusWarmUp() As Boolean

        '    If (Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED") Then

        '        If Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.StartInstrument) = "END" AndAlso
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED" Then
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
        '        End If

        '        If Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END" AndAlso
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED" Then
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = ""
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
        '        End If

        '        If Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END" AndAlso
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED" Then
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = ""
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
        '        End If

        '        If Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END" AndAlso
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED" Then
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = ""
        '            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
        '        End If

        '        Return False

        '    Else
        '        Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 'clear sensor
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "" 'Reset flag
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "" 'Reset flag
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "" 'Reset flag
        '        Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"

        '        Return True
        '    End If

        'End Function

#End Region

    End Class

End Namespace

