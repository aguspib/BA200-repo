Imports Biosystems.Ax00.Core.Services.Interfaces

Namespace Biosystems.Ax00.Core.Services
    Public Class ServiceStatusCallback
        Implements IServiceStatusCallback

        Public Property Sender As IAsyncService Implements IServiceStatusCallback.Sender

        ReadOnly _service As IAsyncService


        ''' <summary>
        ''' This method handles the callback service.
        ''' </summary>
        ''' <param name="service">The sender service to be sent</param>
        ''' <remarks></remarks>
        Shared Sub Invoke(service As IAsyncService)
            If service Is Nothing OrElse service.OnServiceStatusChange Is Nothing Then
                Return
            Else
                Dim ssc As New ServiceStatusCallback
                ssc.Sender = service
                service.OnServiceStatusChange.Invoke(ssc)
            End If
        End Sub

    End Class
End Namespace
