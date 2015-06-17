Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Core.Services.Interfaces
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Services

    Public MustInherit Class AsyncService
        Implements IAsyncService

        Sub New(analyzer As IAnalyzerManager)
            _analyzer = analyzer
        End Sub

#Region "Attributes"

        Protected WithEvents _analyzer As IAnalyzerManager
        Protected _status As ServiceStatusEnum = ServiceStatusEnum.NotYetStarted

#End Region

#Region "Properties"

        Public Property OnServiceStatusChange As Action(Of IServiceStatusCallback) Implements IAsyncService.OnServiceStatusChange
        Public Overridable Property Status As ServiceStatusEnum Implements IAsyncService.Status
            Get
                Return _status
            End Get
            Set(value As ServiceStatusEnum)
                If value <> _status Then
                    _status = value
                    If OnServiceStatusChange IsNot Nothing Then
                        ServiceStatusCallback.Invoke(Me)
                    End If
                End If
            End Set
        End Property

#End Region

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If
                _analyzer = Nothing
                'Debug.WriteLine("Service has been disposed")
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me._disposedValue = True
        End Sub


        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

#Region "Public Methods"

        Public MustOverride Function StartService() As Boolean Implements IAsyncService.StartService
        Public MustOverride Sub PauseService() Implements IAsyncService.PauseService
        Public MustOverride Sub RestartService() Implements IAsyncService.RestartService

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="flagsDs"></param>
        ''' <remarks></remarks>
        Public Sub UpdateFlags(ByVal flagsDs As AnalyzerManagerFlagsDS) Implements IAsyncService.UpdateFlags
            If flagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, flagsDs)
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExistsBottleAlarmsOrRotorIsMissing() As Boolean Implements IAsyncService.ExistsBottleAlarmsOrRotorIsMissing

            Dim existsAlarms As Boolean = False
            Dim bottleErrAlarm As Boolean = False
            Dim reactRotorMissingAlarm As Boolean = _analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_MISSING_ERR) 'AG 12/03/2012

            If _analyzer.ExistBottleAlarms() Then
                bottleErrAlarm = True
            End If

            If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                existsAlarms = True
            End If

            Return existsAlarms

        End Function


#End Region

    End Class

End Namespace
