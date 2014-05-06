Imports System.Drawing
Imports System.Windows.Forms

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSComboBoxEx

        'Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        '    MyBase.OnPaint(pe)

        '    'Add your custom paint code here
        'End Sub

        Private Const WM_LBUTTONDBLCLK As Long = &H203
        Private Const WM_LBUTTONDOWN As Long = &H201
        'Private Const WM_LBUTTONUP As Long = &H202
        'Private Const WM_MBUTTONDBLCLK As Long = &H209
        'Private Const WM_MBUTTONDOWN As Long = &H207
        'Private Const WM_MBUTTONUP As Long = &H208
        'Private Const WM_MOUSEHOVER As Long = &H2A1
        'Private Const WM_RBUTTONDBLCLK As Long = &H206
        Private Const WM_RBUTTONDOWN As Long = &H204
        'Private Const WM_RBUTTONUP As Long = &H205
        'Private Const WM_MOUSELEAVE As Long = &H2A3
        'Private Const WM_MOUSEMOVE As Long = &H200

#Region "Private Backing Fields"
        ' Private backing field for the shadowed enabled property
        Private myEnabled As Boolean = False
        ' BackColor of control when it is enabled
        Private myEnabledBackcolor As Color = MyBase.BackColor
        ' BackColor of a control when it is disabled
        Private myDisabledBackColor As Color = Color.LightGray
#End Region

#Region "Public Exposed Fields"

        ''' <summary>
        ''' Gets or Sets the value indicating if this control is enabled
        ''' </summary>
        ''' <value>Boolean</value>
        ''' <returns>True of control is enabled, otherwise false</returns>
        ''' <remarks>This property shadows the ComboBox base class enabled property</remarks>
        Public Shadows Property Enabled() As Boolean
            Get
                Return myEnabled
            End Get
            Set(ByVal Value As Boolean)
                If myEnabled <> Value Then
                    myEnabled = Value
                    OnEnabledChanged(New EventArgs)
                End If
            End Set
        End Property


        ''' <summary>
        ''' Gets or Sets the BackColor of the control when it is disabled
        ''' </summary>
        ''' <value>Color Structure</value>
        Public Property DisabledBackColor() As Color
            Get
                Return myDisabledBackColor
            End Get
            Set(ByVal value As Color)
                myDisabledBackColor = value
                If Not myEnabled Then
                    MyBase.BackColor = myDisabledBackColor
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or Sets the BackColor of the control when it is enabled
        ''' </summary>
        ''' <value>Color Structure</value>
        ''' <remarks>Shadows the base class BackColor property</remarks>
        Public Shadows Property BackColor() As Color
            Get
                Return myEnabledBackcolor
            End Get
            Set(ByVal value As Color)
                If myEnabledBackcolor <> value Then
                    myEnabledBackcolor = value
                    MyBase.BackColor = myEnabledBackcolor
                End If
            End Set
        End Property

#End Region

        ''' <summary>
        ''' When the shadowed enabled property changes, this method is called
        ''' </summary>
        Protected Overrides Sub OnEnabledChanged(ByVal e As System.EventArgs)
            'COMMON ROUTINE FOR TOGGLING ENABLED STATUS
            ToggleEnabled()
            'SEND NOTIFICATION TO BASE CLASS
            MyBase.OnEnabledChanged(e)
        End Sub


        'OVERRIDE PreProcessMessage TO LOOK FOR KEY PRESSES AND FILTER THEM
        Public Overrides Function PreProcessMessage(ByRef msg As Message) As Boolean
            'PREVENT KEYBOARD ENTRY IF CONTROL IS DISABLED
            If Not myEnabled Then
                'CHECK IF ITS A KEYDOWN MESSAGE (&H100)
                If msg.Msg = &H100 Then
                    'GET THE KEY THAT WAS PRESSED
                    Dim key As Int32 = msg.WParam.ToInt32
                    'ALLOW TAB, LEFT, OR RIGHT KEYS
                    If key <> Keys.Tab OrElse key <> Keys.Left OrElse _
                           key <> Keys.Right Then
                        Return True
                    End If
                End If
            End If
            'CALL BASE METHOD SO DELEGATES RECEIVE EVENT
            Return MyBase.PreProcessMessage(msg)
        End Function

        'OVERRIDE WndProc TO LOOK FOR DROP DOWN MESSAGES AND FILTER THEM
        Protected Overrides Sub WndProc(ByRef m As Message)
            'PREVENT DROPDOWN LIST DISPLAYING IF READONLY

            If Not myEnabled Then
                If m.Msg = WM_LBUTTONDOWN Then
                    m.Msg = WM_RBUTTONDOWN
                ElseIf m.Msg = WM_LBUTTONDOWN OrElse m.Msg = WM_LBUTTONDBLCLK Then
                    Return
                End If
            End If


            'CALL BASE METHOD SO DELEGATES RECEIVE EVENT
            MyBase.WndProc(m)
        End Sub

        'WHEN THE CONTROL IS IN A CONTAINER, AND THE CONTAINER'S ENABLED PROPERTY
        'IS SET TO FALSE, THIS CONTROL GETS ITS OnParentEnabledChanged CALLED
        'NOT JUST THE CONTROLS ENABLED PROPERTY SET SO WE OVERRIDE THIS, AND
        'TOGGLE ENABLED STATE ACCORDINGLY
        Protected Overrides Sub OnParentEnabledChanged(ByVal e As System.EventArgs)
            myEnabled = MyBase.Parent.Enabled

            If myEnabled Then
                MyBase.OnParentEnabledChanged(e)
            Else
                ToggleEnabled()
            End If
        End Sub

#Region "Support Methods"

        'COMMON ROUTINE FOR TOGGLING ENABLED STATE
        Private Sub ToggleEnabled()
            'IF THE CONTROL IS DISABLED, TURN OFF ITS TABSTOP
            MyBase.TabStop = myEnabled
            'IF THE CONTROL IS DISABLED, SET ITS CONTEXT MENU TO
            'A DUMMY NEW CONTEXT MENU SO WE DON'T GET THE
            'DEFAULT CONTEXT MENU, OTHERWISE SETTING IT TO
            'NOTHING REAPPLIES THE DEFAULT CONTEXT MENU
            'ALSO SET BACK COLOR ACCORDINGLY
            If Not myEnabled Then
                MyBase.ContextMenuStrip = New ContextMenuStrip
                MyBase.BackColor = myDisabledBackColor
                'MyBase.ForeColor = Color.DarkGray
            Else
                MyBase.ContextMenuStrip = Nothing
                MyBase.BackColor = myEnabledBackcolor
                'MyBase.ForeColor = Color.DarkGray
            End If

            'DESELECT ANY TEXT FROM COMBOBOX
            Me.SelectionLength = 0
        End Sub

#End Region




    End Class
End Namespace