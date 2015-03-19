Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Namespace Biosystems.Ax00.Core.Services

    Public MustInherit Class AsyncService
        Implements IAsyncService

        Protected WithEvents _analyzer As IAnalyzerManager

        Sub New(analyzer As IAnalyzerManager)
            _analyzer = analyzer
        End Sub

        Public Property OnServiceStatusChange As Action(Of IServiceStatusCallback) Implements IAsyncService.OnServiceStatusChange

        Public MustOverride Function StartService() As Boolean Implements IAsyncService.StartService


        'Public MustOverride ReadOnly Property Status As ServiceStatusEnum Implements IAsyncService.Status
        Public Property Status As ServiceStatusEnum Implements IAsyncService.Status
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

#Region "attributes"
        Protected _status As ServiceStatusEnum = ServiceStatusEnum.NotYetStarted
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If
                _analyzer = Nothing
                Debug.WriteLine("Service has been disposed")
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub


        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace