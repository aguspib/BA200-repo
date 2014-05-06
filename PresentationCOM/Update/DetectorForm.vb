Imports System.Runtime.InteropServices
Imports System.Windows.Forms 'XBC 24/10/2011
Imports Biosystems.Ax00.Global

Public Class DetectorForm
    'Delegate for event handler to handle the device events 
    Public Delegate Sub DriveDetectorEventHandler(ByVal sender As Object, ByVal e As DriveDetectorEventArgs)

    Public Class DetectorForm
        Inherits Form
        Private mDetector As DriveDetector = Nothing

        Public Sub New(ByVal pDetector As DriveDetector)
            mDetector = pDetector
            Me.MinimizeBox = False
            Me.MaximizeBox = False
            Me.ShowInTaskbar = False
            Me.ShowIcon = False
            Me.FormBorderStyle = FormBorderStyle.None
        End Sub

        Private Sub DetectorForm_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
            Me.Visible = False
        End Sub

        Private Sub DetectorForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.Size = New System.Drawing.Size(5, 5)
        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            MyBase.WndProc(m)
            If Not mDetector Is Nothing Then
                mDetector.WndProc(m)
            End If
        End Sub
    End Class

    'Our class for passing in custom arguments to our event handlers 
    Public Class DriveDetectorEventArgs
        Inherits EventArgs

        Public Sub New()
            Cancel = False
            Message = ""
            HookQueryRemove = False
        End Sub

        Public Cancel As Boolean
        Public Message As String
        Public HookQueryRemove As Boolean

    End Class

    ''' <summary>
    ''' Detects insertion or removal of removable drives.
    ''' Use it in 1 or 2 steps:
    ''' 1) Create instance of this class in your project and add handlers for the
    ''' DeviceArrived, DeviceRemoved and QueryRemove events.
    ''' AND (if you do not want drive detector to creaate a hidden form))
    ''' 2) Override WndProc in your form and call DriveDetector's WndProc from there. 
    ''' If you do not want to do step 2, just use the DriveDetector constructor without arguments and
    ''' it will create its own invisible form to receive messages from Windows.
    ''' </summary>
    Public Class DriveDetector

        Private Const WM_DEVICECHANGE As Integer = &H219
        Private Const DBT_DEVICEARRIVAL As Integer = &H8000
        Private Const DBT_DEVICEREMOVECOMPLETE As Integer = &H8004
        'Private Const DBT_DEVTYP_VOLUME As Integer = &H2
        Private Const DBT_BSAnalyzer_REMOVED = &H3
        Public Event DeviceRemoved As DriveDetectorEventHandler
        Private mDeviceNotifyHandle As IntPtr

        Public Sub New()
            Dim frm As New DetectorForm(Me)
            frm.Show()
            Init(frm, Nothing)
        End Sub

        Public Sub New(ByVal pControl As Control)
            Init(pControl, Nothing)
        End Sub

        Private Sub Init(ByVal control As Control, ByVal fileToOpen As String)
            mDeviceNotifyHandle = IntPtr.Zero
        End Sub

        Public Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Try
                Dim devType As Integer
                'Validate if there was a device change
                If m.Msg = WM_DEVICECHANGE Then
                    Select Case m.WParam.ToInt32()

                        Case DBT_DEVICEARRIVAL 'Validate if was a new device added
                            devType = Marshal.ReadInt32(m.LParam, 4)

                        Case DBT_DEVICEREMOVECOMPLETE 'Validate if device was removed.
                            devType = Marshal.ReadInt32(m.LParam, 4)
                            'Validate if the removed item is the BS Analyzer.
                            If devType = DBT_BSAnalyzer_REMOVED Then
                                Dim e As New DriveDetectorEventArgs
                                e.Message = GlobalEnumerates.Messages.ERROR_COMM.ToString()
                                Debug.Print("In DetectorForm.WndProc; before Raise Event DeviceRemoved")
                                RaiseEvent DeviceRemoved(Nothing, e)
                            End If
                            Exit Select
                    End Select
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End Sub

    End Class

End Class