Imports Biosystems.Ax00.Global

Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ''' <summary>
    ''' 
    ''' You can add an UnhandledException event handler that intercepts any exceptions that you have not already caught by using Try Catch blocks. This event handler is contained in the file ApplicationEvents.vb, which is normally hidden.
    ''' To create this event handler: 
    '''  - In Solution Explorer, right-click the application and select Properties. 
    '''  - On the Application tab, click View Application Events. (It's near the bottom so you may need to scroll to find it.) 
    '''  - In the code editor, open the left dropdown and select "(MyApplication Events)." Then in the right dropdown select UnhandledException. 
    ''' Now you can catch exceptions that are not caught by other exception handlers. 
    ''' 
    ''' Note that the exception handler only fires when you are running outside the debugger. In the debugger, the debugger itself catches exceptions that your other code doesn't. 
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 10/03/2014 - #1520
    ''' </remarks>
    Partial Friend Class MyApplication

        Private Sub MyApplication_UnhandledException(sender As Object, e As ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            GlobalBase.CreateLogActivity("An Unhandled Exception has been occurred ", "MyApplication.ApplicationEvents", EventLogEntryType.Error, False)
            ' Display message to user and then exit.
            MessageBox.Show(Biosystems.Ax00.Global.GlobalConstants.UNHANDLED_EXCEPTION_ERR.ToString, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Question)
            e.ExitApplication = False
        End Sub
    End Class

End Namespace

