Option Explicit On
Option Strict On

Imports Microsoft.Win32
Imports System.IO.Ports
Imports System.Management
Imports System.Windows.Forms
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
'Imports System.Configuration
Imports Biosystems.Ax00.Types 'AG 17/01/2011
Imports Biosystems.Ax00.BL
Imports Timer = System.Timers.Timer 'SGM 08/03/11
'Imports Biosystems.Ax00.DAL.DAO 'SGM 21/09/2011

Namespace Biosystems.Ax00.CommunicationsSwFw

    Partial Public Class ApplicationLayer

#Region "Declarations"

        'Declaration from (iPRO adaptation)
        'AG 22/04/2010 - comment not in use  (by now) variables
        Private ConnectedPortNumber As Integer = 0
        Private WaitRequestState As Boolean
        '// flag para el control de Tramas recibidas según los tiempos previstos
        Private FlagACKrebut As Boolean
        '// flag para el reintento de envio Tramas
        Private FlagRetry As Boolean

        ''// Timer para el Control de los tiempos previstos/máximos para el envío/ejecución de las Tramas
        Private TimerControl As New Timer()

        '// flag para la identificación de errores en los tiempos/intentos de envío/ejecución de las Tramas
        Private ReadOnly FlagERROR As Boolean
        'Private  ExitAll As Boolean ' sustitution for the variable Salir, we can't translate as exit, this create a conflict.
        'TR 23/04/2010 Crate the Class variable strClavesUSBenRegistro
        Private USBRegistryKey As New List(Of String)

        'AG - Required variables
        Private WithEvents Link As LinkLayer
        'Private waitingTimeTimer As New System.Timers.Timer() 'AG 07/05/2010 - Move to analyzer manager

        'AG 03/06/2010 All class enumerates are moved to GlobalEnumerate class (Region "Communications Project Enumerates"

        Public ResponseTrack As DateTime = New DateTime 'SGM for track time 03/07/2012
#End Region

#Region "Attributes"
        Private analyzerIDAttribute As String = "" 'AG 20/03/2012, required for read the reagents barcode length
        Private worksessionIDAttribute As String = ""

        'AG 20/04/2010 - Last ExecutionID sent to prepare & the complete instruction data
        Private lastExecutionIDSentAttribute As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND     '-1
        Private lastPreparationInstrucionSentAttribute As String = ""
        Private connectedPortNameAttribute As String = ""  'AG 22/04/2010 - Current connected port ("" no connection)
        Private connectedBaudsAttribute As String = ""  'AG 22/04/2010 - Current connected bauds ("" no connection)
        Private lastInstructionTypeSentAttribute As GlobalEnumerates.AppLayerEventList   'AG 27/04/2010

        Private lastInstructionSentAttribute As String = ""  'AG 20/10/2010 - Last instruction sent (every instruction)
        Private lastInstructionReceivedAttribute As String = "" 'AG 20/10/2010 - Last instruction received
        Private RecoveryResultsInPauseAttribute As Boolean = False 'AG 27/11/2013 - Task #1397


        ' XBC 08/11/2010 - SERVICE SOFTWARE
        Private myFwScriptsData As FwScripts
        Private myFwAdjustmentsDS As SRVAdjustmentsDS

        'SGM 26/01/2012 - SERVICE
        Private myISEInformationDS As ISEInformationDS

        ' PDT TO DELETE !!!
        'Private mySensorsDS As SRVSensorsDS
        'Private mySensorsDelegate As SRVSensorsDelegate
        ' PDT TO DELETE !!!

        Private myPhotometryDataAttr As New PhotometryDataTO

        ' PDT TO DELETE !!!
        'Private AbsorbanceScanDataAttr As New List(Of Double)
        ' PDT TO DELETE !!!

        Private myStressModeDataAttr As New StressDataTO

        '// Tiempo que se pone como límite en el que ya no se espera más a la ejecución de la Trama
        ' XBC 04/05/2011 - Is unused and is recovered as attribute with its public properties
        Private MaxWaitTimeAttr As Integer

        Private EndInstructionTimeAttr As Integer 'SGM 17/10/2011
#End Region

#Region "Properties"

        Public Property LastExecutionIDSent() As Integer '20/04/2010 AG
            Get
                Return lastExecutionIDSentAttribute
            End Get

            Set(ByVal value As Integer)
                lastExecutionIDSentAttribute = value
            End Set
        End Property

        Public Property LastPreparationInstructionSent() As String '20/04/2010 AG
            Get
                Return lastPreparationInstrucionSentAttribute
            End Get

            Set(ByVal value As String)
                lastPreparationInstrucionSentAttribute = value
            End Set
        End Property

        Public Property ConnectedPortName() As String '22/04/2010 AG
            Get
                Return connectedPortNameAttribute
            End Get

            Set(ByVal value As String)
                connectedPortNameAttribute = value
            End Set
        End Property

        Public Property ConnectedBauds() As String '22/04/2010 AG
            Get
                Return connectedBaudsAttribute
            End Get

            Set(ByVal value As String)
                connectedBaudsAttribute = value
            End Set
        End Property

        Public Property LastInstructionTypeSent() As GlobalEnumerates.AppLayerEventList  '27/04/2010 AG
            Get
                Return lastInstructionTypeSentAttribute
            End Get

            Set(ByVal value As GlobalEnumerates.AppLayerEventList)
                lastInstructionTypeSentAttribute = value
            End Set
        End Property

        Public Property InstructionSent() As String '20/10/2010 AG
            Get
                Return lastInstructionSentAttribute
            End Get

            Set(ByVal value As String)
                lastInstructionSentAttribute = value
            End Set
        End Property

        Public Property InstructionReceived() As String '20/10/2010 AG
            Get
                Return lastInstructionReceivedAttribute
            End Get

            Set(ByVal value As String)
                lastInstructionReceivedAttribute = value
            End Set
        End Property

        Public Property PhotometryData() As PhotometryDataTO
            Get
                Return myPhotometryDataAttr
            End Get
            Set(ByVal value As PhotometryDataTO)
                myPhotometryDataAttr = value
            End Set
        End Property

        Public Property StressModeData() As StressDataTO
            Get
                Return myStressModeDataAttr
            End Get
            Set(ByVal value As StressDataTO)
                myStressModeDataAttr = value
            End Set
        End Property

        ' XBC 12/05/2011
        'Private Property AbsorbanceScanData() As List(Of Double)
        '    Get
        '        Return AbsorbanceScanDataAttr
        '    End Get
        '    Set(ByVal value As List(Of Double))
        '        AbsorbanceScanDataAttr = value
        '    End Set
        'End Property


        Public Property MaxWaitTime() As Integer
            Get
                Return Me.MaxWaitTimeAttr
            End Get
            Set(ByVal value As Integer)
                Me.MaxWaitTimeAttr = value
            End Set
        End Property

        Public Property EndInstructionTime() As Integer
            Get
                Return Me.EndInstructionTimeAttr
            End Get
            Set(ByVal value As Integer)
                Me.EndInstructionTimeAttr = value
            End Set
        End Property

        Public WriteOnly Property currentAnalyzerID() As String '20/10/2010 AG
            'Get
            '    Return analyzerIDAttribute
            'End Get

            Set(ByVal value As String)
                analyzerIDAttribute = value
            End Set
        End Property

        Public WriteOnly Property currentWorkSessionID() As String 'AG 31/07/2012
            'Get
            '    Return worksessionIDAttribute
            'End Get

            Set(ByVal value As String)
                worksessionIDAttribute = value
            End Set
        End Property

        'AG 27/11/2013 - Task #1397
        Public Property RecoveryResultsInPause As Boolean
            Get
                Return RecoveryResultsInPauseAttribute
            End Get
            Set(value As Boolean)
                RecoveryResultsInPauseAttribute = value
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub New()
            'Adapt from iPRO (user)
            Try
                Dim myGlobalDataTO As New GlobalDataTO
                'TR 23/04/2010  -Get the USB Registry key value to our class variable 
                'USBRegistryKey.Add(ConfigurationManager.AppSettings("USBRegistryKey").ToString())
                'TR 25/01/2011 -Replace by corresponding value on global base.
                'USBRegistryKey.Add(GlobalBase.USBRegistryKey)

                'SGM 08/03/11 Get from SWParameters table
                Dim myUSBRegistryKey As String
                Dim myParams As New SwParametersDelegate
                myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.USB_REGISTRY_KEY.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myUSBRegistryKey = CStr(myGlobalDataTO.SetDatos)
                Else
                    myUSBRegistryKey = "SYSTEM\CURRENTCONTROLSET\ENUM\FTDIBUS"
                End If

                USBRegistryKey.Add(myUSBRegistryKey)



                If Link Is Nothing Then
                    Link = New LinkLayer
                End If
                'waitingTimeTimer.Enabled = False 'AG 07/05/2010 - Move to AnalyzerManager

                'the scripts data is loaded in the LoadAppScriptsData method
                '' XBC 08/11/2010 - SERVICE SOFTWARE
                'myScriptsData = New Scripts(True)
                '' XBC 08/11/2010 - SERVICE SOFTWARE

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.New", EventLogEntryType.Error, False)
            End Try

        End Sub
#End Region

