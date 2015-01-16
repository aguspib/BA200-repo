Imports System.Timers
Imports System.Windows.Forms
Imports Biosystems.Ax00.Global
Imports CommunicationsAx00

Namespace Biosystems.Ax00.CommunicationsSwFw
    'Adapted from Enlace.vb (iPRO User Sw)

    'Public Class Enlace
    Public Class LinkLayer

#Region "Events definition"

        'Event that relates LinkLayer with ApplicationLayer (owner)
        Public Shared Event ActivateProtocol(ByVal pEvent As GlobalEnumerates.AppLayerEventList, _
                                             ByRef in_datos As String)

#End Region

#Region "Communication variables"

        'Private cls_serialPort As New CommunicationsAx00.ChannelClass
        'RH 14/10/2010 Remove New
        'Private cls_serialPort As New CommunicationsAx00.Channel
        Private cls_serialPort As Channel
        Private cls_tmrReception As New Timers.Timer()
        'RH 14/10/2010 Remove New
        'Private WithEvents cls_notifier As New CommunicationsAx00.ReceptionNotifier
        Private WithEvents cls_notifier As ReceptionNotifier

#End Region

#Region "Declarations"

        Private bytgAjustesByte() As Byte  '// trama de Ajustes en formato byte
        Private myLogAcciones As New ApplicationLogManager()

        Private Enum InstructionTypes
            'iPRO definitions (dont use them in Ax00)
            'Especial = 1
            'Estado = 2
            'Preparacion = 3
            'Ajustes = 4
            'Sonora = 8              '// 06-07-09 XBC v3.0.0 - Alarmas Sonoras y Recuperables
            'TrLeerAjustes = 17      '// 15-05-09 XBC v2.0.0 - trama leer ajustes binarios (Fw -> Sw)
            'Portas = 21             '// 09-04-06 XBC v2.0.0 - Abrir Portas
            'OkPortas = 22           '// 09-04-06 XBC v2.0.0 - Abrir Portas
            'NumSerie = 20           '// 09-04-06 XBC v2.0.0 - Abrir Portas

            'PENDING TO DEFINE if needed
            State = 1

        End Enum

        Public ResponseTrack As DateTime = New DateTime 'SGM for track time 03/07/2012

        Private CurrentChannel As String 'SGM 06/07/2012
        Private CurrentSettings As String 'SGM 06/07/2012

#End Region

