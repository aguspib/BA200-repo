Namespace Biosystems.Ax00.Core.Services
    Public Class ServiceStatusCallback
        Implements IServiceStatusCallback

        Public Property NewServiceStatus As ServiceStatusEnum Implements IServiceStatusCallback.NewServiceStatus

        Public Property Sender As IAsyncService Implements IServiceStatusCallback.Sender

        ReadOnly _service As IAsyncService


        ''' <summary>
        ''' This method handles the callback service.
        ''' </summary>
        ''' <param name="service">The sender service to be sent</param>
        ''' <param name="status">The status to be sent</param>
        ''' <remarks></remarks>
        Shared Sub Invoke(service As IAsyncService, status As ServiceStatusEnum)
            If service Is Nothing OrElse service.OnServiceStatusChange Is Nothing Then Return
            Dim SSC As New ServiceStatusCallback
            SSC.Sender = service
            SSC.NewServiceStatus = status
            service.OnServiceStatusChange.Invoke(SSC)
        End Sub

    End Class
End Namespace