#Region "Events definition & methods"
        'AG 20/04/2010 Events that relates ApplicationLayer with AnalyzerManager (owner)
        ''' <summary>
        ''' Call the AnalyzerManager owner class main function
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <remarks></remarks>
        Public Shared Event ManageAnalyzer(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList, ByVal pInstructionReceived As List(Of InstructionParameterTO))

        ''' <summary>
        ''' Event triggered when an instruction can not be sent
        ''' </summary>
        ''' <remarks>AG 14/10/2011</remarks>
        Public Shared Event SendDataFailed()

        ''' <summary>
        ''' Event for instruction reception from LinkLayer
        ''' </summary>
        ''' <param name="pEvent"></param>
        ''' <param name="in_datos"></param>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' </remarks>
        Public Sub OnActivateProtocolEvent(ByVal pEvent As GlobalEnumerates.AppLayerEventList, ByRef in_datos As String) Handles Link.ActivateProtocol
            Try

                ''Track SGM
                'Dim myTrackTime As Double = DateTime.Now.Subtract(Link.ResponseTrack).TotalMilliseconds
                'Dim myLogAcciones2 As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity("LinkLayer -- AppLayer " & myTrackTime.ToStringWithDecimals(0), "ApplicationLayer.OnActivateProtocolEvent", EventLogEntryType.Information, False)
                ''end Track

                'NOTES: 2on parameter is Sw entry information (nothing in case RECEIVE), 3rd parameter is Fw entry information
                Me.ActivateProtocol(pEvent, Nothing, in_datos)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.OnActivateProtocolEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "iPRO adaptation (GestorEstados.vb) - Public methods"

        ''' <summary>
        ''' Initialize the application Layer. To start using the Comunication Protocol 
        ''' this is the first call to be done. Initialize all the internal variables.
        ''' iPRO (GestorEstados.Iniciar)
        ''' </summary>
        ''' <param name="pPooling">Indicate if  work by pooling (true) or events(false)</param>
        ''' <returns>
        ''' Return true if every thing was ok false if wrong.
        ''' </returns>
        ''' <remarks>CREATE BY: TR 19/04/2010</remarks>
        Public Function Start(ByVal pPooling As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim starResult As Boolean = False
                starResult = Link.Start(pPooling)
                myGlobalDataTO.SetDatos = starResult

                'If Not starResult Then
                '    Link.Terminate()
                'End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.Start", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Initializes again the application Layer.  Initialize all the internal variables.
        ''' iPRO (GestorEstados.Iniciar)
        ''' </summary>
        ''' <returns>
        ''' Return true if every thing was ok false if wrong.
        ''' </returns>
        ''' <remarks>CREATE BY: SGM 03/07/2012</remarks>
        Public Function SynchronizeComm() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim restarResult As Boolean = False
                restarResult = Link.Synchronize()
                myGlobalDataTO.SetDatos = restarResult

                'If Not starResult Then
                '    Link.Terminate()
                'End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SynchronizeComm", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        Dim ConnectionData() As Byte = Nothing  ' XBC 18/06/2012 PROVA !!!


        ''' <summary>
        ''' Open the communication port base on the configuration(Automatic, Manual)
        ''' When the port is open try a connection with the instrument.
        ''' iPRO (GestorEstados.Abrir)
        ''' </summary>
        ''' <param name="pPort">Port</param>
        ''' <param name="pSpeed">Spee</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 20/04/2010
        ''' </remarks>
        Public Function Open(ByVal pPort As String, ByVal pSpeed As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myResult As Boolean = False
            Try
                Dim bauds As String = ""
                Dim final As Boolean = False
                Dim myPortsList As New List(Of String)
                'Dim iCount As Byte = 0
                'Const jCount As Byte = 0
                Dim currentPort As Integer = 0
                Dim myLastConnectePort As Integer = 0
                'Dim ConnectionData() As Byte = Nothing

                'TR 22/02/2012 -Variable used to indicate if connection mode is auto or manual.
                Dim ConnectionMode As String = String.Empty
                'TR 22/02/2012 -END.

                'AG 21/10/2010
                'If Not Link.Close() Then
                Link.Close()

                'Automatic connection (try all ports read)
                If String.Compare(pPort, "", False) = 0 Or pSpeed = "" Then
                    myGlobalDataTO = ReadRegistredPorts()

                    ConnectionMode = "AUTO" 'TR 22/02/2012 -Indicate Auto Connection. 

                    If Not myGlobalDataTO.HasError Then
                        'set the registre port values to the local variable.
                        myPortsList = DirectCast(myGlobalDataTO.SetDatos, List(Of String))
                    End If
                    'bauds = ConfigurationManager.AppSettings("PortSpeed").ToString()
                    'TR 25/01/2011 -Replace by corresponding value on global base.
                    bauds = GlobalBase.PortSpeed.ToString()

                    If myPortsList.Count > 0 Then 'AG 13/07/2010
                        'Order detected ports
                        Dim auxPortsList = myPortsList
                        myPortsList = (From a In auxPortsList Select a Order By a).ToList()
                        'search last conected port.
                        'If ConfigurationManager.AppSettings("LastPortConnected").ToString() <> "" Then
                        'TR 25/01/2011 -Replace by corresponding value on global base.
                        If String.Compare(GlobalBase.LastPortConnected, "", False) <> 0 Then
                            myLastConnectePort = CType(GlobalBase.LastPortConnected, Integer)
                            'Insert the last conected port on the first position on our ports list.
                            SetFirtsPosLastConnectedPort(myLastConnectePort, myPortsList)
                        End If
                    End If 'AG 13/07/2010

                Else
                    myPortsList.Add(pPort)
                    bauds = pSpeed

                    ConnectionMode = "MANUAL" 'TR 22/02/2012 -Indicate Manual Connection.
                End If

                If ConnectedPortNumber >= 0 Then
                    'TR 25/01/2011 -Replace by corresponding value on global base.
                    'If Not ConfigurationManager.AppSettings("LastPortConnected").ToString() = "" Then
                    If Not String.Compare(GlobalBase.LastPortConnected, "", False) = 0 Then
                        ConnectedPortNumber = CType(GlobalBase.LastPortConnected, Integer)
                    End If
                End If

                Dim myLogAcciones As New ApplicationLogManager() 'AG 28/10/2011
                If myPortsList.Count > 0 Then
                    For Each myPort As String In myPortsList
                        'currentPort = CType(myPort.Remove(0, 3), Integer)
                        'If currentPort > 9 Then
                        '    myPort = "\\.\COM" & currentPort
                        'End If

                        If Not myPort.Contains("\\.\COM") Then
                            currentPort = CType(myPort.Remove(0, 3), Integer)
                            If currentPort > 9 Then
                                myPort = "\\.\COM" & currentPort
                            End If
                        End If


                        If Link.Open(myPort) Then

                            While Not myResult
                                If Link.Config(bauds & ",N,8,1") Then
                                    Dim connectionString As String = ""
                                    ConnectionData = GenerateConnectionInstructionInBytes(connectionString)
                                    'Dim myLogAcciones As New ApplicationLogManager()
                                    GlobalBase.CreateLogActivity("Try connect [" & connectionString & "]", "ApplicationLayer.Open", EventLogEntryType.Information, False)

                                    If Link.StartComm(ConnectionData) Then
                                        myResult = True
                                        final = True

                                        'AG 22/04/2010
                                        lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.CONNECT
                                        connectedPortNameAttribute = myPort
                                        connectedBaudsAttribute = bauds

                                        If currentPort <> ConnectedPortNumber Then
                                            ConnectedPortNumber = currentPort
                                            'Save the current conected port if it's diferent than the previous.
                                        End If

                                        'AG 28/10/2011 - new log trace.
                                        GlobalBase.CreateLogActivity("Communication establishment using port: " & myPort & ". Connection Mode: " & ConnectionMode, _
                                                                        "ApplicationLayer.Open", EventLogEntryType.Information, False)
                                        Exit For
                                    Else
                                        'AG 28/10/2011 - new log trace.
                                        GlobalBase.CreateLogActivity("Communication FAILED using port: " & myPort & ". Connection Mode: " & ConnectionMode, _
                                                                        "ApplicationLayer.Open", EventLogEntryType.Information, False)
                                        Exit While
                                    End If

                                End If
                            End While

                            If Not myResult Then
                                Link.Close()
                            End If
                        End If
                    Next
                Else
                    myResult = False
                End If

                ''AG 28/10/2011
                'If Not myResult Then
                '    GlobalBase.CreateLogActivity("Communication establishment FAILED", "ApplicationLayer.Open", EventLogEntryType.Information, False)
                'End If
                ''AG 28/10/2011
                myPortsList = Nothing 'AG 02/08/2012 - free memory
                myGlobalDataTO.SetDatos = myResult

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.Open", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function



        ''' <summary>
        ''' Method incharge to get all the valids port for connection.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 23/04/2010
        ''' </remarks>
        Public Function ReadRegistredPorts() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get all the valids port on the current machine trhought the port list.
                myGlobalDataTO = SearchValidPorts(SerialPort.GetPortNames().ToList())


                If Not myGlobalDataTO.HasError Then
                    Dim MyPortList As New List(Of String)
                    MyPortList = DirectCast(AppDomain.CurrentDomain.GetData("PortList"), List(Of String))
                    'TR 23/04/2010
                    SetUSBPort(MyPortList)
                    SortPortList(MyPortList)
                    myGlobalDataTO.SetDatos = MyPortList    'AG 13/07/2010
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ReadRegistredPorts", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Release the objec from the Application layer
        ''' iPRO (GestorEstados.Terminar)
        ''' </summary>
        ''' <remarks>
        ''' CREATE BY: TR 21/04/2010
        ''' </remarks>
        Public Sub Terminate()
            Try
                'ExitAll = True
                'InitializeTimerControl(-1)

                Link.Terminate()

                If Not Link Is Nothing Then
                    Link = Nothing
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.Terminate", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Stop communications channel
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StopComm() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim stopResult As Boolean = False
                stopResult = Link.StopComm()
                myGlobalDataTO.SetDatos = stopResult

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.StopComm", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Start communications channel after a previous Stop
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StartComm() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim startResult As Boolean = False
                startResult = Link.StartComm(ConnectionData)
                myGlobalDataTO.SetDatos = startResult

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.StartComm", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "iPRO adaptation (GestorEstados.vb) - Private methods"

        ''' <summary>
        ''' Send the instruction in a byte array for not loosing any data wile reading.
        ''' Adapted from iPRO (GestorEstados.EnviarDatos)
        ''' </summary>
        ''' <param name="pInDatosByte"></param>
        ''' <param name="pRepetition"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 21/04/2010
        ''' AG 22/04/2010 define as private function
        ''' </remarks>
        Private Function SendData(ByRef pInDatosByte() As Byte, ByVal pRepetition As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myResult As Boolean = False
                FlagACKrebut = False
                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                    'this is for testing without been connected to the instrument.
                    myResult = True
                Else
                    myResult = Link.SendSynchronousByte(pInDatosByte)
                End If

                If pRepetition Then
                    'Activate the Timer frame
                    'InitializeTimerControl(MaxWaitTime) 'AG 07/05/2010 - To define


                    Do While Not FlagACKrebut
                        If FlagRetry And Not FlagACKrebut Then
                            FlagRetry = False
                            If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                                myResult = True
                            Else
                                myResult = Link.SendSynchronousByte(pInDatosByte)
                            End If
                        End If

                        'if flag error recived then generate the corresponding alarm.
                        If FlagERROR Then

                        End If

                        'Is needed in order the application continues working
                        Application.DoEvents()
                    Loop
                End If

                If Not myResult Then
                    'AG 14/10/2011 - if instruction can be sent
                    myGlobalDataTO.ErrorCode = "ERROR_COMM"
                    myGlobalDataTO.ErrorMessage = "ERROR_COMM"
                    'AG 14/10/2011
                End If

                myGlobalDataTO.SetDatos = myResult

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendData", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


        Private Sub SetFirtsPosLastConnectedPort(ByVal pLastConnectedPort As Integer, ByRef pPortsList As List(Of String))
            Dim myResult As New List(Of String)
            Try
                'Insert on first position.
                myResult.Add("COM" & pLastConnectedPort.ToString())
                pPortsList = (From a In pPortsList _
                             Where String.Compare(a, "COM" & pLastConnectedPort.ToString(), False) <> 0 _
                             Select a).ToList()

                myResult.AddRange(pPortsList)

                pPortsList = myResult
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SetFirtsPosLastConnectedPort", EventLogEntryType.Error, False)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Search the valid ports and excluding the invalid ones.
        ''' </summary>
        ''' <param name="pPorts">list of computer ports</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 21/04/2010.
        ''' </remarks>
        Private Function SearchValidPorts(ByVal pPorts As List(Of String)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Object to save the port list.
                'Get the list of invalid ports name from AppConfig.
                'Dim InvalidPortsName As String = ConfigurationManager.AppSettings("InvalidPortsList").ToString()
                'TR 25/01/2011 -Replace by corresponding value on global base.
                'Dim InvalidPortsName As String = GlobalBase.InvalidPortsList

                'SGM 08/03/11 Get from SWParameters table
                Dim InvalidPortsName As String
                Dim myParams As New SwParametersDelegate
                myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.INVALID_PORT_LIST.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    InvalidPortsName = CStr(myGlobalDataTO.SetDatos)
                Else
                    InvalidPortsName = "Internal Modem,External Modem,PCMCIA,BLUETOOTH"
                End If

                Dim validPortList As New List(Of String)

                'Dim qPort As New List(Of Management.ManagementObject)
                'search the post modem and virtual ports
                Dim InternalDev As ManagementObjectSearcher = _
                  New ManagementObjectSearcher("SELECT * FROM Win32_POTSModem")

                If InternalDev.Get.Count > 0 Then
                    'Go throught the list of devices.
                    For Each myManagObj As ManagementObject In InternalDev.Get
                        'add the device to the invalid ports name list.
                        InvalidPortsName &= "," & myManagObj.Properties.Item("DeviceType").Value.ToString()
                    Next
                End If
                InternalDev = New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity")
                If InternalDev.Get.Count > 0 Then
                    'go throught the list of devices.
                    For Each myManagObj As ManagementObject In InternalDev.Get
                        'validate the device with the list of ports.
                        For Each myPort As String In pPorts

                            If (Not myManagObj.Properties.Item("Name").Value Is Nothing) Then 'IT 10/10/2014: BA-2000
                                If myManagObj.Properties.Item("Name").Value.ToString().Contains(myPort) Then
                                    For Each myInvalidPortName As String In InvalidPortsName.Split(CChar(","))
                                        'validate if the Device contains the name of one of the invalid port name.
                                        If Not myManagObj.Properties.Item("DeviceID").Value.ToString().Contains(myInvalidPortName) Then
                                            'TR 21/02/2012 -Validate the port is not added on the list
                                            If validPortList.Where(Function(a) String.Compare(a, myPort, False) = 0).Count = 0 Then
                                                'add into the valid port list.
                                                validPortList.Add(myPort)
                                                Exit For
                                            End If
                                            'TR 21/02/2012 -END.
                                        End If
                                    Next
                                End If
                            End If
                        Next
                    Next
                End If

                'Set the port list into the domain.
                AppDomain.CurrentDomain.SetData("PortList", validPortList)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        Private Sub InitializeTimerControl(ByVal pInterval As Integer)
            Try
                'AG 07/05/2010 - To define
                'TimerControl.Interval = pInterval * 1000
                'TimerControl.Enabled = True
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.InitializeTimerControl", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' From the recived port list validate if 
        ''' there are any USB type and change the name 
        ''' from COM to USB Ex: COM5 Change to USB5
        ''' </summary>
        ''' <param name="PortsList">Port List</param>
        ''' <remarks>
        ''' CREATE BY: TR 23/04/2010
        ''' </remarks>
        Private Sub SetUSBPort(ByRef PortsList As List(Of String))
            Try
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myPortNumber As Integer = -1
                Dim myPortIndex As Integer = 0
                'Go throught each port on the port list
                For Each myPortName As String In PortsList
                    'Get the port number.
                    myPortNumber = GetPortNumbert(myPortName)
                    'Go throught each USB key on the USBReagisty list
                    'To validate the port number
                    For Each myUSBKey As String In USBRegistryKey
                        'Validate if the curren por number is a USB Type.
                        myGlobalDataTO = ValidateUSBPort(myPortNumber, myUSBKey)
                        If Not myGlobalDataTO.HasError Then
                            If DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
                                'If it's a USB type then change the name.
                                Dim newName As String = "USB" & myPortNumber
                                'Modify the name on the port list.
                                PortsList(myPortIndex) = newName
                            End If
                        End If
                    Next
                    myPortIndex += 1
                Next
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SetUSBPort", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Order the port list.
        ''' </summary>
        ''' <param name="PortsList">Port list</param>
        ''' <remarks>
        ''' CREATE BY: TR 23/04/2010
        ''' </remarks>
        Private Sub SortPortList(ByRef PortsList As List(Of String))
            Try
                Dim myHT As New Hashtable()
                'Dim qPortList As New List(Of String)
                Dim qObjList As New List(Of Object)
                For Each myPort As String In PortsList
                    myHT.Add(GetPortNumbert(myPort), myPort)
                Next


                qObjList = (From a In myHT.Keys Order By a Select a).ToList()
                If qObjList.Count > 0 Then
                    PortsList.Clear()
                    For Each myObjec As Object In qObjList
                        PortsList.Add(myHT(myObjec).ToString())
                    Next
                End If
                qObjList = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SortPortList", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Search on the Registry the port type to validate if it's a USB type
        ''' </summary>
        ''' <param name="pPortNumber">Port Number</param>
        ''' <param name="pRegKey">Registry Key to search the value </param>
        ''' <param name="pUSBKey">Current USB Key Value.</param>
        ''' <returns>True if the port Was found. Else False</returns>
        ''' <remarks>
        ''' CREATE BY: TR 23/04/2010.
        ''' </remarks>
        Private Function SearchOnRegistryKey(ByVal pPortNumber As Integer, ByVal pRegKey As RegistryKey, ByVal pUSBKey As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myResult As Boolean = False
                Dim myRegSubKey As RegistryKey

                If Not pRegKey Is Nothing Then
                    Dim PortNameValue As String = ""
                    Dim SubKeysValue As New List(Of String)
                    'Get the value (PortName) form the recived Registry Key
                    If Not pRegKey.GetValue("PortName") Is Nothing Then ' validate do not return a null value 
                        PortNameValue = pRegKey.GetValue("PortName").ToString()
                        If GetPortNumbert(PortNameValue) = pPortNumber Then
                            myResult = True
                        End If
                    Else ' if not value was return then search on the other sub keys.
                        'Get all the subkeys name. from the current Registry Key
                        SubKeysValue = pRegKey.GetSubKeyNames().ToList()
                        Dim NewRegKey As String = ""
                        'Go throught each value on the subkey values.
                        For Each mySubKeyValue As String In SubKeysValue
                            'create the new Registry key base on the new subKeyValue
                            NewRegKey = (pUSBKey & "\" & mySubKeyValue).Trim()
                            'open the subkey

                            ' DL 10/09/2010 {
                            'myRegSubKey = Registry.LocalMachine.OpenSubKey(NewRegKey) ' original

                            Dim myRegKey As RegistryKey = Registry.CurrentUser
                            myRegSubKey = myRegKey.OpenSubKey(NewRegKey, False)

                            ' DL 10/09/2010 }

                            If Not myRegSubKey Is Nothing Then ' validate do no return a null value.
                                'Recurcivity: If a value was found then this method call it self to search the new 
                                'Registry Key Value.

                                myGlobalDataTO = SearchOnRegistryKey(pPortNumber, myRegSubKey, NewRegKey)
                                If Not myGlobalDataTO.HasError Then
                                    myResult = DirectCast(myGlobalDataTO.SetDatos, Boolean)
                                    'validate if the returned value was true to break the Recurcivity.
                                    If myResult Then
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
                myGlobalDataTO.SetDatos = myResult
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchOnRegistryKey", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Validate if the recived port is a USB Type Port.
        ''' </summary>
        ''' <param name="pPortNumber">Port Number</param>
        ''' <param name="pUSBKey">Current USB Key</param>
        ''' <returns>
        ''' True if recived port a USB type. Else False
        ''' </returns>
        ''' <remarks>
        ''' CREATE BY: TR 23/04/2010
        ''' </remarks>
        Private Function ValidateUSBPort(ByVal pPortNumber As Integer, ByVal pUSBKey As String) As GlobalDataTO
            Dim myGlobalDataTo As New GlobalDataTO
            Try
                Dim myResult As Boolean = False
                Dim RegKey As RegistryKey
                Dim strKeyName As String

                If String.Compare(pUSBKey, "", False) <> 0 Then
                    strKeyName = pUSBKey.Trim()
                    'Get the Registry key values for the current KeyName
                    RegKey = Registry.LocalMachine.OpenSubKey(strKeyName)
                    ' Search on the registry the values for the recived port number and USB Key.
                    myGlobalDataTo = SearchOnRegistryKey(pPortNumber, RegKey, pUSBKey)
                    If Not myGlobalDataTo.HasError Then
                        If DirectCast(myGlobalDataTo.SetDatos, Boolean) Then
                            'if Found set the value True to the result.
                            myResult = True
                        End If
                    End If
                End If
                myGlobalDataTo.SetDatos = myResult

            Catch ex As Exception
                myGlobalDataTo.HasError = True
                myGlobalDataTo.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ValidateUSBPort", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTo
        End Function

        ''' <summary>
        ''' In the Resgistred port indentificate the USB and the COM Port
        ''' this method is incomplete need to implement more functionality.
        ''' </summary>
        ''' <param name="strPuertos"></param>
        ''' <param name="pUSBKey"></param>
        ''' <remarks></remarks>
        Private Sub DiferenciarEntreCOMyUSB(ByRef strPuertos() As String, ByVal pUSBKey As String)
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Dim i As Integer
                'Dim intPort As Integer
                'Dim idRegKey As Integer
                'Dim blnUSB As Boolean
                'Dim strTmp As String

                'If IsNothing(pUSBKey) Then Exit Sub '// Si no hay claves parametrizadas considerarlo todo COM

                ''// Diferenciar entre COM's y USB's
                'For i = 0 To UBound(strPuertos)
                '    blnUSB = False
                '    strTmp = strPuertos(i)
                '    intPort = ObtenerPuerto(strTmp)

                '    For idRegKey = 0 To pUSBKey.Length - 1
                '        If pUSBKey(intPort, idRegKey) Then blnUSB = True
                '        If blnUSB = True Then Exit For
                '    Next idRegKey

                '    If blnUSB Then strPuertos(i) = "USB" & intPort
                'Next i
                'TODO: IMPLEMENT FUNCTION 
                ' ComprobarSiPuertosUSB.
                ' BuscarEnClave.
                ' ObtenerPuerto.

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.Open", EventLogEntryType.Error, False)
            End Try
        End Sub

        Private Function GetPortNumbert(ByVal pPort As String) As Integer
            Dim myPortNumber As Integer = 0
            Try
                If pPort.Length > 0 Then
                    If IsNumeric(pPort.Remove(0, 3)) Then
                        myPortNumber = CType(pPort.Remove(0, 3), Integer)
                    Else
                        myPortNumber = -1
                    End If
                Else
                    myPortNumber = -1
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetPortNumbert", EventLogEntryType.Error, False)
                Throw ex
            End Try

            Return myPortNumber
        End Function

#End Region

#Region "iPRO adaptation (GestorEstados.vb - Commented functions in Ax00"

        'AG 22/04/2010 - commented
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pPort"></param>
        '''' <param name="pSpeed"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATE BY: TR 21/04/2010
        '''' AG 22/04/2010 define as Privte function
        '''' </remarks>
        'Private Function Connect(Optional ByVal pPort As String = "", Optional ByVal pSpeed As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        'Dim myWaytTime As New DateTime
        '        'WaitRequestState = True
        '        'myWaytTime = DateAdd(DateInterval.Second, 1, Date.Now())

        '        'Start the ports connections
        '        myGlobalDataTO = ManageConnection(pPort, pSpeed)
        '        If myGlobalDataTO.HasError Then
        '            myGlobalDataTO.SetDatos = False
        '        Else                    
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
        '        myGlobalDataTO.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.Connect", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function


        'AG 22/04/2010 - commented
        '''' <summary>
        '''' Manage the correct instrument conection initialization 
        '''' </summary>
        '''' <param name="pPort"></param>
        '''' <param name="pSpeed"></param>
        '''' <returns></returns>
        '''' <remarks>CREATE BY: TR 21/04/2010</remarks>
        'Private Function ManageConnection(Optional ByVal pPort As String = "", Optional ByVal pSpeed As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try

        '        'validate if it's conectec to the instrument.
        '        If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
        '            myGlobalDataTO.SetDatos = True
        '        Else
        '            'Try to open the Port
        '            myGlobalDataTO = Open(pPort, pSpeed)
        '            If Not myGlobalDataTO.HasError Then
        '                'validate the operation result.
        '                If Not DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
        '                    myGlobalDataTO.HasError = True
        '                    myGlobalDataTO.ErrorCode = "OPEN_PORT_ERROR"
        '                    Exit Try
        '                End If
        '                'Do the first conection to the instrument.
        '                myGlobalDataTO = ActivateProtocol(ClassEventList.CONNECT)

        '                If myGlobalDataTO.HasError Then
        '                    myGlobalDataTO = New GlobalDataTO
        '                    myGlobalDataTO.HasError = True
        '                    myGlobalDataTO.ErrorCode = "CONECTION_FAIL"
        '                    Exit Try
        '                End If

        '            End If
        '        End If

        '        'TODO: RaiseEvent MonitorEstados() -See IPRO GestorEstados.GestionarConexion

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
        '        myGlobalDataTO.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ManageConnection", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region


        '#Region "ISE Command Structure"

        '        ''' <summary>
        '        ''' ISE command structure
        '        ''' </summary>
        '        ''' <remarks>Created by SGM 12/12/2011</remarks>
        '        Public Structure ISECommandStruct

        '            Public ISEMode As GlobalEnumerates.ISEModes
        '            Public ISECommand As GlobalEnumerates.ISECommands
        '            Public P1 As Integer
        '            Public P2 As Integer
        '            Public P3 As Integer
        '            Public SampleTubePos As Integer
        '            Public SampleTubeType As GlobalEnumerates.ISESampleTubeTypes
        '            Public SampleRotorType As Integer
        '            Public SampleVolume As Integer 'microlitres


        '            'Public Function ToStringCommand() As GlobalDataTO

        '            '    Dim myGlobal As New GlobalDataTO
        '            '    Dim myString As String = ""

        '            '    Try
        '            '        myString &= "M:" & CInt(ISEMode).ToString & ";"
        '            '        myString &= "CMD:" & CInt(ISECommand).ToString & ";"
        '            '        myString &= "P1:" & P1.ToString & ";"
        '            '        myString &= "P2:" & P2.ToString & ";"
        '            '        myString &= "P3:" & P3.ToString & ";"
        '            '        myString &= "M1:" & SampleTubePos.ToString & ";"
        '            '        myString &= "TM1:" & CInt(SampleTubeType).ToString & ";"
        '            '        myString &= "RM1:" & SampleRotorType.ToString & ";"
        '            '        myString &= "VM1:" & SampleVolume.ToString & ";"

        '            '        myGlobal.SetDatos = myString

        '            '    Catch ex As Exception
        '            '        myGlobal.HasError = True
        '            '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '            '        myGlobal.ErrorMessage = ex.Message
        '            '    End Try

        '            '    Return myGlobal
        '            'End Function
        '        End Structure

        '#End Region


#Region "Public methods"

        ''' <summary>
        ''' Main function in application layer class
        ''' 
        ''' When Software wants to send something to the analyzer use this method
        ''' When Firmware sends something an event raise this method how inform the Software level layers about it
        ''' </summary>
        ''' <param name="pEvent"></param>
        ''' <param name="pSwEntry"></param>
        ''' <param name="pFwEntry"></param>
        ''' <param name="pFwScriptID">Identifier of the script/action to be sended</param>
        ''' <param name="pParams">param values to script/action to be sended</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation    AG 19/04/2010
        ''' Modified by XB 08/11/2010 - SERVICE SOFTWARE - Add optional field pScriptID
        '''             XB 18/11/2010 - SERVICE SOFTWARE - Add optional field pParams
        '''             XB 10/10/2013 - Add PAUSE Instruction - BT #1317
        ''' </remarks>
        Public Function ActivateProtocol(ByVal pEvent As GlobalEnumerates.AppLayerEventList, Optional ByVal pSwEntry As Object = Nothing, _
                                          Optional ByVal pFwEntry As String = "", _
                                          Optional ByVal pFwScriptID As String = "", _
                                          Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Select Case pEvent
                    'AG 06/05/2010 - Sw wants to send a short instructions
                    'Case ClassEventList.SHORTINSTRUCTION
                    '    Dim myShortEvent As ClassEventList
                    '    myShortEvent = DirectCast(pSwEntry, ClassEventList)
                    '    myGlobal = Me.SendShortInstruction(myShortEvent)
                    '    Exit Select

                    ' XBC 18/10/2011 - SOUND & ENDSOUND Instructions are placed in a specific case
                    'Case GlobalEnumerates.AppLayerEventList.CONNECT, GlobalEnumerates.AppLayerEventList.SLEEP, _
                    '     GlobalEnumerates.AppLayerEventList.RUNNING, GlobalEnumerates.AppLayerEventList.ENDRUN, _
                    '     GlobalEnumerates.AppLayerEventList.ABORT, GlobalEnumerates.AppLayerEventList.RESRECOVER, _
                    '     GlobalEnumerates.AppLayerEventList.STATE, GlobalEnumerates.AppLayerEventList.SOUND, _
                    '     GlobalEnumerates.AppLayerEventList.ENDSOUND, GlobalEnumerates.AppLayerEventList.START, _
                    '     GlobalEnumerates.AppLayerEventList.SKIP, GlobalEnumerates.AppLayerEventList.STANDBY, _
                    '    GlobalEnumerates.AppLayerEventList.NROTOR

                    Case GlobalEnumerates.AppLayerEventList.CONNECT, GlobalEnumerates.AppLayerEventList.SLEEP, _
                         GlobalEnumerates.AppLayerEventList.RUNNING, GlobalEnumerates.AppLayerEventList.ENDRUN, _
                         GlobalEnumerates.AppLayerEventList.ABORT, GlobalEnumerates.AppLayerEventList.RESRECOVER, _
                         GlobalEnumerates.AppLayerEventList.STATE, GlobalEnumerates.AppLayerEventList.START, _
                         GlobalEnumerates.AppLayerEventList.SKIP, GlobalEnumerates.AppLayerEventList.STANDBY, _
                         GlobalEnumerates.AppLayerEventList.NROTOR, GlobalEnumerates.AppLayerEventList.RECOVER, _
                         GlobalEnumerates.AppLayerEventList.PAUSE

                        myGlobal = Me.SendShortInstruction(pEvent)
                        Exit Select

                        'Sw wants to send the next preparation
                        'AG 17/01/2011 - adapt for PREP_STD & PREP_ISE
                    Case GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION
                        Dim executionToPrepare As Integer = 0
                        If IsNumeric(pSwEntry) Then
                            executionToPrepare = DirectCast(pSwEntry, Integer)
                            Dim myType As String = "PREP_STD"
                            If pEvent = GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION Then myType = "PREP_ISE"

                            myGlobal = Me.SendNextPreparation(executionToPrepare, myType)
                        Else
                            myGlobal.HasError = True
                            myGlobal.ErrorCode = "WRONG_DATATYPE"
                        End If
                        Exit Select


                        'AG 17/01/2011 - Wash running
                    Case GlobalEnumerates.AppLayerEventList.WASH_RUNNING
                        If Not pSwEntry Is Nothing Then
                            Dim washToPerformDS As New AnalyzerManagerDS
                            washToPerformDS = DirectCast(pSwEntry, AnalyzerManagerDS)
                            myGlobal = Me.SendRunningWashInstruction(washToPerformDS)
                        End If
                        Exit Select
                        'AG 17/01/2011

                    Case GlobalEnumerates.AppLayerEventList.WASH 'StandBy Wash
                        'AG 02/03/2011 - By now implement only the conditioning case. When the wash utility has defined
                        'probably an specific parameter will be required
                        'Dim washDS As New WashInstructionDS
                        'washDS = DirectCast(pSwEntry, WashInstructionDS)
                        'myGlobal = Me.SendStandByWashInstruction(washDS)
                        myGlobal = Me.SendStandByWashInstruction()
                        Exit Select

                        'AG 18/05/2010 - Send a adjustment of IT and DAC
                    Case GlobalEnumerates.AppLayerEventList.ALIGHT
                        'myGlobal = Me.SendShortInstruction(pEvent)
                        'TR 03/03/2011 - The Well information is on the pSwEntry.
                        If IsNumeric(pSwEntry) Then
                            myGlobal = Me.SendALIGHTInstruction(CInt(pSwEntry))
                        ElseIf Not pParams Is Nothing Then
                            ' XBC 20/02/2012
                            myGlobal = Me.SendALIGHTInstruction(pParams)
                        End If
                        'TR 03/03/2011 -END.
                        Exit Select

                    Case GlobalEnumerates.AppLayerEventList.INFO
                        If Not pSwEntry Is Nothing Then
                            myGlobal = Me.SendINFOInstruction(CInt(pSwEntry).ToString) 'AG 11/12/2012 - add use 'CInt(pSwEntry).ToString' instead of pSwEntry.ToString
                        End If
                        Exit Select

                    Case GlobalEnumerates.AppLayerEventList.WSCTRL
                        If Not pSwEntry Is Nothing Then
                            myGlobal = Me.SendWSCTRLInstruction(CInt(pSwEntry))
                        End If
                        Exit Select


                        'AG 21/06/2011 - Codebar 
                    Case GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST
                        If Not pSwEntry Is Nothing Then
                            Dim barcodeDS As New AnalyzerManagerDS
                            barcodeDS = DirectCast(pSwEntry, AnalyzerManagerDS)
                            myGlobal = Me.SendCodeBarInstruction(barcodeDS)
                        End If
                        Exit Select
                        'AG 21/06/2011

                        ' XBC 18/10/2011
                    Case GlobalEnumerates.AppLayerEventList.SOUND
                        myGlobal = Me.SendSOUNDInstruction(True)
                        Exit Select

                        ' XBC 18/10/2011
                    Case GlobalEnumerates.AppLayerEventList.ENDSOUND
                        myGlobal = Me.SendSOUNDInstruction(False)
                        Exit Select

                        'AG 23/11/2011
                    Case GlobalEnumerates.AppLayerEventList.CONFIG
                        If Not pSwEntry Is Nothing Then
                            Dim settingsDS As New AnalyzerSettingsDS
                            settingsDS = DirectCast(pSwEntry, AnalyzerSettingsDS)
                            myGlobal = SendCONFIGInstruction(settingsDS)
                        End If

                        Exit Select


                        ' XBC 03/05/2011
                    Case GlobalEnumerates.AppLayerEventList.READADJ
                        myGlobal = Me.SendREADADJInstruction(pSwEntry.ToString)
                        Exit Select


                        ' SGM 12/12/2011
                    Case GlobalEnumerates.AppLayerEventList.ISE_CMD
                        Dim myISECommand As ISECommandTO = CType(pSwEntry, ISECommandTO)
                        myGlobal = Me.SendISECMDInstruction(myISECommand)
                        Exit Select

                        ' SGM 29/05/2012
                    Case GlobalEnumerates.AppLayerEventList.FW_UTIL
                        Dim myFWUtil As FWUpdateRequestTO = CType(pSwEntry, FWUpdateRequestTO)
                        myGlobal = Me.SendFWUTILInstruction(myFWUtil)
                        Exit Select

                    Case GlobalEnumerates.AppLayerEventList.POLLRD 'AG 31/07/2012
                        If Not pSwEntry Is Nothing AndAlso IsNumeric(pSwEntry) Then
                            myGlobal = Me.SendPOLLRDInstruction(CType(pSwEntry, Integer))
                        End If
                        Exit Select

                        '
                        ' SERVICE SOFTWARE-----------------------------------------------------------------------------------------------------------------------
                        ' 

                        ' XBC 08/11/2010
                    Case GlobalEnumerates.AppLayerEventList.COMMAND
                        ' XBC 20/09/2011
                        If pSwEntry Is Nothing Then
                            myGlobal = Me.SendCOMMANDInstruction(pFwScriptID, pParams)
                        Else
                            Dim pInstructions As New List(Of InstructionTO)
                            pInstructions = DirectCast(pSwEntry, List(Of InstructionTO))
                            myGlobal = Me.SendCOMMANDInstructionTest(pInstructions, pParams)
                        End If
                        ' XBC 20/09/2011
                        Exit Select



                        ' XBC 03/05/2011
                    Case GlobalEnumerates.AppLayerEventList.LOADADJ
                        myGlobal = Me.SendLOADADJInstruction(pSwEntry.ToString)
                        Exit Select

                        ' XBC 20/04/2011
                    Case GlobalEnumerates.AppLayerEventList.BLIGHT
                        myGlobal = Me.SendBLIGHTInstruction(pParams)
                        Exit Select

                        ' XBC 18/05/2011
                    Case GlobalEnumerates.AppLayerEventList.TANKSTEST
                        myGlobal = Me.SendTANKSTESTInstruction(pSwEntry.ToString)
                        Exit Select

                        ' XBC 23/04/2011
                    Case GlobalEnumerates.AppLayerEventList.SDMODE
                        myGlobal = Me.SendSDMODEInstruction(pParams)
                        Exit Select

                        ' XBC 23/04/2011
                    Case GlobalEnumerates.AppLayerEventList.SDPOLL
                        myGlobal = Me.SendSDPOLLInstruction()
                        Exit Select

                        ' XBC 25/05/2011
                    Case GlobalEnumerates.AppLayerEventList.POLLFW
                        Dim queryMode As GlobalEnumerates.POLL_IDs
                        If Not pSwEntry Is Nothing Then
                            queryMode = CType(pSwEntry, GlobalEnumerates.POLL_IDs)
                        End If
                        myGlobal = Me.SendPOLLFWInstruction(queryMode)
                        Exit Select


                        'Analyzer new instruction response RECEIVED
                    Case GlobalEnumerates.AppLayerEventList.RECEIVE
                        If String.Compare(pFwEntry, "", False) <> 0 Then
                            lastInstructionReceivedAttribute = pFwEntry 'AG 20/10/2010

                            Dim myLax00 As New LAX00Interpreter
                            Dim myParameterList As New List(Of ParametersTO)

                            'AG 04/10/2011 - Log all received instruction but the ANSINF who is logged only when generates new alarms (true, false)
                            ''AG 19/04/2011 - Log all received instructions in the same way as we log all send instructions (SendGenericalInstructions - XBC 08/11/2010)
                            'Dim myLogAcciones As New ApplicationLogManager()
                            'GlobalBase.CreateLogActivity("Received Instruction [" & pFwEntry.ToString & "]", "ApplicationLayer.ActivateProtocol (case RECEIVE)", EventLogEntryType.Information, False)

                            If Not pFwEntry.Contains(GlobalEnumerates.AppLayerInstrucionReception.ANSINF.ToString) Then
                                Dim myLogAcciones As New ApplicationLogManager()
                                GlobalBase.CreateLogActivity("Received Instruction [" & pFwEntry.ToString & "]", "ApplicationLayer.ActivateProtocol (case RECEIVE)", EventLogEntryType.Information, False)
                            End If
                            'AG 04/10/2011

                            'SGM 30/10/2012 - prevent possible checksum ";" inside ANSISE data
                            If pFwEntry.Contains(GlobalEnumerates.AppLayerInstrucionReception.ANSISE.ToString) Then
                                If pFwEntry.EndsWith(";>;") Then
                                    pFwEntry = pFwEntry.Replace(";>;", "*>;")
                                End If
                            End If

                            myGlobal = myLax00.Read(pFwEntry)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myParameterList = CType(myGlobal.SetDatos, List(Of ParametersTO))
                                myGlobal = ProcessResponse(myParameterList)
                                'TR 18/10/2011 -Commented exit try not needed.
                                'Else
                                '    Exit Try
                            End If
                        End If

                        Exit Select

                        ' XBC 31/05/2011
                    Case GlobalEnumerates.AppLayerEventList.POLLHW
                        Dim queryMode As GlobalEnumerates.POLL_IDs
                        If Not pSwEntry Is Nothing Then
                            queryMode = CType(pSwEntry, GlobalEnumerates.POLL_IDs)
                        End If
                        myGlobal = Me.SendPOLLHWInstruction(queryMode)
                        Exit Select

                        'SGM 10/06/2011
                    Case GlobalEnumerates.AppLayerEventList.ENABLE_EVENTS
                        myGlobal = Me.SendENABLEEVENTSInstruction()
                        Exit Select

                        'SGM 10/06/2011
                    Case GlobalEnumerates.AppLayerEventList.DISABLE_EVENTS
                        myGlobal = Me.SendDISABLEEVENTSInstruction()
                        Exit Select

                        'SGM 01/07/2011
                    Case GlobalEnumerates.AppLayerEventList.RESET_ANALYZER
                        myGlobal = Me.SendRESETInstruction
                        Exit Select

                        'SGM 01/07/2011
                    Case GlobalEnumerates.AppLayerEventList.LOADFACTORYADJ
                        myGlobal = Me.SendLOADFACTORYADJInstruction
                        Exit Select

                        'SGM 05/07/2011
                    Case GlobalEnumerates.AppLayerEventList.UPDATE_FIRMWARE
                        Dim queryMode() As Byte = Nothing
                        If Not pSwEntry Is Nothing Then
                            queryMode = CType(pSwEntry, Byte())
                        End If
                        myGlobal = Me.SendUPDATEFWInstruction(queryMode)
                        Exit Select

                        ' SGM 27/07/2011
                    Case GlobalEnumerates.AppLayerEventList.READCYC
                        myGlobal = Me.PDT_SendREADCYCLESInstruction(pSwEntry.ToString)
                        Exit Select

                        ' SGM 27/07/2011
                    Case GlobalEnumerates.AppLayerEventList.WRITECYC
                        myGlobal = Me.PDT_SendWRITECYCLESInstruction(pSwEntry.ToString)
                        Exit Select

                        ' XBC 04/06/2012
                    Case GlobalEnumerates.AppLayerEventList.UTIL
                        Dim myUtilCommand As UTILCommandTO = CType(pSwEntry, UTILCommandTO)
                        myGlobal = Me.SendUTILInstruction(myUtilCommand)
                        Exit Select

                        ' XBC 30/07/2012
                    Case GlobalEnumerates.AppLayerEventList.POLLSN
                        myGlobal = Me.SendPOLLSNInstruction()
                        Exit Select

                        'AG 01/03/2011 - removed instructions
                        'Case GlobalEnumerates.AppLayerEventList.BLIGHT, GlobalEnumerates.AppLayerEventList.DLIGHT
                        '    Dim myWell As Integer = 0
                        '    If IsNumeric(pSwEntry) Then
                        '        myWell = DirectCast(pSwEntry, Integer)
                        '        myGlobal = Me.SendBaseLineWithoutAdjustmentInstruction(pEvent, myWell) 'use SendShortInstruction as example!!!
                        '    Else
                        '        myGlobal.HasError = True
                        '        myGlobal.ErrorCode = "WRONG_DATATYPE"
                        '    End If
                        '    Exit Select
                        'END AG 18/05/2010

                        '    'SGM 01/04/11 SENSORS
                        'Case GlobalEnumerates.AppLayerEventList.SENSORS_RECEIVE
                        '    lastInstructionReceivedAttribute = pFwEntry

                        '    Dim myLax00 As New LAX00Interpreter
                        '    Dim myParameterList As New List(Of ParametersTO)

                        '    myGlobal = myLax00.Read(pFwEntry)
                        '    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        '        myParameterList = CType(myGlobal.SetDatos, List(Of ParametersTO))
                        '        myGlobal = ProcessResponse(myParameterList)
                        '    Else
                        '        Exit Try
                        '    End If
                        '    Exit Select

                End Select

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ActivateProtocol", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns Scripts Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Scripts</returns>
        ''' <remarks>Created by XBC 08/11/2010</remarks>
        Public Function ReadFwScriptData() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal.SetDatos = myFwScriptsData.FwScriptsData

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ReadFwScriptData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns FW Adjustments DS
        ''' </summary>
        ''' <returns>DS with the FW Adjustments</returns>
        ''' <remarks>Created by SGM 26/01/11</remarks>
        Public Function ReadFwAdjustmentsDS() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal.SetDatos = myFwAdjustmentsDS

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE - Updates FW Adjustments DS
        ''' </summary>
        ''' <remarks>Created by SG 26/01/11</remarks>
        Public Function UpdateFwAdjustmentsDS(ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If pAdjustmentsDS IsNot Nothing Then
                    myFwAdjustmentsDS = pAdjustmentsDS
                    myGlobal.SetDatos = pAdjustmentsDS
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.UpdateFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns ISE Info DS
        ''' </summary>
        ''' <returns>DS with the ISE Info DS</returns>
        ''' <remarks>Created by SGM 26/01/12</remarks>
        Public Function ReadISEInfoDS() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal.SetDatos = myISEInformationDS

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ReadISEInfoDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE - Updates ISE Info DS
        ''' </summary>
        ''' <remarks>Created by SG 26/01/12</remarks>
        Public Function UpdateISEInfoDS(ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If pISEInfoDS IsNot Nothing Then
                    myISEInformationDS = pISEInfoDS
                    myGlobal.SetDatos = pISEInfoDS
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.UpdateISEInfoDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#Region "TO DELETE"
        '''' <summary>
        '''' SERVICE - Updates Sensors data
        '''' </summary>
        '''' <remarks>Created by SG 26/01/11</remarks>
        'Public Function UpdateSensorsData(ByVal pSensors As String, ByVal pValues As String) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        Dim myNewSensorData As New SRVSensorsDS
        '        myGlobal = MyClass.mySensorsDelegate.ConvertReceivedDataToDS(pSensors, pValues)
        '        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
        '            myNewSensorData = CType(myGlobal.SetDatos, SRVSensorsDS)
        '            For Each N As SRVSensorsDS.srv_tfmwSensorsRow In myNewSensorData.srv_tfmwSensors.Rows
        '                For Each R As SRVSensorsDS.srv_tfmwSensorsRow In MyClass.mySensorsDS.srv_tfmwSensors.Rows
        '                    If R.GroupId.ToUpper = N.GroupId.ToUpper Then
        '                        If R.SensorId.ToUpper = N.SensorId.ToUpper Then
        '                            If Not N.IsValueNull And Not N.IsTimeStampNull Then
        '                                R.Value = N.Value
        '                                R.TimeStamp = N.TimeStamp
        '                            End If
        '                        End If
        '                    End If
        '                Next
        '            Next
        '            MyClass.mySensorsDS.AcceptChanges()
        '        End If

        '        myGlobal.SetDatos = MyClass.mySensorsDS

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.UpdateSensorsData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        ' XBC 12/05/2011
        '''' <summary>
        '''' SERVICE - Converets the data as string to list
        '''' </summary>
        '''' <param name="pData"></param>
        '''' <remarks></remarks>
        'Public Function LoadAbsorbanceScanData(ByVal pData As String) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        If pData.Length > 0 Then
        '            Dim myAbsorbance As New List(Of Double)
        '            Dim myData As String() = pData.Split(CChar("|"))
        '            For c As Integer = 0 To myData.Length - 1
        '                myAbsorbance.Add(CDbl(myData(c).Replace(".", ",")))
        '            Next

        '            myGlobal.SetDatos = myAbsorbance

        '            MyClass.AbsorbanceScanDataAttr = myAbsorbance
        '        Else
        '            myGlobal.HasError = True
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.LoadAbsorbanceData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function


        ' XBC 12/05/2011
        '''' <summary>
        '''' SERVICE - Gets the absorbance scan data
        '''' </summary>
        '''' <remarks></remarks>
        'Public Function GetAbsorbanceScanData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try

        '        myGlobal.SetDatos = MyClass.AbsorbanceScanData

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetAbsorbanceScanData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        ' XBC 12/05/2011
        '''' <summary>
        '''' SERVICE - Gets the absorbance scan data
        '''' </summary>
        '''' <remarks></remarks>
        'Public Function ResetAbsorbanceScanData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try

        '        MyClass.AbsorbanceScanDataAttr.Clear()

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ResetAbsorbanceScanData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function
#End Region

        ''' <summary>
        ''' SERVICE SOFTWARE
        ''' Loads to the Application the Global Scripts Data from the Xml File
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 24/11/10</remarks>
        Public Function LoadAppFwScriptsData() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myFwScriptsData = New FwScripts(True)

                If myFwScriptsData.FwScriptsDataErrorCode.Length > 0 Then
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = myFwScriptsData.FwScriptsDataErrorCode
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.LoadAppFwScriptsData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Loads the Adjustments Master data stored in the database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 27/01/11
        ''' Modified by SGM : 24/11/2011 - Get Master data from resources if SimulationMode
        '''</remarks>
        Public Function LoadFwAdjustmentsMasterData(ByVal pAnalyzerID As String, ByVal pSimulationMode As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                myGlobal = myAdjustmentsDelegate.ReadAdjustmentsFromDB(Nothing, pAnalyzerID)
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then

                    myFwAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                    With myFwAdjustmentsDS
                        .AnalyzerModel = "Simulated Analyzer"
                        .FirmwareVersion = "Simulated Firmware"
                        .ReadedDatetime = Nothing
                    End With

                    ''SGM 21/09/2011
                    ''Get from file next
                    'Dim myData As String = ""
                    'Dim myGlobalbase As New GlobalBase
                    'Dim objReader As System.IO.StreamReader
                    'Dim path As String = System.Windows.Forms.Application.StartupPath & GlobalBase.FwAdjustmentsFile
                    'objReader = New System.IO.StreamReader(path)
                    'myData = objReader.ReadToEnd()
                    'objReader.Close()

                    'SGM 24/11/2011
                    If pSimulationMode Then

                        'get from project resources the adjustments master data values
                        Dim myData As String = My.Resources.AdjustmentsMasterData


                        Dim myFileAdjustmentsDS As SRVAdjustmentsDS = Nothing
                        Dim myFwAdjustmentsDelegate As New FwAdjustmentsDelegate(myFwAdjustmentsDS)
                        myGlobal = myFwAdjustmentsDelegate.ConvertReceivedDataToDS(myData, "Simulated Analyzer", "Simulated Firmware")

                        If myGlobal.SetDatos IsNot Nothing And Not myGlobal.HasError Then
                            myFileAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                        End If

                        'if any of the adjustments in DB is empty fill with the existing in file
                        Dim isChanged As Boolean = False
                        If myFwAdjustmentsDS IsNot Nothing Then
                            If myFileAdjustmentsDS IsNot Nothing Then
                                For Each D As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myFwAdjustmentsDS.srv_tfmwAdjustments.Rows
                                    If D.Value.Trim.Length = 0 Then
                                        For Each F As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myFileAdjustmentsDS.srv_tfmwAdjustments.Rows
                                            If String.Compare(F.CodeFw.ToUpper.Trim, D.CodeFw.ToUpper.Trim, False) = 0 Then
                                                myFwAdjustmentsDS.srv_tfmwAdjustments.BeginInit()
                                                D.Value = F.Value.Trim
                                                myFwAdjustmentsDS.srv_tfmwAdjustments.EndInit()
                                                myFwAdjustmentsDS.AcceptChanges()
                                                isChanged = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                Next D
                                For Each D As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myFwAdjustmentsDS.srv_tfmwAdjustments.Rows
                                    If D.Value.Trim.Length = 0 Then
                                        myFwAdjustmentsDS.srv_tfmwAdjustments.BeginInit()
                                        D.Value = "0"
                                        myFwAdjustmentsDS.srv_tfmwAdjustments.EndInit()
                                        myFwAdjustmentsDS.AcceptChanges()
                                        isChanged = True
                                    End If
                                Next

                                If isChanged Then
                                    'update DB
                                    Dim myAdjustmentsDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                                    myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, myAdjustmentsDS)
                                End If
                            End If

                        End If
                        'END SGM 21/09/2011
                    End If
                    'END SGM 24/11/2011

                End If



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.LoadFwAdjustments", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Returns PhotometryDataTO
        ''' </summary>
        ''' <returns>TO with the Photometry data</returns>
        ''' <remarks>Created by XBC 25/02/2011</remarks>
        Public Function ReadPhotometryData() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal.SetDatos = myPhotometryDataAttr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ReadPhotometryData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Set PhotometryDataTO
        ''' </summary>
        ''' <param name="pPhotometryData">GlobalDataTo with the data of the Photometry</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function SetPhotometryData(ByVal pPhotometryData As PhotometryDataTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myPhotometryDataAttr = pPhotometryData

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SetPhotometryData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#Region "TO DELETE"
        '''' <summary>
        '''' SERVICE
        '''' Loads the Sensors Master data stored in the database
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>SGM 28/02/11</remarks>
        'Public Function LoadFwSensorsMasterData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        'If MyClass.mySensorsDS Is Nothing Then 'only if it is not loaded yet
        '        Dim mySensorsDAO As New Biosystems.Ax00.DAL.DAO.tfmwSensorsDAO
        '        myGlobal = mySensorsDAO.GetSensorsMasterData(Nothing)
        '        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
        '            MyClass.mySensorsDS = CType(myGlobal.SetDatos, SRVSensorsDS)
        '            MyClass.mySensorsDelegate = New SRVSensorsDelegate(MyClass.mySensorsDS)

        '        End If
        '        'End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.LoadFwSensors", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SERVICE
        '''' Gets the Sensors Master data for a Screen
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>SGM 28/02/11</remarks>
        'Public Function GetGlobalSensorsMasterData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        myGlobal.SetDatos = MyClass.mySensorsDS

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetGlobalSensorsMasterData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SERVICE
        '''' Gets value of specific Sensor by code fw
        '''' </summary>
        '''' <param name="pCode"></param>
        '''' <returns></returns>
        '''' <remarks>SG 28/02/11</remarks>
        'Public Function GetSensorDataByCodeFw(ByVal pCode As String) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        myGlobal = MyClass.mySensorsDelegate.GetSensorDataByCodeFw(pCode)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetMonitorItemValuebyCodeFw", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal

        'End Function

        '''' <summary>
        '''' SERVICE
        '''' Gets value of specific Sensor by Element Id
        '''' </summary>
        '''' <param name="pSensor"></param>
        '''' <returns></returns>
        '''' <remarks>SG 28/02/11</remarks>
        'Public Function GetSensorDataById(ByVal pSensor As String) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        myGlobal = MyClass.mySensorsDelegate.GetSensorDataById(pSensor)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GetSensorDataById", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal

        'End Function
        'Public Function UpdateSensorsDS(ByVal pSensorsDS As SRVSensorsDS) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        myGlobal = mySensorsDelegate.UpdateSensorsDS(pSensorsDS)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '    End Try

        '    Return myGlobal

        'End Function
#End Region

        ''' <summary>
        ''' SERVICE
        ''' Returns StressDataTO
        ''' </summary>
        ''' <returns>TO with the Stress Mode data</returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Public Function ReadStressModeData() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal.SetDatos = myStressModeDataAttr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ReadStressModeData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Set StressDataTO
        ''' </summary>
        ''' <param name="pStressModeData">GlobalDataTo with the data of the StressMode</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Public Function SetStressModeData(ByVal pStressModeData As StressDataTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myStressModeDataAttr = pStressModeData

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SetStressModeData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "1st level private methods (Process new instrucions received and Generate & send instrucions)"

        ''' <summary>
        ''' Generate preparation instrucion parameter list and send it to the analyzer (TEST, PTEST, ISETEST)
        ''' </summary>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pExecutionType" ></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' AG 17/01/2011 - add pExecutionType parameter
        ''' </remarks>
        Private Function SendNextPreparation(ByVal pExecutionID As Integer, ByVal pExecutionType As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim startTime As DateTime = Now 'AG 05/06/2012 - time estimation

                'FIRST: Generate next instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                'myGlobal = myInstruction.GeneratePreparation(pExecutionID)
                If String.Equals(pExecutionType, "PREP_STD") Then
                    myGlobal = myInstruction.GeneratePreparation(pExecutionID)
                ElseIf String.Equals(pExecutionType, "PREP_ISE") Then
                    'TR Genereate the ise type Peparation.
                    myGlobal = myInstruction.GenerateISEPreparation(pExecutionID)
                End If
                'Debug.Print("ApplicationLayer.SendNextPreparation (Generate instruction): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

                ' XB 22/04/2014 - PENDING TO IMPLEMENT !!! #1599
                'If myGlobal.HasError Then
                '    Dim myWSExecutionDS As New ExecutionsDS
                '    Dim myExecutionDelegate As New ExecutionsDelegate
                '    'Get the execution data 
                '    myGlobal = myExecutionDelegate.GetExecution(Nothing, pExecutionID)
                '    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                '        myWSExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                '        If (myWSExecutionDS.twksWSExecutions.Rows.Count > 0) Then

                '            myGlobal = myExecutionDelegate.GetAffectedISEExecutions(Nothing, myWSExecutionDS.twksWSExecutions(0).AnalyzerID, _
                '                                                                    myWSExecutionDS.twksWSExecutions(0).WorkSessionID, pExecutionID, _
                '                                                                    myWSExecutionDS.twksWSExecutions(0).SampleClass)
                '            If Not myGlobal.HasError Then
                '                myWSExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                '                'Go throug each related executioin and  Set the ExecutionStatus to LOCKED.

                '                Dim myLogAcciones As New ApplicationLogManager()

                '                For Each myWSExec As ExecutionsDS.twksWSExecutionsRow In myWSExecutionDS.twksWSExecutions.Rows
                '                    myWSExec.BeginEdit()
                '                    myWSExec.ExecutionStatus = "LOCKED"
                '                    myWSExec.EndEdit()
                '                    GlobalBase.CreateLogActivity("Instruction with empty fields. Sw locks execution: " & myWSExec.ExecutionID.ToString, "Instructions.GenerateISEPreparation", EventLogEntryType.Information, False) 'AG 30/05/2012
                '                Next
                '                'Update Status on Database.
                '                myGlobal = myExecutionDelegate.UpdateStatus(Nothing, myWSExecutionDS)
                '            End If
                '            'After locking make sure send error to next level
                '            myGlobal.HasError = True
                '            myGlobal.ErrorCode = "EMPTY_FIELDS"
                '        End If
                '    End If
                'End If
                ' XB 22/04/2014 - PENDING TO IMPLEMENT !!! #1599

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                'startTime = Now 'AG 05/06/2012 - time estimation
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)
                'Debug.Print("ApplicationLayer.SendNextPreparation (Send): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

                'FINALLY: If instruction sent inform the properties last executionID and last instruction data sent
                Dim sentInstruction As Boolean = False
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    sentInstruction = DirectCast(myGlobal.SetDatos, Boolean)
                    If sentInstruction Then
                        lastExecutionIDSentAttribute = pExecutionID
                        lastPreparationInstrucionSentAttribute = myInstructionToSend

                        'AG 03/02/2011
                        'lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SEND_PREPARATION
                        If String.Equals(pExecutionType, "PREP_STD") Then
                            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SEND_PREPARATION
                        ElseIf String.Equals(pExecutionType, "PREP_ISE") Then
                            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION
                        End If
                        'AG 03/02/2011

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendNextPreparation", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function



        ''' <summary>
        ''' Process instruction received from Analyzer. Depending the instruction type call the analyzer manager main function
        ''' with different cases
        ''' </summary>
        ''' <param name="pFwInstruction"></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' </remarks>
        Private Function ProcessResponse(ByVal pFwInstruction As List(Of ParametersTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim myInstruction As New Instructions
                Dim myIndexedParameters As New List(Of InstructionParameterTO)

                'myGlobal = myInstruction.GenerateReception(pFwInstruction)
                myGlobal = myInstruction.GenerateReception(pFwInstruction) 'TR 21/05/2010

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myIndexedParameters = DirectCast(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                ' Get the instruction type value (always in parameter index = 2)
                Dim myUtilities As New Utilities
                Dim myInstParameterTO As New InstructionParameterTO
                myGlobal = myUtilities.GetItemByParameterIndex(myIndexedParameters, 2)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParameterTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'SGM Track 
                ResponseTrack = DateTime.Now 'SGM 03/07/2012 just for debugging timing improvement
                'end Track

                Select Case myInstParameterTO.Parameter
                    'Sw has receive from Analyzer a new STATUS instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.STATUS.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, myIndexedParameters)
                        Exit Select

                        'Sw has receive from Analyzer a new RESULTS instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSPHR.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READINGS_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 01/03/2011 - Rename ANSAL -> ANSBLD. Remove ANSBL and ANSDL
                        'Case GlobalEnumerates.AppLayerInstrucionReception.ANSAL.ToString().Trim(), GlobalEnumerates.AppLayerInstrucionReception.ANSBL.ToString().Trim(), _
                        'GlobalEnumerates.AppLayerInstrucionReception.ANSDL.ToString().Trim()
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBLD.ToString().Trim
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BASELINE_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 03/01/2011 - Sw has receive from Analyzer a new ISE RESULTS instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSISE.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED, myIndexedParameters)
                        Exit Select

                        'SGM 25/05/2012 - Sw has receive from Analyzer a new ANSFWU instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFWU.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFWU_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 14/03/2011 - Sw has receive from Analyzer a new ANSERR instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSERR.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 15/03/2011 - Sw has receive from Analyzer a new ANSBR1, ANSBR2, ANSBM1 instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString.Trim, GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString.Trim, GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 07/04/2011 - Sw has receive from Analyzer a new ANSINF instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSINF.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSINF_RECEIVED, myIndexedParameters)
                        Exit Select

                        'AG 22/06/2011 - Sw has received from Analyzer a new ANSBCR instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSCBR.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSCBR_RECEIVED, myIndexedParameters)
                        Exit Select

                        ' XBC 30/07/2012
                        'Sw has receive from Analyzer a new ANSPSN instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSPSN.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSPSN_RECEIVED, myIndexedParameters)
                        Exit Select

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSPRD.ToString.Trim 'AG 31/07/2012
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSPRD_RECEIVED, myIndexedParameters)
                        Exit Select

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSTIN.ToString.Trim 'AG 31/07/2012
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSTIN_RECEIVED, myIndexedParameters)
                        Exit Select


                        ''''''''''''''''''''''
                        'SERVICE INSTRUCTIONS
                        ''''''''''''''''''''''

                        ' XBC 03/05/2011
                        'Sw has receive from Analyzer a new ANSCMD instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSCMD.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND_RECEIVED, myIndexedParameters)
                        Exit Select

                        ' SGM 26/01/11
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSADJ.ToString.Trim
                        'Adjustments received
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 08/06/2011 - Sw has receive from Analyzer a new ANSCPU instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSCPU.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSCPU_RECEIVED, myIndexedParameters)
                        Exit Select

                        'SGM 24/05/2011 - Sw has receive from Analyzer a new ANSJEX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSJEX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSJEX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'SGM 24/05/2011 - Sw has receive from Analyzer a new ANSJEX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSSFX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSSFX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'SGM 24/05/2011 - Sw has receive from Analyzer a new ANSJEX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSGLF.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSGLF_RECEIVED, myIndexedParameters)
                        Exit Select


                        'XBC 31/05/2011 - Sw has receive from Analyzer a new ANSBXX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBXX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSBXX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 01/06/2011 - Sw has receive from Analyzer a new ANSDXX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSDXX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSDXX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 01/06/2011 - Sw has receive from Analyzer a new ANSRXX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSRXX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSRXX_RECEIVED, myIndexedParameters)
                        Exit Select


                        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                        '    ' SGM 14/03/11
                        'Case GlobalEnumerates.AppLayerInstrucionReception.ABSORBANCE_RECEIVED.ToString.Trim
                        '    'Sensors Data received
                        '    RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABSORBANCESCAN_RECEIVED, myIndexedParameters)
                        '    Exit Select

                        '    ' SGM 28/02/11
                        'Case GlobalEnumerates.AppLayerInstrucionReception.SENSORS_RECEIVED.ToString.Trim
                        '    'PDT Sensors Data received
                        '    RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SENSORS_RECEIVED, myIndexedParameters)

                        '    Exit Select

                        ' XBC 22/03/2011
                        'Sw has receive from Analyzer a new ANSSDM instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSSDM.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSSDM, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFCP instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFCP.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFCP_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFBX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFBX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFBX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFDX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFDX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFDX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFRX instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFRX.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFRX_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFGL instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFGL.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFGL_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFJE instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFJE.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFJE_RECEIVED, myIndexedParameters)
                        Exit Select

                        'XBC 02/06/2011 - Sw has receive from Analyzer a new ANSFSF instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSFSF.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSFSF_RECEIVED, myIndexedParameters)
                        Exit Select

                        'SGM 04/10/2012 - Sw has receive from Analyzer a new ANSUTIL instruction
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSUTIL.ToString.Trim
                        '1) Inform the analyzer manager about the received instruction
                        RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSUTIL_RECEIVED, myIndexedParameters)
                        Exit Select

                        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                        '    ' SGM 27/03/11
                        'Case GlobalEnumerates.AppLayerInstrucionReception.TANKTESTEMPTYLC_OK.ToString.Trim
                        '    'Sensors Data received
                        '    RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTEMPTYLC_OK, myIndexedParameters)
                        '    Exit Select

                        '    ' SGM 27/03/11
                        'Case GlobalEnumerates.AppLayerInstrucionReception.TANKTESTFILLDW_OK.ToString.Trim
                        '    'Sensors Data received
                        '    RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTFILLDW_OK, myIndexedParameters)
                        '    Exit Select

                        '    ' SGM 27/03/11
                        'Case GlobalEnumerates.AppLayerInstrucionReception.TANKTESTTRANSFER_OK.ToString.Trim
                        '    'Sensors Data received
                        '    RaiseEvent ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTTRANSFER_OK, myIndexedParameters)
                        '    Exit Select
                        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent



                        ' PDT !!! pending to define !!! 
                        ' També queda pendent, abans d'entrar a aquesta funció, la lectura de la resposta d'Script a través de LAX00Interpreter.ReadScript




                    Case Else
                        myGlobal.HasError = True
                        myGlobal.ErrorCode = "NOT_CASE_FOUND"

                End Select

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.ProcessResponse", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Generate a short instrucion parameter list and send it to the analyzer
        ''' </summary>
        ''' <param name="pInstType"></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' </remarks>
        Private Function SendShortInstruction(ByVal pInstType As GlobalEnumerates.AppLayerEventList) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate short instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateShortInstruction(pInstType)  'Generate the short instruction
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: Check if instruction properly sent
                Dim sentInstruction As Boolean = False
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    sentInstruction = DirectCast(myGlobal.SetDatos, Boolean)
                    If sentInstruction Then
                        lastInstructionTypeSentAttribute = pInstType

                        'TODO: Business associated (if needed)
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendShortInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generate a short connection instrucion parameter list
        ''' </summary>
        ''' <param name="pConnectionString" ></param>
        ''' <returns>Array of bytes containing the instruction</returns>
        ''' <remarks>
        ''' Creation RH 05/12/2010
        ''' </remarks>
        Private Function GenerateConnectionInstructionInBytes(ByRef pConnectionString As String) As Byte()
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate short instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateShortInstruction(GlobalEnumerates.AppLayerEventList.CONNECT)  'Generate the short instruction
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                    lastInstructionSentAttribute = myInstructionToSend     'AG 20/10/2010
                Else
                    Exit Try
                End If

                pConnectionString = myInstructionToSend

                'FINALLY: Convert string to byte
                Dim myUtilities As New Utilities
                myGlobal = myUtilities.ConvertStringToAscii(myInstructionToSend, True)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.GenerateConnectionInstructionInBytes", EventLogEntryType.Error, False)

            End Try

            Return CType(myGlobal.SetDatos, Byte())
        End Function

        '''' <summary>
        '''' Generate the BLIGHT or DLIGHT instructions
        '''' </summary>
        '''' <param name="pInstType"></param>
        '''' <returns>GlobalDataTo indicating if an error has occurred or not. If success, returns the instruction sent (STRING)</returns>
        '''' <remarks>
        '''' Creation RH 05/31/2010
        '''' Removed XBC 20/04/2011 - Is no used
        '''' </remarks>
        'Private Function SendBaseLineWithoutAdjustmentInstruction(ByVal pInstType As GlobalEnumerates.AppLayerEventList, ByVal pWell As Integer) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        'FIRST: Generate short instruction
        '        Dim myInstruction As New Instructions
        '        Dim paramInstructionList As New List(Of InstructionParameterTO)

        '        myGlobal = myInstruction.GenerateShortInstruction(pInstType)  'Generate the short instruction
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
        '        Else
        '            Exit Try
        '        End If

        '        'SECOND: Convert InstructionParameterTO into LAX00 string to send
        '        Dim myInstructionToSend As String = ""
        '        myGlobal = Me.TransformToStringInstruction(paramInstructionList)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstructionToSend = CType(myGlobal.SetDatos, String)
        '        Else
        '            Exit Try
        '        End If

        '        myInstructionToSend = myInstructionToSend.Replace("W;", String.Format("W:{0};", pWell))

        '        'THIRD: Send generical instruction!!
        '        myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

        '        'FINALLY: Check if instruction properly sent
        '        Dim sentInstruction As Boolean = False
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            sentInstruction = DirectCast(myGlobal.SetDatos, Boolean)
        '            If sentInstruction Then
        '                lastInstructionTypeSentAttribute = pInstType
        '                myGlobal.SetDatos = myInstructionToSend
        '                'TODO: Business associated (if needed)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendBaseLineWithoutAdjustmentInstruction", EventLogEntryType.Error, False)

        '    End Try
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' Generate a script micro-instrucions list and send it to the analyzer
        ''' </summary>
        ''' <param name="pFwScriptID ">Identifier of the script/action to be sended</param>
        ''' <param name="pParams">param values to script/action to be sended</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by XBC 08/11/2010
        ''' Modified by XBC 18/11/2010 - Add optional field pParams
        ''' </remarks>
        Private Function SendCOMMANDInstruction(ByVal pFwScriptID As String, Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate script instruction
                Dim myInstructionsList As New List(Of InstructionTO)

                myGlobal = myFwScriptsData.GenerateFwScriptInstruction(pFwScriptID, pParams)  'Generate the list microinstructions of the specified ScriptID
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionsList = CType(myGlobal.SetDatos, List(Of InstructionTO))

                    'END instruction time
                    If myInstructionsList IsNot Nothing AndAlso myInstructionsList.Count > 0 Then
                        Dim myENDInstruction As InstructionTO = myInstructionsList(myInstructionsList.Count - 1)
                        EndInstructionTime = myENDInstruction.Timer
                    End If

                Else
                    Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("Error COMMAND Instruction", "ApplicationLayer.SendCOMMANDInstruction", EventLogEntryType.Error, False)
                    Exit Try
                End If

                'SECOND: Convert InstructionTO into LAX00 string to send
                Dim myLAX00 As New LAX00Interpreter
                Dim myInstructionToSend As String
                myGlobal = myLAX00.WriteFwScript(myInstructionsList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = "A400;COMMAND;" & CType(myGlobal.SetDatos, String)
                    If String.Compare(myInstructionToSend, "", False) <> 0 Then
                        myGlobal = Me.SendGenericalInstructions(myInstructionToSend)
                        If myGlobal.HasError Then
                            Exit Try
                        Else
                            'FINALLY: If instruction sent inform the properties last instruction data sent
                            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.COMMAND
                        End If
                    Else
                        Exit Try
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendCOMMANDInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generate a script micro-instrucions list and send it to the analyzer
        ''' </summary>
        ''' <param name="pInstructions ">List of instructions to send to Instrument</param>
        ''' <param name="pParams">param values to script/action to be sended</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by XBC 20/09/2011
        ''' </remarks>
        Private Function SendCOMMANDInstructionTest(ByVal pInstructions As List(Of InstructionTO), Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate script instruction
                Dim myInstructionsList As New List(Of InstructionTO)

                myGlobal = myFwScriptsData.GenerateFwScriptInstruction(pInstructions, pParams)  'Generate the list microinstructions of the specified ScriptID
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionsList = CType(myGlobal.SetDatos, List(Of InstructionTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionTO into LAX00 string to send
                Dim myLAX00 As New LAX00Interpreter
                Dim myInstructionToSend As String
                myGlobal = myLAX00.WriteFwScript(myInstructionsList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = "A400;COMMAND;" & CType(myGlobal.SetDatos, String)
                    If String.Compare(myInstructionToSend, "", False) <> 0 Then
                        myGlobal = Me.SendGenericalInstructions(myInstructionToSend)
                        If myGlobal.HasError Then
                            Exit Try
                        Else
                            'FINALLY: If instruction sent inform the properties last instruction data sent
                            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.COMMAND
                        End If
                    Else
                        Exit Try
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendCOMMANDInstructionTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generate washing running instrucion using information in pWashingDataDS
        ''' </summary>
        ''' <param name="pWashingDataDS"></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Modified by: SA 22/11/2013 - BT #1359 => Update attribute lastPreparationInstructionSent with value of the received instruction
        ''' </remarks>
        Private Function SendRunningWashInstruction(ByVal pWashingDataDS As AnalyzerManagerDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate WRUN instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                If pWashingDataDS.nextPreparation.Rows.Count > 0 Then '(1)
                    'TR 28/01/2011 -Enable Instruction generation.
                    myGlobal = myInstruction.GenerateWRunInstruction(pWashingDataDS)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                    Else
                        Exit Try
                    End If

                    'SECOND: Convert InstructionParameterTO into LAX00 string to send
                    Dim myInstructionToSend As String = ""
                    myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myInstructionToSend = CType(myGlobal.SetDatos, String)
                    Else
                        Exit Try
                    End If

                    'THIRD: Send generical instruction!!
                    myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                    'FINALLY: If instruction sent inform the properties last instruction data sent
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        lastPreparationInstrucionSentAttribute = myInstructionToSend
                        lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.WASH_RUNNING
                    End If

                End If '(1) If pWashingDataDS.nextPreparation.Rows.Count > 0 Then


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendRunningWashInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SendStandByWashInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate WASH instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateWASHInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.WASH
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendStandByWashInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send ALIGHT instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/03/2011</remarks>
        Private Function SendALIGHTInstruction(ByVal pWell As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate ALIGHT instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateALIGHTInstruction(pWell)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.ALIGHT
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendALIGHTInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send INFO instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>TR 07/04/2011 - created</remarks>
        Private Function SendINFOInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate INFO instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateINFOInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.INFO
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendINFOInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Send WSCTRL instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>TR 07/04/2011 - created</remarks>
        Private Function SendWSCTRLInstruction(ByVal pQueryMode As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate WSCTRL instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateWSCTRLInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.WSCTRL
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendWSCTRLInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Generate barcode instruction using information in pBarCodeDS
        ''' </summary>
        ''' <param name="pBarCodeDS" ></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks></remarks>
        Private Function SendCodeBarInstruction(ByVal pBarCodeDS As AnalyzerManagerDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate WRUN instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                If pBarCodeDS.barCodeRequests.Rows.Count > 0 Then '(1)
                    'TR 28/01/2011 -Enable Instruction generation.
                    myGlobal = myInstruction.GenerateCODEBRInstruction(analyzerIDAttribute, pBarCodeDS)

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                    Else
                        Exit Try
                    End If

                    'SECOND: Convert InstructionParameterTO into LAX00 string to send
                    Dim myInstructionToSend As String = ""
                    myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myInstructionToSend = CType(myGlobal.SetDatos, String)
                    Else
                        Exit Try
                    End If

                    'THIRD: Send generical instruction!!
                    myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                    'FINALLY: If instruction sent inform the properties last executionID and last instruction data sent
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST
                    End If

                End If '(1) If pBarCodeDS.barCodeRequests.Rows.Count > 0 Then


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendCodeBarInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send ALIGHT instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: XBC 20/02/2012 - overloaded to allow fillmode parameter</remarks>
        Private Function SendALIGHTInstruction(ByVal pParams As List(Of String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate ALIGHT instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)
                Dim myWell As Integer
                Dim myFillMode As Integer

                If IsNumeric(pParams(0)) Then
                    myWell = CInt(pParams(0))
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If

                If IsNumeric(pParams(1)) Then
                    myFillMode = CInt(pParams(1))
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If

                myGlobal = myInstruction.GenerateALIGHTInstruction(myWell, myFillMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.ALIGHT
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendALIGHTInstruction overload 2", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send BLIGHT instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: XBC 20/04/2011</remarks>
        Private Function SendBLIGHTInstruction(ByVal pParams As List(Of String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate BLIGHT instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)
                Dim myWell As Integer
                Dim myFillMode As Integer

                If IsNumeric(pParams(0)) Then
                    myWell = CInt(pParams(0))
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If

                If IsNumeric(pParams(1)) Then
                    myFillMode = CInt(pParams(1))
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If

                myGlobal = myInstruction.GenerateBLIGHTInstruction(myWell, myFillMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.BLIGHT
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendBLIGHTInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send READADJ instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 03/05/2011 - created</remarks>
        Private Function SendREADADJInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate READADJ instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateREADADJInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.READADJ
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendREADADJInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send ISECMD instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 12/12/2011 - created</remarks>
        Private Function SendISECMDInstruction(ByVal pISECommand As ISECommandTO) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate ISECMD instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateISECMDInstruction(pISECommand)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.ISE_CMD
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendISECMDInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Send POLLRD instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>AG 31/07/2012 - created
        ''' Modify by AG 22/11/2013 - #1397 - New parameter informing if analyzer in pause mode or not</remarks>
        Private Function SendPOLLRDInstruction(ByVal pPollRdAction As Integer) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate POLLRD instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GeneratePOLLRDInstruction(analyzerIDAttribute, worksessionIDAttribute, pPollRdAction, RecoveryResultsInPauseAttribute) 'AG 27/11/2013 #1397 new parameter allowScanInRunningAttribute

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.POLLRD
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendPOLLRDInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send FWUTIL instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 29/05/2012 - created</remarks>
        Private Function SendFWUTILInstruction(ByVal pFWAction As FWUpdateRequestTO) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate FWUTIL instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateFWUTILInstruction(pFWAction)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If


                'THIRD: Send generical instruction!!
                If pFWAction.ActionType = GlobalEnumerates.FwUpdateActions.SendRepository Then
                    'blocks must be passed directly as byte array in order to avoid data loss in conversion

                    Dim myUtilities As New Utilities
                    Dim myCommand As String = "A400;FWUTIL;A:2;N:" & pFWAction.DataBlockIndex.ToString & ";S:" & pFWAction.DataBlockSize.ToString & ";"
                    myGlobal = myUtilities.ConvertStringToAscii(myCommand, True)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        Dim myCommandInBytes() As Byte = CType(myGlobal.SetDatos, Byte())
                        Dim myFinalByte As Byte = Asc(CChar(";"))

                        Dim myFinalInstruction(myCommandInBytes.Length + pFWAction.DataBlockSize) As Byte
                        Dim i As Integer = 0
                        Dim j As Integer = 0
                        For b1 As Integer = 0 To myCommandInBytes.Length - 1
                            myFinalInstruction(b1) = myCommandInBytes(b1)
                            i = i + 1
                        Next
                        j = i
                        For b2 As Integer = 0 To pFWAction.DataBlockBytes.Length - 1
                            myFinalInstruction(i + b2) = pFWAction.DataBlockBytes(b2)
                            j = j + 1
                        Next
                        myFinalInstruction(j) = myFinalByte

                        myGlobal = Me.SendGenericalInstructionsBytes(myFinalInstruction)

                    End If

                Else
                    myGlobal = Me.SendGenericalInstructions(myInstructionToSend)
                End If


                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.FW_UTIL
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendFWUTILInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        '''' <summary>
        '''' Send FWUTIL instruction.
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>SGM 29/05/2012 - created</remarks>
        'Private Function SendFWUTILInstruction(ByVal pFWAction As FWUpdateRequestTO) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        'FIRST: Generate FWUTIL instruction
        '        Dim myInstruction As New Instructions
        '        Dim paramInstructionList As New List(Of InstructionParameterTO)

        '        myGlobal = myInstruction.GenerateFWUTILInstruction(pFWAction)

        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
        '        Else
        '            Exit Try
        '        End If

        '        'SECOND: Convert InstructionParameterTO into LAX00 string to send
        '        Dim myInstructionToSend As String = ""
        '        myGlobal = Me.TransformToStringInstruction(paramInstructionList)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstructionToSend = CType(myGlobal.SetDatos, String)
        '        Else
        '            Exit Try
        '        End If

        '        'DATA BLOCK
        '        Dim isSendingFwDataBlock As Boolean = (pFWAction.ActionType = GlobalEnumerates.FwUpdateActions.SendRepository)
        '        If isSendingFwDataBlock Then
        '            myInstructionToSend &= pFWAction.DataBlockString & ";"
        '        End If

        '        'THIRD: Send generical instruction!!
        '        myGlobal = Me.SendGenericalInstructions(myInstructionToSend, isSendingFwDataBlock)

        '        'FINALLY: If instruction sent inform the properties last instruction data sent
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.FW_UTIL
        '        End If


        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendFWUTILInstruction", EventLogEntryType.Error, False)

        '    End Try
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' Send LOADADJ instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 06/05/2011 - created</remarks>
        Private Function SendLOADADJInstruction(ByVal pParamsToSave As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate LOADADJ instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateLOADADJInstruction(pParamsToSave)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                    myInstructionToSend = myInstructionToSend.Trim
                    myInstructionToSend = myInstructionToSend.Replace(" ", "")
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.LOADADJ
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendLOADADJInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send RESET_ANALYZER instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 01/07/2011 - created</remarks>
        Private Function SendRESETInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate RESET instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateRESETInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.RESET_ANALYZER
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendRESETInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send LOADFACTORYADJ instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 01/07/2011 - created</remarks>
        Private Function SendLOADFACTORYADJInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate LOADFACTORYADJ instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateLOADFACTORYADJInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.LOADFACTORYADJ
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendLOADFACTORYADJInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send UPDATEFW instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 05/07/2011 - created</remarks>
        Private Function SendUPDATEFWInstruction(ByVal pFirmware() As Byte) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate LOADADJ instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateUPDATEFWInstruction(pFirmware)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.UPDATE_FIRMWARE
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendUPDATEFWInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send TANKSTEST instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 18/05/2011 - created</remarks>
        Private Function SendTANKSTESTInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate TANKSTEST instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateTANKSTESTInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.TANKSTEST
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendTANKSTESTInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send STRESS TEST instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 23/05/2011 - created</remarks>
        Private Function SendSDMODEInstruction(ByVal pParams As List(Of String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate STRESS TEST instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)
                Dim myQueryMode As String
                Dim myCycles As String
                Dim myHour As String
                Dim myMinute As String
                Dim mySecond As String

                myQueryMode = pParams(0)
                myCycles = pParams(1)
                myHour = pParams(2)
                myMinute = pParams(3)
                mySecond = pParams(4)

                myGlobal = myInstruction.GenerateSDMODEInstruction(myQueryMode, myCycles, myHour, myMinute, mySecond)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SDMODE
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendSDMODEInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send STRESS READ instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 23/05/2011 - created</remarks>
        Private Function SendSDPOLLInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate SDPOLL instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateSDPOLLInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SDPOLL
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendSDPOLLInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send POLLFW instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 25/05/2011 - created</remarks>
        Private Function SendPOLLFWInstruction(ByVal pQueryMode As GlobalEnumerates.POLL_IDs) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate POLLFW instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GeneratePOLLFWInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.POLLFW
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendPOLLFWInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send POLLHW instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 31/05/2011 - created</remarks>
        Private Function SendPOLLHWInstruction(ByVal pQueryMode As GlobalEnumerates.POLL_IDs) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate POLLHW instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GeneratePOLLHWInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.POLLHW
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendPOLLHInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send ENABLE EVENTS instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 10/06/2011 - created</remarks>
        Private Function SendENABLEEVENTSInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate ENABLE FW EVENTS instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateENABLEEVENTSInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.ENABLE_EVENTS
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendENABLEEVENTSInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send DISABLE EVENTS instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 10/06/2011 - created</remarks>
        Private Function SendDISABLEEVENTSInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate DISABLE FW EVENTS instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateDISABLEEVENTSInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.DISABLE_EVENTS
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendDISABLEEVENTSInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send READCYCLES instruction. PENDING TO SPEC!!!!!!!!!!!!!
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 28/07/2011 - created</remarks>
        Private Function PDT_SendREADCYCLESInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate READCYC instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.PDT_GenerateREADCYCLESInstruction(pQueryMode)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.READCYC
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendREADCYCLESInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send WRITECYCLES instruction. PENDING TO SPEC!!!!!!!!!!!!!
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 28/07/2011 - created</remarks>
        Private Function PDT_SendWRITECYCLESInstruction(ByVal pParamsToSave As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate LOADADJ instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.PDT_GenerateWRITECYCLESInstruction(pParamsToSave)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.WRITECYC
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendWRITECYCLESInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send SOUND instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 18/10/2011 - created</remarks>
        Private Function SendSOUNDInstruction(ByVal pStatus As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate SOUND instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                If pStatus Then
                    myGlobal = myInstruction.GenerateSOUNDInstruction("1")
                Else
                    myGlobal = myInstruction.GenerateSOUNDInstruction("2")
                End If

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.SOUND
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendSOUNDInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Generate a config instruction and send it to the analyzer
        ''' </summary>
        ''' <param name="pConfigSettingsDS" ></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation AG 23/11/2011
        ''' </remarks>
        Private Function SendCONFIGInstruction(ByVal pConfigSettingsDS As AnalyzerSettingsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate config instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GenerateCONFIGInstruction(pConfigSettingsDS)  'Generate the config instruction
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.CONFIG
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendCONFIGInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Send UTIL instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 04/06/2012</remarks>
        Private Function SendUTILInstruction(ByVal pUtilCommand As UTILCommandTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate UTIL instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                Dim myTankTest As String = "0"
                Dim myCollisionTest As String = "0"
                Dim mySNAction As String = "0"
                Dim mySerialNumber As String = "0"


                Select Case pUtilCommand.ActionType
                    Case GlobalEnumerates.UTILInstructionTypes.IntermediateTanks
                        myTankTest = CInt(pUtilCommand.TanksActionType).ToString

                    Case GlobalEnumerates.UTILInstructionTypes.NeedleCollisionTest
                        myCollisionTest = CInt(pUtilCommand.CollisionTestActionType).ToString

                    Case GlobalEnumerates.UTILInstructionTypes.SaveSerialNumber
                        mySNAction = CInt(pUtilCommand.SaveSerialAction).ToString
                        mySerialNumber = pUtilCommand.SerialNumberToSave

                End Select

                If pUtilCommand.ActionType <> GlobalEnumerates.UTILInstructionTypes.None Then

                    myGlobal = myInstruction.GenerateUTILInstruction(myTankTest, myCollisionTest, mySNAction, mySerialNumber)

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                    Else
                        Exit Try
                    End If

                    'SECOND: Convert InstructionParameterTO into LAX00 string to send
                    Dim myInstructionToSend As String = ""
                    myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myInstructionToSend = CType(myGlobal.SetDatos, String)
                    Else
                        Exit Try
                    End If

                    'THIRD: Send generical instruction!!
                    myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                    'FINALLY: If instruction sent inform the properties last instruction data sent
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.UTIL
                    End If


                End If



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendUTILInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        '''' <summary>
        '''' Send UTIL instruction.
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/06/2012</remarks>
        'Private Function SendUTILInstruction(ByVal pParams As List(Of String)) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        'FIRST: Generate UTIL instruction
        '        Dim myInstruction As New Instructions
        '        Dim paramInstructionList As New List(Of InstructionParameterTO)
        '        Dim myTankTest As String
        '        Dim myCollisionTest As String
        '        Dim myAction As String
        '        Dim mySerialNumber As String

        '        myTankTest = pParams(0)
        '        myCollisionTest = pParams(1)
        '        myAction = pParams(2)
        '        mySerialNumber = pParams(3)

        '        myGlobal = myInstruction.GenerateUTILInstruction(myTankTest, myCollisionTest, myAction, mySerialNumber)

        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
        '        Else
        '            Exit Try
        '        End If

        '        'SECOND: Convert InstructionParameterTO into LAX00 string to send
        '        Dim myInstructionToSend As String = ""
        '        myGlobal = Me.TransformToStringInstruction(paramInstructionList)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstructionToSend = CType(myGlobal.SetDatos, String)
        '        Else
        '            Exit Try
        '        End If

        '        'THIRD: Send generical instruction!!
        '        myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

        '        'FINALLY: If instruction sent inform the properties last instruction data sent
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.UTIL
        '        End If


        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendUTILInstruction", EventLogEntryType.Error, False)

        '    End Try
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' Send POLLSN instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 30/07/2012 - created</remarks>
        Private Function SendPOLLSNInstruction() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'FIRST: Generate POLLSN instruction
                Dim myInstruction As New Instructions
                Dim paramInstructionList As New List(Of InstructionParameterTO)

                myGlobal = myInstruction.GeneratePOLLSNInstruction()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    paramInstructionList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))
                Else
                    Exit Try
                End If

                'SECOND: Convert InstructionParameterTO into LAX00 string to send
                Dim myInstructionToSend As String = ""
                myGlobal = Me.TransformToStringInstruction(paramInstructionList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstructionToSend = CType(myGlobal.SetDatos, String)
                Else
                    Exit Try
                End If

                'THIRD: Send generical instruction!!
                myGlobal = Me.SendGenericalInstructions(myInstructionToSend)

                'FINALLY: If instruction sent inform the properties last instruction data sent
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    lastInstructionTypeSentAttribute = GlobalEnumerates.AppLayerEventList.POLLSN
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendPOLLSNInstruction", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

#End Region

#Region "2on level private methods"

        ''' <summary>
        ''' Generical function for sending all instruction types (convert to byte array and send it)
        ''' 
        ''' </summary>
        ''' <param name="pInstruction"></param>
        ''' <returns>GlobalDataTo (set data is boolean)</returns>
        ''' <remarks>Created by AG 22/04/2010</remarks>
        Private Function SendGenericalInstructions(ByVal pInstruction As String, Optional ByVal pWithoutLog As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                lastInstructionSentAttribute = pInstruction     'AG 20/10/2010

                'FIRST: Convert string to byte in order to send
                Dim myUtilities As New Utilities
                Dim bytesToSend() As Byte
                myGlobal = myUtilities.ConvertStringToAscii(pInstruction, True)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    bytesToSend = DirectCast(myGlobal.SetDatos, Byte())
                Else
                    Exit Try
                End If

                If Not pWithoutLog Then
                    ' XBC 08/11/2010
                    Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("Send Instruction [" & pInstruction.ToString & "]", "ApplicationLayer.SendGenericalInstructions", EventLogEntryType.Information, False)
                    ' XBC 08/11/2010
                End If

                'SECOND: Send it!!                
                myGlobal = Me.SendData(bytesToSend, False)

                If Not myGlobal.HasError Then
                    If Not CType(myGlobal.SetDatos, Boolean) Then
                        RaiseEvent SendDataFailed()
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendGenericalInstructions", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generical function for sending all instruction types (directly in bytes)
        ''' </summary>
        ''' <param name="pBytesToSend"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 28/06/12</remarks>
        Private Function SendGenericalInstructionsBytes(ByVal pBytesToSend() As Byte) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                lastInstructionSentAttribute = (pBytesToSend.ToString)

                'FIRST: 
                Dim bytesToSend() As Byte = pBytesToSend


                'SECOND: Send it!!                
                myGlobal = Me.SendData(bytesToSend, False)

                If Not myGlobal.HasError Then
                    If Not CType(myGlobal.SetDatos, Boolean) Then
                        RaiseEvent SendDataFailed()
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SendGenericalInstructionsBytes", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generical transformation from Generate.... (instruction generation: List(Of InstructionParameterTO)
        ''' into Lax00 string
        ''' 
        ''' </summary>
        ''' <param name="pParamInstructionList"></param>
        ''' <returns>GlobalDataTO with set data as string</returns>
        ''' <remarks>Created by AG 22/04/2010 </remarks>
        Private Function TransformToStringInstruction(ByVal pParamInstructionList As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim myInstruction As New Instructions

                'FIRST: Convert InstructionParameterTO into ParameterTO (without parameter indexes)
                For Each myInstructionTO As InstructionParameterTO In pParamInstructionList
                    myInstruction.Add(myInstructionTO.Parameter, myInstructionTO.ParameterValue)
                Next
                Dim parameterList As New List(Of ParametersTO)
                parameterList = DirectCast(myInstruction.GetParameterList.SetDatos, List(Of ParametersTO))

                'SECOND: Convert parameter list to string (Lax00)
                Dim myInstructionToSend As String = ""
                Dim myLAX00 As New LAX00Interpreter
                myGlobal = myLAX00.Write(parameterList)

                'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                'myInstructionToSend = CType(myGlobal.SetDatos, String)
                'End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.TransformToStringInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

#End Region

    End Class



End Namespace