#Region "Methods"

        'Public Function Iniciar(ByVal in_polling As Boolean, ByRef in_form As Form) As Boolean
        Public Function Start(ByVal in_polling As Boolean) As Boolean
            'Try
            Start = False 'RH 07/03/2012

            AddHandler cls_tmrReception.Elapsed, _
                       AddressOf cls_tmrReception_Timer

            For Each p As Process In Process.GetProcessesByName("CommAx00")
                If p.CloseMainWindow() Then
                    'GlobalBase.CreateLogActivity("[Link][Start] --->> CloseMainWindow OK", "Link.Start", EventLogEntryType.Information, False)

                Else
                    p.Kill()
                    'GlobalBase.CreateLogActivity("[Link][Start] --->> Kill", "Link.Start", EventLogEntryType.Information, False)
                End If
            Next

            'cls_serialPort = CreateObject("CommunicationsAx00.Channel") 'AG 21/04/2010
            cls_serialPort = New Channel 'RH 14/10/2010

            If Not cls_serialPort.Init() Then
                Throw New Exception("[Link][Start] --->> Exception: Not cls_serialPort.Init()") 'RH 07/03/2012
            End If

            If in_polling Then
                '// Polling
                cls_tmrReception.Interval = 500

            Else
                ' // Interrupt
                cls_serialPort.ActivateEventReception()

                cls_notifier = cls_serialPort.Notifier

                cls_tmrReception.Interval = 1
            End If

            cls_tmrReception.Enabled = True
            Start = True

            'RH 14/10/2010 It is a bad practice to catch an exception, do nothing with it and throw it again. 
            'Catch an exception only if you want to do something with it 
            'Catch ex As Exception 
            '    Throw ex 
            'End Try 

        End Function


        'Public Function Abrir(ByVal in_channel) As Boolean
        Public Function Open(ByVal in_channel As String) As Boolean

            'Try                
            Open = cls_serialPort.OpenChannel(in_channel, "9600,N,8,1")
            'GlobalBase.CreateLogActivity("[Link][Open] --->> Open value: '" & Open & "'", "Link.Start", EventLogEntryType.Information, False)

            CurrentChannel = in_channel

            'RH 14/10/2010 It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'Catch ex As Exception
            '    Throw ex
            'End Try

        End Function

        'Public Function Configurar(ByRef in_settings As String) As Boolean
        Public Function Config(ByRef in_settings As String) As Boolean

            'Try                
            Config = cls_serialPort.ChannelSettings(in_settings)

            CurrentSettings = in_settings

            'RH 14/10/2010 It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'Catch ex As Exception
            '    Throw ex
            'End Try

        End Function

        'Public Function Arrancar() As Boolean
        Public Function StartComm(ByVal ConnectionData() As Byte) As Boolean

            'GlobalBase.CreateLogActivity("[Link][StartComm] --->> Try StartComm", "Link.StartComm", EventLogEntryType.Information, False)
            Try
                StartComm = cls_serialPort.StartComm(ConnectionData)
                'GlobalBase.CreateLogActivity("[Link][StartComm] --->> StartComm value: '" & StartComm & "'", "Link.StartComm", EventLogEntryType.Information, False)

                ActivateReception()
                'GlobalBase.CreateLogActivity("[Link][StartComm] --->> after ActivateReception()", "Link.StartComm", EventLogEntryType.Information, False)

            Catch ex As Exception
                GlobalBase.CreateLogActivity("[Link][StartComm] --->> Exception Error", "Link.StartComm", EventLogEntryType.Error, False)
                Throw 'ex
            End Try

        End Function


        'New function to send instruction with byte array
        'Public Function EnviarSincronoByte(ByRef in_datosByte() As Byte) As Boolean
        Public Function SendSynchronousByte(ByRef in_datosByte() As Byte) As Boolean

            'Try
            SendSynchronousByte = cls_serialPort.SendSynchronousDataByte(in_datosByte)

            'RH 14/10/2010 It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'Catch ex As Exception
            '    'AG 06/07/2012 - activate try ... catch
            '    GlobalBase.CreateLogActivity("[Link][SendSynchronousByte] --->> Exception Error", "Link.SendSynchronousByte", EventLogEntryType.Error, False)
            '    Throw 'ex
            'End Try

        End Function

        'Public Function Parar() As Boolean
        Public Function StopComm() As Boolean

            'Try                
            DeactivateReception()
            StopComm = cls_serialPort.StopComm()

            'RH 14/10/2010 It is a bad practice to catch an exception and do nothing with it.
            'This hides the source of possible errors or bugs
            'Catch ex As Exception

            'End Try

        End Function

        'Public Function Cerrar() As Boolean
        Public Function Close() As Boolean

            'Try                
            DeactivateReception()
            Close = cls_serialPort.CloseChannel

            'RH 14/10/2010 It is a bad practice to catch an exception and do nothing with it.
            'This hides the source of possible errors or bugs
            'Catch ex As Exception

            'End Try

        End Function

        'Private Sub ActivarRecepcion()
        Private Sub ActivateReception()

            If cls_notifier Is Nothing Then
                '// Polling
                If cls_tmrReception IsNot Nothing Then 'SGM 06/07/2012
                    cls_tmrReception.Interval = 500
                    cls_tmrReception.Enabled = True
                End If
            Else
                '// Interrupt
                If cls_serialPort IsNot Nothing Then 'SGM 06/07/2012
                    cls_serialPort.ActivateEventReception()
                End If
            End If

        End Sub

        'Private Sub DesactivarRecepcion()
        Private Sub DeactivateReception()

            If cls_notifier Is Nothing Then
                '// Polling
                If cls_tmrReception IsNot Nothing Then 'SGM 06/07/2012
                    cls_tmrReception.Enabled = False
                End If
            Else
                '// Interrupt
                If cls_serialPort IsNot Nothing Then 'SGM 06/07/2012
                    cls_serialPort.DeactivateEventReception(False)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Instruction reception
        ''' By polling: check for instructions every 0.5s
        ''' By interrupt: timer is enabled only when a instruction is received
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub cls_tmrReception_Timer(ByVal source As Object, _
                                          ByVal e As ElapsedEventArgs)

            Dim dataReceived As String = ""
            Dim dataReceivedByte() As Byte = Nothing
            Dim i As Integer
            'Dim fi As Integer

            'Try
            If Not cls_serialPort Is Nothing Then
                'Disable timer during instruction receive treatment
                cls_tmrReception.Enabled = False

                'Reception byte array from CommAx00
                If cls_serialPort.ReceiveDataByteToString(dataReceivedByte) Then
                    dataReceived = ""

                    If Not dataReceivedByte Is Nothing Then
                        ''AG 08/06/2012 - Comment when stress tests are finished
                        'GlobalBase.CreateLogActivity("Instruction received", "Link.cls_tmrReception_Timer", EventLogEntryType.Information, False)

                        For i = 0 To UBound(dataReceivedByte)
                            If i = 0 Then
                                dataReceived = dataReceived + Chr(dataReceivedByte(i) And &H7F)

                            Else
                                dataReceived = dataReceived + Chr(dataReceivedByte(i))
                            End If
                        Next i

                        'This code save the data received into a global class variable only in case we receive the adjustments instruction
                        'fi = CInt(Mid$(dataReceived, 1, 2))
                        'If fi = InstructionTypes.TrLeerAjustes Then
                        '    bytgAjustesByte = dataReceivedByte
                        'End If

                        'SGM Track 
                        MyClass.ResponseTrack = DateTime.Now 'SGM 03/07/2012 just for debugging timing improvement
                        'end Track

                        'AG 10/07/2012 - move before End Sub
                        'If Not cls_serialPort Is Nothing Then 'SGM 06/07/2012
                        '    'AG 20/04/2010
                        '    RaiseEvent ActivateProtocol(GlobalEnumerates.AppLayerEventList.RECEIVE, dataReceived)
                        '    'Activate reception (timer <polling> or events <interrupt>)
                        '    ActivateReception()
                        '    Application.DoEvents()
                        'End If
                    End If

                Else
                    GlobalBase.CreateLogActivity("[Link][cls_tmrReception_Timer] --- Returns False", "Link.cls_tmrReception_Timer", EventLogEntryType.Information, False) 'AG 20/02/2014 - #1514 (change Error to Information)
                End If
            End If

            '    'RH 14/10/2010 It is a bad practice to catch an exception, do nothing with it and throw it again.
            '    'Catch an exception only if you want to do something with it
            'Catch ex As Exception
            '    'AG 06/07/2012 - activate try ... catch
            '    GlobalBase.CreateLogActivity("[Link][cls_tmrReception_Timer] --- Exception Error", "Link.cls_tmrReception_Timer", EventLogEntryType.Error, False)
            '    Throw 'ex
            'End Try

            'AG 10/07/2012
            If Not cls_serialPort Is Nothing Then
                RaiseEvent ActivateProtocol(GlobalEnumerates.AppLayerEventList.RECEIVE, dataReceived)
                'Activate reception (timer <polling> or events <interrupt>)
                ActivateReception()
                Application.DoEvents()
            End If
            'AG 10/07/2012
        End Sub



        ''' <summary>
        ''' Communications server event. Allow us read the received instructions by interrupt
        ''' In order to work properly it's needed to configure the server with the cls_serialPort.ActivateEventReception method
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub cls_notifier_DataReceived() Handles cls_notifier.DataReceived

            'Disable the event reception in order to dont allow instruction interference
            cls_serialPort.DeactivateEventReception(False)
            cls_tmrReception.Interval = 1
            cls_tmrReception.Enabled = True

        End Sub

        Protected Overrides Sub Finalize()

            'If Not cls_serialPort Is Nothing Then
            cls_serialPort = Nothing
            'End If

            'If Not cls_tmrReception Is Nothing Then
            cls_tmrReception = Nothing
            'End If

        End Sub

        'Public Sub Terminar()
        Public Sub Terminate()

            Try
                'GlobalBase.CreateLogActivity("[Link][Terminate] --->> Closing...", "Link.Terminate", EventLogEntryType.Information, False)
                Me.StopComm()

                Me.Close()
                'GlobalBase.CreateLogActivity("[Link][Terminate] --->> Closing cls_serialPort...", "Link.Terminate", EventLogEntryType.Information, False)

                'If Not cls_serialPort Is Nothing Then
                cls_serialPort = Nothing
                'End If
                'GlobalBase.CreateLogActivity("[Link][Terminate] --->> Closing cls_notifier...", "Link.Terminate", EventLogEntryType.Information, False)

                'If Not cls_notifier Is Nothing Then
                cls_notifier = Nothing
                'End If
                'GlobalBase.CreateLogActivity("[Link][Terminate] --->> Closing cls_tmrRecepcio...", "Link.Terminate", EventLogEntryType.Information, False)

                'If Not cls_tmrReception Is Nothing Then
                cls_tmrReception = Nothing
                'End If

                'Force killing the CommAx00 process from tasks administrator
                For Each p As Process In Process.GetProcessesByName("CommAx00")
                    If p.CloseMainWindow() Then
                        'GlobalBase.CreateLogActivity("[Link][Terminate] --->> CloseMainWindow OK", "Link.Terminate", EventLogEntryType.Information, False)
                    Else
                        p.Kill()
                        'GlobalBase.CreateLogActivity("[Link][Terminate] --->> Kill", "Link.Terminate", EventLogEntryType.Information, False)
                    End If
                Next

            Catch ex As Exception
                'AG 06/07/2012 - activate try ... catch
                GlobalBase.CreateLogActivity("[Link][Terminate] --->> Error !", "Link.Terminate", EventLogEntryType.Error, False)
                Throw 'ex
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 03/07/2012</remarks>
        Public Function Synchronize() As Boolean

            Dim result As Boolean

            Try

                'result=cls_serialPort.Synchronize()

            Catch ex As Exception
                result = False
            End Try

        End Function

#End Region

    End Class

End Namespace