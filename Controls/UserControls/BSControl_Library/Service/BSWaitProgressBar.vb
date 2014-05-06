Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSWaitProgressBar

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub

#Region "Public Enumerates"

        Public Enum Visibilities
            NEVER
            INPROGRESS
            ALWAYS
        End Enum

#End Region

#Region "Timing Delegates and Timers"

        Private WaitTimerCallBack As System.Threading.TimerCallback
        Private WithEvents WaitTimer As System.Threading.Timer
        Private Delegate Sub UpdateProgressCallBack()
        Private Delegate Sub HideProgressBarCallBack()
#End Region

#Region "Public Events"
        Public Event IsTimeForWaitElapsed(ByVal sender As Object)

#End Region

#Region "Private Properties"

        Private Property StartTime() As TimeSpan
            Get
                Return StartTimeAttr
            End Get
            Set(ByVal value As TimeSpan)
                StartTimeAttr = value
            End Set
        End Property
        Private StartTimeAttr As TimeSpan = Nothing

        Private Property ExpectedEndTime() As TimeSpan
            Get
                Return ExpectedEndTimeAttr
            End Get
            Set(ByVal value As TimeSpan)
                ExpectedEndTimeAttr = value
            End Set
        End Property
        Private ExpectedEndTimeAttr As TimeSpan = Nothing

        Private Property IsInProgress() As Boolean
            Get
                Return IsInprogressAttr
            End Get
            Set(ByVal value As Boolean)
                IsInprogressAttr = value
            End Set
        End Property
        Private IsInprogressAttr As Boolean = False

#End Region

#Region "Public Properties"
        ''' <summary>
        ''' Visibility of the progress bar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SGM 07/07/2011</remarks>
        Public Property Visibility() As Visibilities
            Get
                Return VisibilityAttr
            End Get
            Set(ByVal value As Visibilities)
                VisibilityAttr = value
                If Not IsInprogress Then
                    MyBase.Visible = (value = Visibilities.ALWAYS)
                End If
            End Set
        End Property
        Private VisibilityAttr As Visibilities = Visibilities.INPROGRESS

        ''' <summary>
        ''' Time interval in which the progras bar will be waiting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SGM 07/07/2011</remarks>
        Public Property TimeForWait() As Integer
            Get
                Return TimeForWaitAttr
            End Get
            Set(ByVal value As Integer)
                If value >= 0 Then
                    TimeForWaitAttr = value
                End If
            End Set
        End Property
        Private TimeForWaitAttr As Integer = 0


        Public ReadOnly Property ElapsedTime() As Integer
            Get
                Dim NowTime As New TimeSpan(Now.Ticks)
                Try
                    Return CInt(NowTime.TotalMilliseconds - MyClass.StartTime.TotalMilliseconds)
                Catch ex As Exception
                    Return -1
                End Try
            End Get
        End Property

        Public ReadOnly Property IsProgressing() As Boolean
            Get
                Return IsInprogressAttr
            End Get
        End Property

#End Region

#Region "Public Methods"


        Public Sub StartWaiting(Optional ByVal pTimeForWait As Integer = -1)

            Try
                If pTimeForWait > -1 Then
                    MyClass.TimeForWait = pTimeForWait
                End If

                If MyClass.TimeForWait > 0 Then
                    MyClass.StartTime = New TimeSpan(Now.Ticks)
                    MyClass.ExpectedEndTime = MyClass.StartTime.Add(New TimeSpan(0, 0, 0, 0, MyClass.TimeForWait))

                    MyBase.Minimum = 0
                    MyBase.Maximum = MyClass.TimeForWait

                    MyClass.WaitTimerCallBack = New System.Threading.TimerCallback(AddressOf OnWaitTimerTick)
                    MyClass.WaitTimer = New System.Threading.Timer(MyClass.WaitTimerCallBack, New Object, 10, 10)

                    MyClass.IsInprogress = True

                    If MyClass.Visibility = Visibilities.ALWAYS Or MyClass.Visibility = Visibilities.INPROGRESS Then
                        MyBase.Visible = True
                    End If
                End If

            Catch ex As Exception
                MyClass.StopWaiting()
                Throw ex
            End Try

        End Sub


        Public Sub StopWaiting()

            Try

                If MyBase.InvokeRequired Then
                    Dim d As New HideProgressBarCallBack(AddressOf HideProgressBar)
                    MyBase.Invoke(d)
                Else
                    MyClass.HideProgressBar()
                End If

                If MyClass.WaitTimer IsNot Nothing Then
                    MyClass.WaitTimer.Dispose()
                    MyClass.WaitTimer = Nothing
                End If

                If MyClass.WaitTimerCallBack IsNot Nothing Then
                    MyClass.WaitTimerCallBack = Nothing
                End If

                MyClass.IsInprogress = False

            Catch ex As Exception
                Throw ex
            End Try

        End Sub

        
#End Region

#Region "Private Methods"


        Private Sub UpdateProgress()
            Try
                If MyClass.ElapsedTime >= MyBase.Minimum And MyClass.ElapsedTime <= MyBase.Maximum Then
                    MyBase.Value = MyClass.ElapsedTime
                    MyBase.Refresh()
                    If MyBase.Container IsNot Nothing Then
                        Dim myContainer As Windows.Forms.Control = CType(MyBase.Container, Windows.Forms.Control)
                        If myContainer IsNot Nothing Then
                            myContainer.Refresh()
                        End If
                    End If
                End If
            Catch ex As Exception
                MyClass.StopWaiting()
                Throw ex
            End Try
        End Sub

        Private Sub HideProgressBar()
            Try
                If MyClass.Visibility <> Visibilities.ALWAYS Then
                    MyBase.Visible = False
                End If

                MyBase.Value = MyBase.Minimum

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub OnWaitTimerTick(ByVal stateInfo As Object)

            Try

                If (MyClass.ElapsedTime <= MyBase.Maximum) Then
                    If MyBase.InvokeRequired Then
                        Dim d As New UpdateProgressCallBack(AddressOf UpdateProgress)
                        MyBase.Invoke(d)
                    Else
                        MyClass.UpdateProgress()
                    End If
                Else
                    MyClass.StopWaiting()
                    RaiseEvent IsTimeForWaitElapsed(Me)

                End If

            Catch ex As Exception
                MyClass.StopWaiting()
                Throw ex
            End Try
        End Sub

       
#End Region

    End Class
End Namespace