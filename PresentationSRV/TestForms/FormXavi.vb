
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class FormXavi


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

#Region "Private Enumerates"


    Private CurrentStatus As AnalyzerManagerStatus = AnalyzerManagerStatus.SLEEPING

#End Region

#Region "Simulation Delayed Responses"

    'Simulation Delayed Response timer
    Private AutoResponseTimerCallBack As System.Threading.TimerCallback
    Private WithEvents AutoResponseTimer As System.Threading.Timer


    Private SimulatedResponse As String = ""
    Private SimulatedResponseToSend As String = ""



    Private SimulatedResponse2 As String = ""

    'SOLO PARA SCRIPTS

    Private Sub AutoCommandResponse()
        Try
            MyClass.SimulatedResponseToSend = MyClass.SimulatedResponse
            If MyClass.SimulatedResponseToSend.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(MyClass.SimulatedResponse)
                MyClass.SimulatedResponse = ""
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function StartAutoResponseTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.AutoResponseTimerCallBack = New System.Threading.TimerCallback(AddressOf OnAutoResponseTimerTick)

            MyClass.AutoResponseTimer = New System.Threading.Timer(MyClass.AutoResponseTimerCallBack, New Object, 100, 0)

            MyClass.SimulatedResponseToSend = MyClass.SimulatedResponse

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            Throw ex
        End Try
        Return myGlobal
    End Function


    'SOLO PARA SCRIPTS
    Private Sub OnAutoResponseTimerTick(ByVal stateInfo As Object)

        Dim myGlobal As New GlobalDataTO

        Try
            If MyClass.SimulatedResponseToSend.Length > 0 Then

                myGlobal = myAnalyzerManager.SimulateInstructionReception(MyClass.SimulatedResponseToSend)

                Application.DoEvents()

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " OnAutoResponseTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            MyClass.SimulatedResponse = ""
            If MyClass.AutoResponseTimer IsNot Nothing Then
                MyClass.AutoResponseTimer.Dispose()
                MyClass.AutoResponseTimer = Nothing
            End If
        End Try
    End Sub

#End Region


#Region "Simulation Processed Responses"

    'Simulation Delayed Response timer
    Private ProcessResponseTimerCallBack As System.Threading.TimerCallback
    Private WithEvents ProcessResponseTimer As System.Threading.Timer

    'SOLO PARA SCRIPTS
    Private Function StartProcessResponseTimer(ByVal pTime As Integer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.ProcessResponseTimerCallBack = New System.Threading.TimerCallback(AddressOf OnProcessResponseTimerTick)

            MyClass.ProcessResponseTimer = New System.Threading.Timer(MyClass.ProcessResponseTimerCallBack, New Object, pTime, 0)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " StartReadingTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    Private Sub OnProcessResponseTimerTick(ByVal stateInfo As Object)

        Dim myGlobal As New GlobalDataTO

        Try
            If SimulatedResponse2.Length > 0 Then
                myGlobal = myAnalyzerManager.SimulateInstructionReception(SimulatedResponse2)
                SimulatedResponse2 = ""
            End If

            Application.DoEvents()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " OnAutoResponseTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region


#Region "Firmware Events"
    Private WithEvents UTILTimer As System.Threading.Timer
    Private UTILTimerCallBack As System.Threading.TimerCallback


    Private Function StartUTILTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.UTILTimerCallBack = New System.Threading.TimerCallback(AddressOf OnUTILTimerTick)

            Me.UTILTimer = New System.Threading.Timer(MyClass.UTILTimerCallBack, New Object, 1000, 1000)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " StartUTILTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    Private Function StopUTILTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyClass.UTILTimer IsNot Nothing Then
                MyClass.UTILTimer.Dispose()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " StopUTILTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    Private Sub OnUTILTimerTick(ByVal stateInfo As Object)

        Dim myGlobal As New GlobalDataTO

        Try

            SimulatedResponse = ANSUTIL_Generator()
            StartAutoResponseTimer()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " OnUTILTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region


#Region "Firmware Events"
    Private ANSINFOTimerCallBack As System.Threading.TimerCallback
    Private WithEvents ANSINFOTimer As System.Threading.Timer


    Private Function StartANSINFOTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.ANSINFOTimerCallBack = New System.Threading.TimerCallback(AddressOf OnANSINFOTimerTick)

            MyClass.ANSINFOTimer = New System.Threading.Timer(MyClass.ANSINFOTimerCallBack, New Object, 2000, 2000)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " StartANSINFOTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    Private Function StopANSINFOTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyClass.ANSINFOTimer IsNot Nothing Then
                MyClass.ANSINFOTimer.Dispose()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " StopANSINFOTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    Private Sub OnANSINFOTimerTick(ByVal stateInfo As Object)

        Dim myGlobal As New GlobalDataTO

        Try

            ANSINF_Generator()

            'StartUTILTimer()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " OnANSINFOTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region


#Region "Definitions"
    Private WithEvents myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

    Private AnalyzerAdjustmentsDS As SRVAdjustmentsDS
    Private myFwAdjustmentsDelegate As FwAdjustmentsDelegate
    Private myDbAdjustmentsDelegate As New DBAdjustmentsDelegate


#End Region

#Region "SA Testings"
    Private Sub BsButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton1.Click

        'Dim myWorkSession As New WorkSessionsDS

        'Dim myWSOrderTestRow As WSOrderTestsDS.twksWSOrderTestsRow
        'Dim myWSOrderTests As New WSOrderTestsDS

        'Dim myGlobalDataTO As New GlobalDataTO
        'Dim myWSClass As New WorkSessionsDelegate

        'Dim currentUser As String = "SUSANA"
        'Dim currentDateTime As DateTime = Now

        ''Creo los campos de la WorkSession
        'Dim myWorkSessionRow As WorkSessionsDS.twksWorkSessionsRow
        'myWorkSessionRow = CType(myWorkSession.twksWorkSessions.NewRow, WorkSessionsDS.twksWorkSessionsRow)
        'myWorkSessionRow.WorkSessionID = ""
        'myWorkSessionRow.WSDateTime = currentDateTime
        'myWorkSessionRow.TS_User = currentUser
        'myWorkSessionRow.TS_DateTime = currentDateTime
        'myWorkSession.twksWorkSessions.AddtwksWorkSessionsRow(myWorkSessionRow)

        ''Cargo la lista de Order Tests a incluir en la Work Session
        'Dim i As Integer
        'Dim myOrderTestList(0 To 9) As Integer

        'myOrderTestList(0) = 3
        'myOrderTestList(1) = 4
        'myOrderTestList(2) = 5
        'myOrderTestList(3) = 6
        'myOrderTestList(4) = 7
        'myOrderTestList(5) = 9
        'myOrderTestList(6) = 12
        'myOrderTestList(7) = 13
        'myOrderTestList(8) = 14
        'myOrderTestList(9) = 116

        'For i = 0 To 9
        '    myWSOrderTestRow = CType(myWSOrderTests.twksWSOrderTests.NewRow, WSOrderTestsDS.twksWSOrderTestsRow)
        '    myWSOrderTestRow.OrderTestID = myOrderTestList(i)
        '    myWSOrderTestRow.ToSendFlag = True
        '    myWSOrderTestRow.TS_User = currentUser
        '    myWSOrderTestRow.TS_DateTime = currentDateTime
        '    myWSOrderTests.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestRow)
        'Next i

        ''Creación de la Work Session
        '' myGlobalDataTO = myWSClass.AddWorkSession(myWSOrderTests)
        'If (myGlobalDataTO.HasError) Then
        '    MsgBox("ERROR: " & myGlobalDataTO.ErrorCode & "-" & myGlobalDataTO.ErrorMessage)
        'Else
        '    MsgBox("WORK SESSION CREADA CORRECTAMENTE")
        'End If

    End Sub

    'Private Sub getOpenOTs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getOpenOTs.Click
    '    'Dim getSamples As New WorkSessionsDelegate
    '    'Dim resultData As New GlobalDataTO

    '    ''resultData = getSamples.GetSamplesForWorkSession()


    'End Sub
#End Region

#Region "AG General Form Events"

    Private Sub Form3_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim myGlobal As New GlobalDataTO

        Try

            'FW Adjustments Master Data
            myGlobal = myAnalyzerManager.LoadFwAdjustmentsMasterData(True)
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                MyClass.AnalyzerAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                If MyClass.AnalyzerAdjustmentsDS IsNot Nothing Then
                    MyClass.myFwAdjustmentsDelegate = New FwAdjustmentsDelegate(MyClass.AnalyzerAdjustmentsDS)
                End If
            End If

            'AG - Basic method to solve exceptions when differents threads access to the same control
            'See: http://www.elguille.info/NET/vs2005/trucos/acceder_a_un_control_desde_otro_hilo.htm
            CheckForIllegalCrossThreadCalls = False

            myArmSelected = "BM1"
            myProbeSelected = "DR1"
            myRotorSelected = "RR1"
            myFwSelected = "CPU"
            myFwSelected2 = "CP"
            myRotorSelected2 = "(10)"
            myProbeSelected2 = "(7)"
            myArmSelected2 = "(4)"
            myFwSelected3 = "(1)"

        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "AG Old Testings (April 2010)"

    'Private Sub BsAGTests_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAGTests.Click
    '    Try
    '        If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '            Dim myGlobal As New GlobalDataTO

    '            Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    '            If myAnalyzerManager.CommThreadsStarted Then
    '                'Test INIT COMMS
    '                'myGlobal = myAnalyzerManager.ManageAnalyzer(Biosystems.Ax00.CommunicationsSwFw.AnalyzerManager.ClassActionList.INIT_COMMS)

    '                'Test NEW_PREPARATION
    '                myGlobal = myAnalyzerManager.ManageAnalyzer(Biosystems.Ax00.CommunicationsSwFw.AnalyzerManager.ClassActionList.NEW_PREPARATION)

    '            End If
    '        End If
    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Sub BsLAX00_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLAX00.Click
    '    Try
    '        Dim myLAX00 As New LAX00Interpreter
    '        Dim myGlobal As New GlobalDataTO

    '        'Write
    '        Dim myInstruc As New Instructions
    '        Dim myInstrucList As New List(Of InstructionParameterTO)

    '        'Manually instruction creation
    '        myInstruc.Add("A", "A400")
    '        myInstruc.Add("TYPE", "STATE")
    '        myInstruc.Add("S", "1")
    '        myInstruc.Add("AC", "12")
    '        myInstruc.Add("T", "13")
    '        myInstruc.Add("C", "14")
    '        myInstruc.Add("W", "15")
    '        myInstruc.Add("R", "0")
    '        myInstruc.Add("E", "1")
    '        myGlobal = myLAX00.Write(DirectCast(myInstruc.GetParameterList().SetDatos, List(Of ParametersTO)))
    '        BsTextWrite.Text = myGlobal.SetDatos.ToString()

    '        If BsTextWrite.Text.Trim = "" Then
    '            myGlobal = myInstruc.GeneratePreparation(50)

    '            If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then Exit Try
    '            myInstrucList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))

    '            For Each myInstructionTO As InstructionParameterTO In myInstrucList
    '                'agregamos a nuestra lista de intrucciones
    '                myInstruc.Add(myInstructionTO.Parameter, myInstructionTO.ParameterValue)
    '            Next

    '            myGlobal = myLAX00.Write(DirectCast(myInstruc.GetParameterList.SetDatos, List(Of ParametersTO)))
    '            If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then Exit Try
    '            BsTextWrite.Text = DirectCast(myGlobal.SetDatos, String)
    '        End If

    '        'Read
    '        BsTextRead.Clear()
    '        myGlobal = myLAX00.Read(BsTextWrite.Text.Trim)
    '        Dim myResultList As New List(Of ParametersTO)
    '        If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then Exit Try
    '        myResultList = DirectCast(myGlobal.SetDatos, List(Of ParametersTO))

    '        If myResultList.Count <> 0 Then
    '            BsTextRead.Text = DirectCast(myLAX00.Write(myResultList).SetDatos, String)
    '        End If

    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Sub BsAGReadPorts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAGReadPorts.Click
    '    Try
    '        If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '            Dim myGlobal As New GlobalDataTO

    '            Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    '            If myAnalyzerManager.CommThreadsStarted Then
    '                'Read registred ports
    '                myGlobal = myAnalyzerManager.ReadRegistredPorts

    '            End If
    '        End If

    '    Catch ex As Exception

    '    End Try

    'End Sub



    'Private Sub BsNextPreparation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsNextPreparation.Click
    '    Try
    '        If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '            Dim myGlobal As New GlobalDataTO

    '            Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    '            If myAnalyzerManager.CommThreadsStarted Then
    '                myAnalyzerManager.ActiveWorkSession = BsWorkSessionText.Text.ToString.Trim
    '                myAnalyzerManager.ActiveAnalyzer = "SN0000099999_Ax400"
    '                myGlobal = myAnalyzerManager.ManageAnalyzer(AnalyzerManager.ClassActionList.NEW_PREPARATION)

    '                'Dim executionIDSent As Integer = 0
    '                'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
    '                'executionIDSent = DirectCast(myGlobal.SetDatos, Integer)
    '                'End If

    '                myGlobal = myAnalyzerManager.ManageAnalyzer(AnalyzerManager.ClassActionList.PREPARATION_ACCEPTED)

    '            End If
    '        End If

    '    Catch ex As Exception

    '    End Try
    'End Sub

#End Region

#Region "AG Initial Generate Instructions Testings"

    Private Sub BsShortAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsShortAction.Click
        Try
            Dim myGlobal As New GlobalDataTO

            Dim myInstruc As New Instructions
            Dim myInstrucList As New List(Of InstructionParameterTO)


            myGlobal = myInstruc.GenerateShortInstruction(GlobalEnumerates.AppLayerEventList.RUNNING)
            'myGlobal = myInstruc.GenerateShortInstruction(ApplicationLayer.ClassEventList.CONNECT)

            If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then Exit Try
            myInstrucList = CType(myGlobal.SetDatos, List(Of InstructionParameterTO))

            For Each myInstructionTO As InstructionParameterTO In myInstrucList
                'agregamos a nuestra lista de intrucciones
                myInstruc.Add(myInstructionTO.Parameter, myInstructionTO.ParameterValue)
            Next

            Dim myLAX00 As New LAX00Interpreter
            myGlobal = myLAX00.Write(DirectCast(myInstruc.GetParameterList.SetDatos, List(Of ParametersTO)))
            If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then Exit Try
            BsTextWrite.Text = DirectCast(myGlobal.SetDatos, String)

            ''Test use AnalyzerManager class (Step by step until communications with analyzer)
            'If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
            '    Dim myGlobal As New GlobalDataTO

            ''Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            '    If myAnalyzerManager.CommThreadsStarted Then
            '        'Short instructions
            '        myGlobal = myAnalyzerManager.ManageAnalyzer(Biosystems.Ax00.CommunicationsSwFw.AnalyzerManager.ClassActionList.ABORT)

            '    End If
            'End If

        Catch ex As Exception

        End Try

    End Sub


    Private Sub BsReceptionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceptionButton.Click
        Try
            'Simulate instruction reception
            If BsTextWrite.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions
                        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite.Text.Trim)

                    End If
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BsReceptionButton_End_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceptionButton_End.Click
        Try
            'Simulate instruction reception
            If BsTextWrite_End.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions

                        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite_End.Text.Trim)

                    End If
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub BsReceptionButton_Sleeping_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceptionButton_Sleeping.Click
        Try
            'Simulate instruction reception
            If BsTextWrite_End.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions

                        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite_Sleeping.Text.Trim)

                    End If
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "AG Calculations & Time Remaining Testings"

    Private Sub BsExecuteCalc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExecuteCalc.Click
        'Try
        '    Dim myGlobal As New GlobalDataTO
        '    Dim myExec As Integer = 1
        '    If IsNumeric(BsExecution.Text) Then
        '        myExec = CInt(BsExecution.Text.ToString)

        '        'Execute calculations test
        '        Dim myCalc As New CalculationsDelegate
        '        'myGlobal = myCalc.CalculateExecution(Nothing, myExec, "SN0000099999_Ax400", "2010010501", False, "")
        '        myGlobal = myCalc.CalculateExecution(Nothing, myExec, Ax00MainMDI.ActiveAnalyzer, Ax00MainMDI.ActiveWorkSession, False, "")
        '        If myGlobal.HasError Then
        '            Exit Try
        '        End If

        '        ''Validate execution readings test
        '        'Dim myRead As New WSReadingsDelegate
        '        'myGlobal = myRead.ValidateByExecutionID(Nothing, "SN0000099999_Ax400", "2010010501", myExec)
        '        'Dim myResult As Boolean = False
        '        'myResult = DirectCast(myGlobal.SetDatos, Boolean)
        '        'If myResult Then

        '        'End If

        '    End If

        'Catch ex As Exception

        'End Try
    End Sub


    Private Sub BsRemainingTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRemainingTime.Click
        'Try
        '    Dim resultData As New GlobalDataTO
        '    Dim myWS As New WorkSessionsDelegate

        '    resultData = myWS.CalculateTimeRemaining(Nothing, Ax00MainMDI.ActiveWorkSession)
        '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '        Dim reaminingTime As Single = CType(resultData.SetDatos, Single)
        '    End If

        'Catch ex As Exception

        'End Try
    End Sub

#End Region

#Region "AG - Several Instruction Generation & Reception Testings"

    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton2.Click
        Try
            ''Test use AnalyzerManager class (Step by step until communications with analyzer)
            'If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
            '    Dim myGlobal As New GlobalDataTO

            '    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            '    If myAnalyzerManager.CommThreadsStarted Then
            '        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BASE_LIGHT, True, Nothing, BsExecution.Text.Trim)
            '        BsTextWrite.Text = DirectCast(myGlobal.SetDatos, String)
            '    End If
            'End If

        Catch ex As Exception

        End Try
    End Sub


    Private Sub BsSendPrep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSendPrep.Click
        Try
            Dim myglobalto As New GlobalDataTO
            Dim myInstruction As New Instructions
            Dim myInstrucList As New List(Of InstructionParameterTO)
            'obtener los valores para los parametros para las preparaciones
            myglobalto = myInstruction.GeneratePreparation(CType(BsTextBoxToSend.Text.Trim(), Integer))
            If Not myglobalto.HasError Then
                Dim myLA00Inter As New LAX00Interpreter
                'asignamos lo valores a nuestra listra de instrucciones
                myInstrucList = DirectCast(myglobalto.SetDatos, List(Of InstructionParameterTO))
                For Each myInstructionTO As InstructionParameterTO In myInstrucList
                    'agregamos a nuestra lista de intrucciones
                    myInstruction.Add(myInstructionTO.Parameter, myInstructionTO.ParameterValue)
                Next

                If myInstruction.InstructionList.Count > 0 Then
                    DataGridView1.DataSource = DirectCast(myInstruction.GetParameterList().SetDatos, List(Of ParametersTO))
                    'recibimos una lista de parametros lista para ser escrita.
                    TextBox5.Text = myLA00Inter.Write(DirectCast(myInstruction.GetParameterList().SetDatos, List(Of ParametersTO))).SetDatos.ToString()
                End If


                myglobalto = myInstruction.GenerateReception(DirectCast(myInstruction.GetParameterList().SetDatos, List(Of ParametersTO)))
                If Not myglobalto.HasError Then
                    BsDataGridView1.DataSource = DirectCast(myglobalto.SetDatos, List(Of InstructionParameterTO))
                End If

                Dim myInstruction2 As New Instructions
                Dim myInstrucList2 As New List(Of InstructionParameterTO)
                myInstrucList2 = DirectCast(myglobalto.SetDatos, List(Of InstructionParameterTO))
                For Each myInstructionTO2 As InstructionParameterTO In myInstrucList2
                    'agregamos a nuestra lista de intrucciones
                    myInstruction2.Add(myInstructionTO2.Parameter, myInstructionTO2.ParameterValue)
                Next

                If myInstruction2.InstructionList.Count > 0 Then
                    'DataGridView1.DataSource = DirectCast(myInstruction.GetParameterList().SetDatos, List(Of ParametersTO))
                    'recibimos una lista de parametros lista para ser escrita.
                    TextBox5.Text = TextBox5.Text & vbNewLine & myLA00Inter.Write(DirectCast(myInstruction2.GetParameterList().SetDatos, List(Of ParametersTO))).SetDatos.ToString()
                End If



            Else
                DataGridView1.DataSource = Nothing
                TextBox5.Clear()
                ShowMessage("Error", myglobalto.ErrorCode, myglobalto.ErrorMessage)
            End If
        Catch ex As Exception
            ShowMessage("ERROR", "ERROR", ex.Message)
        End Try

    End Sub

    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton3.Click
        Try
            'Simulate instruction reception
            If TextBox5.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions
                        myGlobal = myAnalyzerManager.SimulateInstructionReception(TextBox5.Text.Trim)

                        'Me.DataGridView1.DataSource = DirectCast (myGlobal.SetDatos, 
                    End If
                End If

            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Communications Board Testings"

    Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles myAnalyzerManager.ReceptionEvent
        Try
            'If Not pTreated Then
            If BsReceivedTextBox.Text.Length > 10000 Then BsReceivedTextBox.Clear()

            If Not pInstructionReceived.Contains("ANSINF") AndAlso MyClass.myFWUpdateAction <> FwUpdateActions.SendRepository Then
                'BsReceivedTextBox.Clear()
                BsReceivedTextBox.Text += Now.ToString("hh:mm:ss") & vbTab & ">> " & pInstructionReceived & vbCrLf
                BsReceivedTextBox.Focus()
            End If
            'End If


        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub

    Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles myAnalyzerManager.SendEvent

        Dim myglobal As New GlobalDataTO

        Try

            'If Not pInstructionSent.Contains("INFO") Then
            BsReceivedTextBox.Text += Now.ToString("hh:mm:ss") & vbTab & "<< " & pInstructionSent & vbCrLf
            'End If

            'If pInstructionSent.Contains("POLLHW") Or pInstructionSent.Contains("POLLFW") Then
            '    BsReceivedTextBox.Text += Now.ToString("hh:mm:ss") & vbTab & "<< " & pInstructionSent & vbCrLf
            'End If

            If pInstructionSent.Contains("FWUTIL") Then

                If pInstructionSent.Contains("A:") Then
                    Dim a As String = pInstructionSent.Substring(pInstructionSent.IndexOf("A:") + 2)
                    Dim b As String = a.Substring(0, a.IndexOf(";"))
                    If IsNumeric(b) Then
                        MyClass.myFWUpdateAction = CType(CInt(b), FwUpdateActions)

                        myAnalyzerManager.SimulateInstructionReception(BsTextWrite.Text.Trim)

                        'data blocks
                        If MyClass.myFWUpdateAction = FwUpdateActions.SendRepository Then
                            SimulatedResponse2 = MyClass.ANSFWU_Generator()
                            MyClass.StartProcessResponseTimer(1)
                        End If
                    End If

                End If

                Exit Try
            End If


            'FOR TESTING
            If pInstructionSent.Contains("READADJ") Then
                AdjustmentsGenerator()
            End If

            'singular INFO request
            If pInstructionSent.Contains("INFO;Q:1;") Then
                ANSINF_Generator()
            End If

            'automatic refresh INFO start
            If pInstructionSent.Contains("INFO;Q:3;") Then
                StartANSINFOTimer()
            End If

            'automatic refresh INFO stop
            If pInstructionSent.Contains("INFO;Q:4;") Then
                StopANSINFOTimer()
            End If


            ''FOR TESTING
            If pInstructionSent.Contains("ISETEST") Or pInstructionSent.Contains("ISECMD") Then
                ANSISE_Generator(pInstructionSent)
            End If

            ''FOR TESTING
            If pInstructionSent.Contains("SLEEP") Then
                SLEEPING_Generator()
            End If

            ''FOR TESTING
            If pInstructionSent.Contains("STANDBY") Then
                STANDBY_Generator()
            End If

            ''FOR TESTING
            If pInstructionSent.Contains("LOADADJ") Then
                LOADADJ_Generator(pInstructionSent)
            End If

            ''FOR TESTING
            If pInstructionSent.Contains("LOADFACTORYADJ") Then
                LOADFACTORYADJ_Generator()
            End If

            ''FOR TESTING
            If pInstructionSent.Contains("UPDATEFW") Then
                UPDATEFW_Generator()
            End If


            ''FOR TESTING
            If pInstructionSent.Contains("RESET;") Then
                RESET_Generator()
            End If

            'FOR TESTING
            'update elements after receiving scripts
            If pInstructionSent.Contains("COMMAND") Then
                SimulateElementsUpdatement(pInstructionSent)
            End If

            'FOR TESTING
            If pInstructionSent.Contains("UTIL") Then
                InstructionStartEnd_Generator(AnalyzerManagerAx00Actions.UTIL_START)

                Application.DoEvents()

                If pInstructionSent.Contains("T:1;") Then 'tanks
                    myCurrentUTILAction = UTILInstructionTypes.IntermediateTanks
                    StartUTILTimer()
                End If
                If pInstructionSent.Contains("C:1;") Then 'collision start
                    myCurrentUTILAction = UTILInstructionTypes.NeedleCollisionTest
                    StartUTILTimer()
                End If
                If pInstructionSent.Contains("A:1;") Then 'tanks
                    myCurrentUTILAction = UTILInstructionTypes.SaveSerialNumber
                    StartUTILTimer()
                End If
                If pInstructionSent.Contains("T:0;C:0;A:0;SN:0") Then 'stop all
                    myCurrentUTILAction = UTILInstructionTypes.None
                    StopUTILTimer()
                End If
            End If

            If pInstructionSent.Contains("DISABLE_EVENTS") Then
                StopUTILTimer()
            End If


            If pInstructionSent.Contains("POLLHW") Then
                'FOR TESTING
                'read CPU
                If pInstructionSent.Contains("ID:1;") Then 'CPU
                    SimulatedResponse = ANSCPU_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read ARMS
                If pInstructionSent.Contains("ID:2;") Then 'BR1
                    myArmSelected = "BR1"
                    myArmSelected2 = "(2)"
                    SimulatedResponse = ANSBXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:3;") Then 'BR2
                    myArmSelected = "BR2"
                    myArmSelected2 = "(3)"
                    SimulatedResponse = ANSBXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:5;") Then 'AG1
                    myArmSelected = "AG1"
                    myArmSelected2 = "(5)"
                    SimulatedResponse = ANSBXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:6;") Then 'AG2
                    myArmSelected = "AG2"
                    myArmSelected2 = "(6)"
                    SimulatedResponse = ANSBXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:4;") Then 'BM1
                    myArmSelected = "BM1"
                    myArmSelected2 = "(4)"
                    SimulatedResponse = ANSBXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If

                'read PROBES
                If pInstructionSent.Contains("ID:7;") Then 'DR1
                    myProbeSelected = "DR1"
                    myProbeSelected2 = "(7)"
                    SimulatedResponse = ANSDXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:8;") Then 'DR2
                    myProbeSelected = "DR2"
                    myProbeSelected2 = "(8)"
                    SimulatedResponse = ANSDXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:9;") Then 'DM1
                    myProbeSelected = "DM1"
                    myProbeSelected2 = "(9)"
                    SimulatedResponse = ANSDXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If

                'read ROTORS
                If pInstructionSent.Contains("ID:10;") Then 'RM1
                    myRotorSelected = "RR1"
                    myRotorSelected2 = "(10)"
                    SimulatedResponse = ANSRXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:11;") Then 'RR1
                    myRotorSelected = "RM1"
                    myRotorSelected2 = "(11)"
                    SimulatedResponse = ANSRXX_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If


                'read MANIFOLD
                If pInstructionSent.Contains("ID:13;") Then 'JE1
                    SimulatedResponse = ANSJEX_Generator()
                    If SimulatedResponse.Length > 0 Then
                        myglobal = myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                    'StartReadingTimer()
                End If

                'FOR TESTING
                'read FLUIDICS
                If pInstructionSent.Contains("ID:14;") Then 'SF1
                    SimulatedResponse = ANSSFX_Generator()
                    If SimulatedResponse.Length > 0 Then
                        myglobal = myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                    'StartReadingTimer()
                End If

                'read PHOTOMETRICS
                If pInstructionSent.Contains("ID:12;") Then 'GLF
                    SimulatedResponse = ANSGLF_Generator()
                    If SimulatedResponse.Length > 0 Then
                        myglobal = myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                    'StartReadingTimer()
                End If

            End If




            If pInstructionSent.Contains("POLLFW") Then
                'FOR TESTING

                ' for testing Analyzer Info Screen
                FirstTimePhotometrics = True
                FirstTimeFluidics = True
                FirstTimeManifold = True
                ' for testing Analyzer Info Screen

                'read CPU
                If pInstructionSent.Contains("ID:1;") Then 'CPU
                    myFwSelected = "CPU"
                    myFwSelected2 = "CP"
                    myFwSelected3 = "(1)"
                    SimulatedResponse = ANSFCP_Generator()
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        'StartAutoResponseTimer()
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read ARMS
                If pInstructionSent.Contains("ID:4;") Then 'BM1
                    myFwSelected = "BM1"
                    myFwSelected2 = "BX"
                    myFwSelected3 = "(4)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.BM1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:2;") Then 'BR1
                    myFwSelected = "BR1"
                    myFwSelected2 = "BX"
                    myFwSelected3 = "(2)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.BR1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:3;") Then 'BR2
                    myFwSelected = "BR2"
                    myFwSelected2 = "BX"
                    myFwSelected3 = "(3)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.BR2)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:5;") Then 'AG1
                    myFwSelected = "AG1"
                    myFwSelected2 = "BX"
                    myFwSelected3 = "(5)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.AG1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:6;") Then 'AG2
                    myFwSelected = "AG2"
                    myFwSelected2 = "BX"
                    myFwSelected3 = "(6)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.AG2)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read PROBES
                If pInstructionSent.Contains("ID:7;") Then 'DR1
                    myFwSelected = "DR1"
                    myFwSelected2 = "DX"
                    myFwSelected3 = "(7)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.DR1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:8;") Then 'DR2
                    myFwSelected = "DR2"
                    myFwSelected2 = "DX"
                    myFwSelected3 = "(8)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.DR2)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:9;") Then 'DM1
                    myFwSelected = "DM1"
                    myFwSelected2 = "DX"
                    myFwSelected3 = "(9)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.DM1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read ROTORS
                If pInstructionSent.Contains("ID:10;") Then 'RR1
                    myFwSelected = "RR1"
                    myFwSelected2 = "RX"
                    myFwSelected3 = "(10)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.RR1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                If pInstructionSent.Contains("ID:11;") Then 'RM1
                    myFwSelected = "RM1"
                    myFwSelected2 = "RX"
                    myFwSelected3 = "(11)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.RM1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read PHOTOMETRICS
                If pInstructionSent.Contains("ID:12;") Then 'GLF
                    myFwSelected = "GLF"
                    myFwSelected2 = "GL"
                    myFwSelected3 = "(12)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.GLF)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read MANIFOLD
                If pInstructionSent.Contains("ID:13;") Then 'JE1
                    myFwSelected = "JE1"
                    myFwSelected2 = "JE"
                    myFwSelected3 = "(13)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.JE1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
                'read FLUIDICS
                If pInstructionSent.Contains("ID:14;") Then 'SF1
                    myFwSelected = "SF1"
                    myFwSelected2 = "SF"
                    myFwSelected3 = "(14)"
                    SimulatedResponse = ANSF_Generator(POLL_IDs.SF1)
                    System.Threading.Thread.Sleep(1000) ' 1 second
                    If SimulatedResponse.Length > 0 Then
                        myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                        SimulatedResponse = ""
                    End If
                End If
            End If





            'FOR TESTING
            If pInstructionSent.Contains("CODEBR") Then

                'CODE BAR
                If pInstructionSent.Contains("R:1;") Then 'REAGENTS

                    If pInstructionSent.Contains("A:1;") Then 'CONFIG
                        SimulatedResponse = ANSCBR_Generator("1", "1")
                    End If

                    If pInstructionSent.Contains("A:4;") Then 'TEST MODE
                        SimulatedResponse = ANSCBR_Generator("1", "5")
                    End If

                    If pInstructionSent.Contains("A:5;") Then 'END TEST MODE
                        SimulatedResponse = ANSCBR_Generator("1", "6")
                    End If

                End If

                If pInstructionSent.Contains("R:2;") Then 'SAMPLES

                    If pInstructionSent.Contains("A:1;") Then 'CONFIG
                        SimulatedResponse = ANSCBR_Generator("2", "1")
                    End If

                    If pInstructionSent.Contains("A:4;") Then 'TEST MODE
                        SimulatedResponse = ANSCBR_Generator("2", "5")
                    End If

                    If pInstructionSent.Contains("A:5;") Then 'END TEST MODE
                        SimulatedResponse = ANSCBR_Generator("2", "6")
                    End If

                End If

                System.Threading.Thread.Sleep(1000) ' 1 second
                If SimulatedResponse.Length > 0 Then
                    'StartAutoResponseTimer()
                    myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                    SimulatedResponse = ""
                End If
            End If


            'FOR TESTING
            If pInstructionSent.Contains("CONFIG") Then
                'CONFIG
                SimulatedResponse = ANSCONFIG_Generator()

                System.Threading.Thread.Sleep(1000) ' 1 second
                If SimulatedResponse.Length > 0 Then
                    'StartAutoResponseTimer()
                    myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                    SimulatedResponse = ""
                End If
            End If

            'If pInstructionSent.Contains("READSENSOR") Then
            '    Dim mySensorsList As New List(Of SENSOR)
            '    If pInstructionSent.Contains("WSL") Then
            '        mySensorsList.Add(SENSOR.WASHING_SOLUTION_LEVEL)
            '    End If
            '    If pInstructionSent.Contains("HCL") Then
            '        mySensorsList.Add(SENSOR.HIGH_CONTAMINATION_LEVEL)
            '    End If
            '    If pInstructionSent.Contains("DWL1") Then
            '        mySensorsList.Add(SENSOR.DISTILLED_WATER_EMPTY)
            '        IntermediateTanksTest_ON = True
            '    End If
            '    If pInstructionSent.Contains("DWL2") Then
            '        mySensorsList.Add(SENSOR.DISTILLED_WATER_FULL)
            '        IntermediateTanksTest_ON = True
            '    End If
            '    If pInstructionSent.Contains("LCL1") Then
            '        mySensorsList.Add(SENSOR.LOW_CONTAMINATION_EMPTY)
            '        IntermediateTanksTest_ON = True
            '    End If
            '    If pInstructionSent.Contains("LCL2") Then
            '        mySensorsList.Add(SENSOR.LOW_CONTAMINATION_FULL)
            '        IntermediateTanksTest_ON = True
            '    End If

            '    SensorDataGenerator(mySensorsList)
            'End If

            'If pInstructionSent.Contains("TANKSTESTEXIT") Then
            '    IntermediateTanksTest_ON = False
            '    IntermediateTanksTime = 0
            'End If

            'If pInstructionSent.Contains("ABSORBANCE") Then
            '    AbsorbanceRequested = True
            'End If

            'If pInstructionSent.Contains("READADJ") Then
            '    AdjustmentsRequested = True
            'End If

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub

    Private Sub BsCommTestings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCommTestings.Click
        Try
            Me.Cursor = Cursors.WaitCursor

            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                Dim myGlobal As New GlobalDataTO
                Dim myInstruction As String = BsInstruction.Text.ToUpper.Trim

                Select Case myInstruction
                    Case "CONNECT"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT, True) 'CONNECT

                    Case "STATE"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'STATE

                    Case "STANDBY"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True) 'STANDBY

                    Case "SLEEP"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True) 'SLEEP

                    Case "PAUSE"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE, True) 'PAUSE

                    Case "RUNNING"
                        'myAnalyzerManager.ActiveAnalyzer = Ax00MainMDI.ActiveAnalyzer
                        'myAnalyzerManager.ActiveWorkSession = Ax00MainMDI.ActiveWorkSession

                        'myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True) 'RUNNING

                    Case "START"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.START, True) 'START

                    Case "TEST"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION, True) 'START

                    Case "ENDRUN"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True) 'ENDRUN

                    Case "ABORT"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True) 'ENDRUN


                    Case "SCRIPT"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND, True, Nothing, "", Me.BsAction.Text)

                    Case Else
                End Select

                Dim myTitle As String = ""
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    If Not myAnalyzerManager.Connected Then
                        myGlobal.ErrorCode = "ERROR_COMM"
                        myTitle = "Warning"
                    End If

                ElseIf myGlobal.HasError Then
                    myTitle = "Error"
                End If

                If myTitle <> "" Then
                    Me.ShowMessage(myTitle, myGlobal.ErrorCode)
                Else
                    'BsReceivedTextBox.Text += myAnalyzerManager.InstructionSent & vbCrLf
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsCommTestings_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsCommTestings_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            Me.Cursor = Cursors.Default

        End Try

    End Sub

    Private Sub BsClearReception_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsClearReception.Click
        Try
            BsReceivedTextBox.Clear()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsClearReception_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsClearReception_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub

    Private Sub BsRestore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRestore.Click
        Try
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                myAnalyzerManager.ClearQueueToSend()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsRestore_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsRestore_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


#End Region

    Private Sub BsListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsListBox1.SelectedIndexChanged
        Dim str As String
        str = BsListBox1.SelectedItem.ToString
        BsTextWrite_End.Text = str
    End Sub

    'FOR SIMULATING
    Private myLastWSValue As Double = 512
    Private myLastHCValue As Double = 512

    'Private Enum TEST_SENSORS
    '    BALANCE_TANKS_LEVELS
    '    INTERMEDIATE_TANKS_LEVELS
    'End Enum

    Private IntermediateTanksTest_ON As Boolean = False
    Private IntermediateTanksTime As Integer = 0

    Private Sub AdjustmentsGenerator()

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myDataBuilder As New System.Text.StringBuilder
            myGlobal = MyClass.myFwAdjustmentsDelegate.ConvertDSToString()
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myData As String = CStr(myGlobal.SetDatos)
                If myData.Length > 0 Then

                    'split into lines
                    Dim myLines() As String = myData.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)

                    'the data comes with lines and comments
                    For L As Integer = 0 To myLines.Length - 1 Step 1
                        Dim myAdjustmentLine As String = ""

                        Dim myLine As String = myLines(L).Trim

                        If myLine.Length > 0 Then

                            If Not myLine.Trim.StartsWith("--") Then 'header, comments

                                If Not myLine.Trim.StartsWith("{") Then 'group

                                    myAdjustmentLine = myLine

                                    myAdjustmentLine = myAdjustmentLine.Substring(0, myAdjustmentLine.IndexOf("{")).Trim

                                    myDataBuilder.Append(myAdjustmentLine)


                                End If
                            End If
                        End If
                    Next

                End If

                myData = myDataBuilder.ToString
                'myData = myData.Replace(";", "|") 'TESTING!!! para que pase el filtro de sintaxis
                'myData = myData.Replace(":", ">") 'TESTING!!! para que pase el filtro de sintaxis
                myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;" & GlobalEnumerates.AppLayerInstrucionReception.ANSADJ.ToString & _
                                                                          ";" & myData)
                If Me.BsANSADJCheckbox.Checked Then
                    BsReceivedTextBox.Text += Now.ToString("hh:mm:ss") & vbTab & "<< " & "A400;ANSADJ;" & myData & vbCrLf
                End If

            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'SENSORS
    Private AnalyzerSensorNumericalValues As New Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single)

    Private Enum MonitorSensors

        GC  'General Cover
        '0	Closed
        '1	Open

        PC  'Photometrics Cover
        '0	Closed
        '1	Open

        RC  'Reagents Rotor Cover
        '0	Closed
        '1	Open

        SC  'Samples Rotor Cover
        '0	Closed
        '1	Open

        HS  'System Liquid Sensor
        '0	Low sensor Down - High Sensor Down (Empty)
        '1	Low sensor High - High Sensor Down (Middle)
        '2	Low Sensor Down - High sensor High (En Running es un estado imposible - error) (En servicio es posible con manipulación)
        '3	Both sensors High (Full)

        WS  'Waste Sensor
        '0	Low sensor Down - High Sensor Down (Empty)
        '1	Low sensor High - High Sensor Down (Middle)
        '2	Low Sensor Down - High sensor High (En Running es un estado imposible - error) (En servicio es posible con manipulación)
        '3	Both sensors High (Full)

        SW  'Washing Solution Weigth
        '0-4096	

        WW  'High contamination Waste Weigth
        '0-4096	

        PT  'Photometrics Temperature
        '20-45	ºC

        FS  'Fridge Status
        '0	Off
        '1	ON

        FT  'Fridge Temperature
        '0-45	ºC

        HT  'Washing Station Heater Temperature
        '20-60	ºC

        R1T 'R1 Probe Temperature
        '20-45	ºC

        R2T 'R2 Probe Temperature
        '20-45	ºC

        ISE 'ISE Status
        '0	Off
        '1	ON

    End Enum

    Private Sub ANSINF_Generator()
        Try
            Dim Rnd As New Random()

            '1  GC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_GENERAL) = CInt(Rnd.Next(0, 2)) ' Now.Second Mod 30)

            '2  PC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_REACTIONS) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 6)

            '3  RC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_FRIDGE) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 12)

            '4  SC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_SAMPLES) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 4)

            '5  HS
            AnalyzerSensorNumericalValues(AnalyzerSensors.WATER_DEPOSIT) = CInt(Rnd.Next(0, 4)) 'Now.Second Mod 5)

            '6  WS
            AnalyzerSensorNumericalValues(AnalyzerSensors.WASTE_DEPOSIT) = CInt(Rnd.Next(0, 4)) 'Now.Second Mod 5)

            '7 SW
            AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_WASHSOLUTION) = CSng(Rnd.Next(1000, 1800)) '1000 * (Now.Second / 60))

            '8 WW
            AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) = CSng(Rnd.Next(1000, 1800)) '1000 * (1 - (Now.Second / 60)))

            '9  PT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_REACTIONS) = CSng(Rnd.Next(35, 40)) '37.2 - Rnd(Now.Second))

            '10 FS
            AnalyzerSensorNumericalValues(AnalyzerSensors.FRIDGE_STATUS) = CSng(Rnd.Next(0, 2)) '1)

            '11 FT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_FRIDGE) = CSng(Rnd.Next(0, 10)) '15 + Rnd(Now.Second))

            '12 HT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_WASHINGSTATION) = CSng(Rnd.Next(35, 55)) '27.6 + Rnd(Now.Second))

            '13 R1T
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R1) = CSng(Rnd.Next(35, 40)) '34.6 + Rnd(Now.Second))

            '14 R2T
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R2) = CSng(Rnd.Next(35, 40)) '35.7 - Rnd(Now.Second))

            '15 IS
            AnalyzerSensorNumericalValues(AnalyzerSensors.ISE_STATUS) = 1 ' CSng(Rnd.Next(0, 2)) '1)

            Dim myData As String = ""
            myData &= "GC:" & AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_GENERAL).ToString & ";"
            myData &= "PC:" & AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_REACTIONS).ToString & ";"
            myData &= "RC:" & AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_FRIDGE).ToString & ";"
            myData &= "SC:" & AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_SAMPLES).ToString & ";"
            myData &= "HS:" & AnalyzerSensorNumericalValues(AnalyzerSensors.WATER_DEPOSIT).ToString & ";"
            myData &= "WS:" & AnalyzerSensorNumericalValues(AnalyzerSensors.WASTE_DEPOSIT).ToString & ";"
            myData &= "SW:" & AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_WASHSOLUTION).ToString & ";"
            myData &= "WW:" & AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE).ToString & ";"
            myData &= "PT:" & AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_REACTIONS).ToString & ";"
            myData &= "FS:" & AnalyzerSensorNumericalValues(AnalyzerSensors.FRIDGE_STATUS).ToString & ";"
            myData &= "FT:" & AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_FRIDGE).ToString & ";"
            myData &= "HT:" & AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_WASHINGSTATION).ToString & ";"
            myData &= "R1T:" & AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R1).ToString & ";"
            myData &= "R2T:" & AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R2).ToString & ";"
            myData &= "IS:" & AnalyzerSensorNumericalValues(AnalyzerSensors.ISE_STATUS).ToString & ";"

            If myData.Length > 1 Then
                Dim myGlobal As New GlobalDataTO
                myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSINF;" & myData)

                If Me.BsANSINFOCheckbox.Checked Then
                    BsReceivedTextBox.Text += Now.ToString("hh:mm:ss") & vbTab & "<< " & "A400;ANSINF;" & myData & vbCrLf
                End If

            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub SLEEPING_Generator()
        Try
            Dim myGlobal As New GlobalDataTO

            'start response
            MyClass.SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:3;T:5000;C:0;W:0;R:0;E:0;I:1;"
            myGlobal = StartAutoResponseTimer()

            Application.DoEvents()
            CurrentStatus = AnalyzerManagerStatus.SLEEPING

            'end response
            MyClass.SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:4;T:0;C:0;W:0;R:0;E:0;I:1;"
            myGlobal = StartProcessResponseTimer(5000)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub STANDBY_Generator()
        Try
            Dim myGlobal As New GlobalDataTO

            'start response
            MyClass.SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:5;T:5000;C:0;W:0;R:0;E:0;I:1;"
            myGlobal = StartAutoResponseTimer()

            Application.DoEvents()
            CurrentStatus = AnalyzerManagerStatus.STANDBY

            'end response
            MyClass.SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:6;T:0;C:0;W:0;R:0;E:0;I:1;"
            myGlobal = StartProcessResponseTimer(5000)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub InstructionStartEnd_Generator(ByVal pAction As AnalyzerManagerAx00Actions)
        Try
            Dim myGlobal As New GlobalDataTO

            MyClass.SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:" & CInt(pAction).ToString & ";T:5000;C:0;W:0;R:0;E:0;I:1;"
            myGlobal = StartAutoResponseTimer()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub LOADADJ_Generator(ByVal pString As String)
        Try

            If pString.Trim.Length = 0 Then Exit Sub

            Dim myGlobal As New GlobalDataTO
            Dim myAdjString As String = pString.Substring(pString.IndexOf("LOADADJ") + 8).Trim

            Dim Adjs() As String = myAdjString.Split(CChar(";"))
            For a As Integer = 0 To Adjs.Length - 1
                Dim myCodeFw As String
                Dim myValue As String
                If Adjs(a).Trim.Length > 0 Then
                    myCodeFw = Adjs(a).Substring(0, Adjs(a).IndexOf(":"))
                    myValue = Adjs(a).Substring(Adjs(a).IndexOf(":") + 1)
                    If myCodeFw.Trim.Length > 0 And myValue.Trim.Length > 0 Then
                        MyClass.AnalyzerAdjustmentsDS.BeginInit()
                        For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.AnalyzerAdjustmentsDS.srv_tfmwAdjustments.Rows
                            If myCodeFw.ToUpper.Trim = R.CodeFw.ToUpper.Trim Then
                                R.Value = myValue.Trim
                                Exit For
                            End If
                        Next
                        MyClass.AnalyzerAdjustmentsDS.EndInit()
                        MyClass.AnalyzerAdjustmentsDS.AcceptChanges()
                    End If
                End If

            Next

            CurrentStatus = AnalyzerManagerStatus.STANDBY

            'start response
            SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:52;T:1000;C:0;W:0;R:0;E:0;I:1;"
            AutoCommandResponse()
            'StartAutoResponseTimer()

            Application.DoEvents()

            'end response
            SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:53;T:0;C:0;W:0;R:0;E:0;I:1;"
            StartProcessResponseTimer(5000)


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub LOADFACTORYADJ_Generator()
        Try
            Dim myGlobal As New GlobalDataTO

            'start response
            SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:102;T:1000;C:0;W:0;R:0;E:0;I:1;"
            AutoCommandResponse()
            'StartAutoResponseTimer()

            Application.DoEvents()

            'end response
            SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:103;T:0;C:0;W:0;R:0;E:0;I:1;"
            StartProcessResponseTimer(1000)


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub UPDATEFW_Generator()
        Try
            Dim myGlobal As New GlobalDataTO

            'start response
            SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:104;T:10000;C:0;W:0;R:0;E:0;I:1;"
            AutoCommandResponse()
            'StartAutoResponseTimer()

            Application.DoEvents()

            'end response
            SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:105;T:0;C:0;W:0;R:0;E:0;I:1;"
            StartProcessResponseTimer(10000)


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub RESET_Generator()
        Try
            Dim myGlobal As New GlobalDataTO

            'start response
            SimulatedResponse = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:100;T:1000;C:0;W:0;R:0;E:0;I:1;"
            AutoCommandResponse()
            'StartAutoResponseTimer()

            Application.DoEvents()
            CurrentStatus = AnalyzerManagerStatus.SLEEPING

            'end response
            SimulatedResponse2 = "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:101;T:0;C:0;W:0;R:0;E:0;I:1;"
            StartProcessResponseTimer(1000)


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private FirstTimeManifold As Boolean = True
    Private FirstTimeFluidics As Boolean = True
    Private FirstTimePhotometrics As Boolean = True

    'MANIFOLD
    Private ManifoldNumericalValues As New Dictionary(Of GlobalEnumerates.MANIFOLD_ELEMENTS, String)

    'FLUIDICS
    Private FluidicsNumericalValues As New Dictionary(Of GlobalEnumerates.FLUIDICS_ELEMENTS, String)

    'PHOTOMETRICS
    Private PhotometricsNumericalValues As New Dictionary(Of GlobalEnumerates.PHOTOMETRICS_ELEMENTS, String)

    ' CPU VALUES
    Private CpuValues As New Dictionary(Of GlobalEnumerates.CPU_ELEMENTS, String)
    ' ARMS VALUES
    Private ArmsValues As New Dictionary(Of GlobalEnumerates.ARMS_ELEMENTS, String)
    ' PROBES VALUES
    Private ProbesValues As New Dictionary(Of GlobalEnumerates.PROBES_ELEMENTS, String)
    ' ROTORS VALUES
    Private RotorsValues As New Dictionary(Of GlobalEnumerates.ROTORS_ELEMENTS, String)
    ' FIRMWARE VALUES
    Private FwValues As New Dictionary(Of GlobalEnumerates.FW_INFO, String)


    'PENDIENTE
    Private Function ANSJEX_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try

            If FirstTimeManifold Then
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_TEMP) = (75 * Rnd(24.6)).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1) = HW_DC_STATES.EN.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1H) = HW_MOTOR_HOME_STATES.C.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A) = "1345"
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2) = HW_DC_STATES.EN.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2H) = HW_MOTOR_HOME_STATES.C.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A) = "2365"
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MS) = HW_DC_STATES.EN.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSH) = HW_MOTOR_HOME_STATES.C.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA) = "997"
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV6) = CInt(HW_BOOL_STATES._OFF).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV6D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE1) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE2) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE3) = HW_DC_STATES.DI.ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLT) = "3778"
                ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLTD) = HW_CLOT_DIAGNOSIS.OK.ToString


                FirstTimeManifold = False
            End If

            Dim myData As String = ""
            myData &= "TMP:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_TEMP) & ";"
            myData &= "MR1:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1) & ";"
            myData &= "MR1H:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1H) & ";"
            myData &= "MR1A:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A) & ";"
            myData &= "MR2:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2) & ";"
            myData &= "MR2H:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2H) & ";"
            myData &= "MR2A:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A) & ";"
            myData &= "MS:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MS) & ";"
            myData &= "MSH:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSH) & ";"
            myData &= "MSA:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA) & ";"
            myData &= "B1:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1) & ";"
            myData &= "B1D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1D) & ";"
            myData &= "B2:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2) & ";"
            myData &= "B2D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2D) & ";"
            myData &= "B3:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3) & ";"
            myData &= "B3D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3D) & ";"
            myData &= "E1:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1) & ";"
            myData &= "E1D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1D) & ";"
            myData &= "E2:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2) & ";"
            myData &= "E2D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2D) & ";"
            myData &= "E3:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3) & ";"
            myData &= "E3D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3D) & ";"
            myData &= "E4:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4) & ";"
            myData &= "E4D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4D) & ";"
            myData &= "E5:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5) & ";"
            myData &= "E5D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5D) & ";"
            myData &= "E6:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV6) & ";"
            myData &= "E6D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV6D) & ";"
            myData &= "GE1:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE1) & ";"
            myData &= "GE1D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE1D) & ";"
            myData &= "GE2:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE2) & ";"
            myData &= "GE2D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE2D) & ";"
            myData &= "GE3:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE3) & ";"
            myData &= "GE3D:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_GE3D) & ";"
            myData &= "CLT:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLT) & ";"
            myData &= "CLTD:" & ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLTD) & ";"


            If myData.Length > 1 Then

                Return "A400;ANSJEX;ID:13;" & myData

            Else
                Return ""
            End If

            'myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSJEX;ID:JE1;TMP:27.3;M1:2106;M2:897;M3:1506;B1:0;B2:0;B3:1;E1:0;E2:0;E3:0;E4:0;E5:0;GE:0;GEM1:2000;")

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Sub UpdateManifold(ByVal pElement As String, ByVal pValue As String, Optional ByVal pRelative As Boolean = False)
        Try
            Select Case pElement.ToUpper.Trim
                Case MANIFOLD_ELEMENTS.JE1_MR1.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1) = pValue


                Case MANIFOLD_ELEMENTS.JE1_MR1H.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1H) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A) = "0"
                    End If

                Case MANIFOLD_ELEMENTS.JE1_MR1A.ToString
                    If Not pRelative Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A) = pValue
                    Else
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A) = (CInt(ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1A)) + CInt(pValue)).ToString
                    End If
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR1H) = HW_MOTOR_HOME_STATES.O.ToString

                Case MANIFOLD_ELEMENTS.JE1_MR2.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2) = pValue


                Case MANIFOLD_ELEMENTS.JE1_MR2H.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2H) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A) = "0"
                    End If

                Case MANIFOLD_ELEMENTS.JE1_MR2A.ToString
                    If Not pRelative Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A) = pValue
                    Else
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A) = (CInt(ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2A)) + CInt(pValue)).ToString
                    End If
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MR2H) = HW_MOTOR_HOME_STATES.O.ToString

                Case MANIFOLD_ELEMENTS.JE1_MS.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MS) = pValue

                Case MANIFOLD_ELEMENTS.JE1_MSH.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSH) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA) = "0"
                    End If

                Case MANIFOLD_ELEMENTS.JE1_MSA.ToString
                    If Not pRelative Then
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA) = pValue
                    Else
                        ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA) = (CInt(ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSA)) + CInt(pValue)).ToString
                    End If
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_MSH) = HW_MOTOR_HOME_STATES.O.ToString

                Case MANIFOLD_ELEMENTS.JE1_B1.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1) = pValue

                Case MANIFOLD_ELEMENTS.JE1_B1D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B1D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_B2.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2) = pValue

                Case MANIFOLD_ELEMENTS.JE1_B2D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B2D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_B3.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3) = pValue

                Case MANIFOLD_ELEMENTS.JE1_B3D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_B3D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV1.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV1D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV1D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV2.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV2D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV2D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV3.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV3D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV3D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV4.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV4D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV4D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV5.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5) = pValue

                Case MANIFOLD_ELEMENTS.JE1_EV5D.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_EV5D) = pValue

                Case MANIFOLD_ELEMENTS.JE1_CLT.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLT) = pValue

                Case MANIFOLD_ELEMENTS.JE1_CLTD.ToString
                    ManifoldNumericalValues(MANIFOLD_ELEMENTS.JE1_CLTD) = pValue

            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'PENDIENTE
    Private Function ANSSFX_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try

            If FirstTimeFluidics Then

                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_TEMP) = (80 * Rnd(24.6)).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MS) = HW_DC_STATES.EN.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSH) = HW_MOTOR_HOME_STATES.C.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA) = "2099"
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1) = CInt(HW_BOOL_STATES._OFF).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2) = CInt(HW_BOOL_STATES._OFF).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3) = CInt(HW_BOOL_STATES._OFF).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTH) = "37"
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTHD) = CInt(HW_THERMISTROR_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSH) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSHD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSW) = "554"
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSWD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCW) = "489"
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCWD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WAS) = CInt(HW_TANK_LEVEL_SENSOR_STATES.EMPTY).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WTS) = HW_DC_STATES.EN.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_SLS) = CInt(HW_TANK_LEVEL_SENSOR_STATES.FULL).ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_STS) = HW_DC_STATES.EN.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST1) = HW_DC_STATES.DI.ToString
                FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST2) = HW_DC_STATES.DI.ToString

                FirstTimeFluidics = False

            End If






            Dim myData As String = ""
            myData &= "TMP:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_TEMP) & ";"
            myData &= "MS:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MS) & ";"
            myData &= "MSH:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSH) & ";"
            myData &= "MSA:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA) & ";"
            myData &= "B1:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1) & ";"
            myData &= "B1D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1D) & ";"
            myData &= "B2:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2) & ";"
            myData &= "B2D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2D) & ";"
            myData &= "B3:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3) & ";"
            myData &= "B3D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3D) & ";"
            myData &= "B4:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4) & ";"
            myData &= "B4D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4D) & ";"
            myData &= "B5:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5) & ";"
            myData &= "B5D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5D) & ";"
            myData &= "B6:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6) & ";"
            myData &= "B6D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6D) & ";"
            myData &= "B7:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7) & ";"
            myData &= "B7D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7D) & ";"
            myData &= "B8:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8) & ";"
            myData &= "B8D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8D) & ";"
            myData &= "B9:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9) & ";"
            myData &= "B9D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9D) & ";"
            myData &= "B10:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10) & ";"
            myData &= "B10D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10D) & ";"
            myData &= "GE1:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1) & ";"
            myData &= "GE1D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1D) & ";"
            myData &= "E1:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1) & ";"
            myData &= "E1D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1D) & ";"
            myData &= "E2:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2) & ";"
            myData &= "E2D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2D) & ";"
            myData &= "E3:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3) & ";"
            myData &= "E3D:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3D) & ";"
            myData &= "WSTH:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTH) & ";"
            myData &= "WSTHD:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTHD) & ";"
            myData &= "WSH:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSH) & ";"
            myData &= "WSHD:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSHD) & ";"
            myData &= "WSW:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSW) & ";"
            myData &= "WSWD:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSWD) & ";"
            myData &= "HCW:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCW) & ";"
            myData &= "HCWD:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCWD) & ";"
            myData &= "WAS:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WAS) & ";"
            myData &= "WTS:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WTS) & ";"
            myData &= "SLS:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_SLS) & ";"
            myData &= "STS:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_STS) & ";"
            myData &= "ST1:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST1) & ";"
            myData &= "ST2:" & FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST2) & ";"


            If myData.Length > 1 Then
                Return "A400;ANSSFX;ID:14;" & myData
            Else
                Return ""
            End If


            'myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSSFX;ID:SF1;TMP:27.3;M1:1998;B1:0;B2:0;B3:0;B4:0;B5:0;B6:0;B7:0;B8:0;B9:0;B10:0;E1:0;E2:0;")

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Sub UpdateFluidics(ByVal pElement As String, ByVal pValue As String, Optional ByVal pRelative As Boolean = False)
        Try
            Select Case pElement.ToUpper.Trim
                Case FLUIDICS_ELEMENTS.SF1_MS.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MS) = pValue

                Case FLUIDICS_ELEMENTS.SF1_MSH.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSH) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA) = "0"
                    End If

                Case FLUIDICS_ELEMENTS.SF1_MSA.ToString
                    If Not pRelative Then
                        FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA) = pValue
                    Else
                        FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA) = (CInt(FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSA)) + CInt(pValue)).ToString
                    End If
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_MSH) = HW_MOTOR_HOME_STATES.O.ToString

                Case FLUIDICS_ELEMENTS.SF1_B1.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B1D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B1D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B2.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B2D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B2D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B3.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B3D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B3D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B4.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B4D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B4D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B5.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B5D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B5D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B6.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B6D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B6D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B7.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B7D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B7D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B8.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B8D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B8D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B9.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B9D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B9D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B10.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10) = pValue

                Case FLUIDICS_ELEMENTS.SF1_B10D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_B10D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV1.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV1D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV1D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_GE1.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1) = pValue

                Case FLUIDICS_ELEMENTS.SF1_GE1D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_GE1D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV2.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV2D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV2D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV3.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3) = pValue

                Case FLUIDICS_ELEMENTS.SF1_EV3D.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_EV3D) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSTH.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTH) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSTHD.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSTHD) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSH.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSH) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSHD.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSHD) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSW.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSW) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WSWD.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WSWD) = pValue

                Case FLUIDICS_ELEMENTS.SF1_HCW.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCW) = pValue

                Case FLUIDICS_ELEMENTS.SF1_HCWD.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_HCWD) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WAS.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WAS) = pValue

                Case FLUIDICS_ELEMENTS.SF1_WTS.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_WTS) = pValue

                Case FLUIDICS_ELEMENTS.SF1_SLS.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_SLS) = pValue

                Case FLUIDICS_ELEMENTS.SF1_STS.ToString
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_STS) = pValue

                Case FLUIDICS_ELEMENTS.SF1_ST1.ToString 'agitator 1
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST1) = pValue

                Case FLUIDICS_ELEMENTS.SF1_ST2.ToString 'agitator 2
                    FluidicsNumericalValues(FLUIDICS_ELEMENTS.SF1_ST2) = pValue



            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function ANSGLF_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try

            If FirstTimePhotometrics Then


                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_TEMP) = (100 * Rnd(24.6)).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MR) = HW_DC_STATES.EN.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRH) = HW_MOTOR_HOME_STATES.C.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA) = "1897"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) = "1897"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRED) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MW) = HW_DC_STATES.DI.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWH) = HW_MOTOR_HOME_STATES.C.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA) = "2568"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_CD) = HW_COLLISION_STATES.C.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTH) = "37"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTHD) = CInt(HW_THERMISTROR_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PH) = HW_DC_STATES.DI.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHD) = CInt(HW_PELTIER_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1) = "547"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2) = "547"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3) = "547"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4) = "547"
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_RC) = HW_SWITCH_STATES.C.ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHT) = CInt(HW_PHOTOMETRY_STATES.OK).ToString
                PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHFM) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString


                FirstTimePhotometrics = False

            End If


            Dim myData As String = ""
            myData &= "TMP:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_TEMP) & ";"
            myData &= "MR:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MR) & ";"
            myData &= "MRH:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRH) & ";"
            myData &= "MRA:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA) & ";"
            myData &= "MRE:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) & ";"
            myData &= "MRED:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRED) & ";"
            myData &= "MW:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MW) & ";"
            myData &= "MWH:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWH) & ";"
            myData &= "MWA:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA) & ";"
            myData &= "CD:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_CD) & ";"
            myData &= "PTH:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTH) & ";"
            myData &= "PTHD:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTHD) & ";"
            myData &= "PH:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PH) & ";"
            myData &= "PHD:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHD) & ";"
            myData &= "PF1:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1) & ";"
            myData &= "PF1D:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1D) & ";"
            myData &= "PF2:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2) & ";"
            myData &= "PF2D:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2D) & ";"
            myData &= "PF3:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3) & ";"
            myData &= "PF3D:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3D) & ";"
            myData &= "PF4:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4) & ";"
            myData &= "PF4D:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4D) & ";"
            myData &= "RC:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_RC) & ";"
            myData &= "PHT:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHT) & ";"
            myData &= "PHFM:" & PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHFM) & ";"

            If myData.Length > 1 Then

                Return "A400;ANSGLF;ID:12;" & myData

            Else
                Return ""
            End If



        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Sub UpdatePhotometrics(ByVal pElement As String, ByVal pValue As String, Optional ByVal pRelative As Boolean = False)
        Try
            Select Case pElement.ToUpper.Trim
                Case PHOTOMETRICS_ELEMENTS.GLF_MR.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MR) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_MRH.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRH) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA) = "0"
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) = "0"
                    End If

                Case PHOTOMETRICS_ELEMENTS.GLF_MRA.ToString
                    If Not pRelative Then
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA) = pValue
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) = pValue
                    Else
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA) = (CInt(PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA)) + CInt(pValue)).ToString
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) = PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRA)
                    End If
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRH) = HW_MOTOR_HOME_STATES.O.ToString

                Case PHOTOMETRICS_ELEMENTS.GLF_MRE.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRE) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_MRED.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MRED) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_MW.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MW) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_MWH.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWH) = pValue
                    If pValue = HW_MOTOR_HOME_STATES.C.ToString Then
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA) = "0"
                    End If

                Case PHOTOMETRICS_ELEMENTS.GLF_MWA.ToString
                    If Not pRelative Then
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA) = pValue
                    Else
                        PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA) = (CInt(PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWA)) + CInt(pValue)).ToString
                    End If
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_MWH) = HW_MOTOR_HOME_STATES.O.ToString

                Case PHOTOMETRICS_ELEMENTS.GLF_CD.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_CD) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PTH.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTH) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PTHD.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PTHD) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PH.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PH) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PHD.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHD) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF1.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF2.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF3.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF4.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF1D.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF1D) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF2D.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF2D) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF3D.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF3D) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PF4D.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PF4D) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_RC.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_RC) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PHT.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHT) = pValue

                Case PHOTOMETRICS_ELEMENTS.GLF_PHFM.ToString
                    PhotometricsNumericalValues(PHOTOMETRICS_ELEMENTS.GLF_PHFM) = pValue

            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private myCurrentUTILAction As UTILInstructionTypes = UTILInstructionTypes.None

    Private Function ANSUTIL_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try
            'A400;ANSUTIL;A:2;T:0;C:1;SN:0

            Dim myData As String = ""

            Select Case myCurrentUTILAction
                Case UTILInstructionTypes.IntermediateTanks 'NOT V1

                Case UTILInstructionTypes.NeedleCollisionTest

                    Dim myNeedle As UTILCollidedNeedles = UTILCollidedNeedles.None

                    myNeedle = CType(CInt(DateTime.Now.Second / 10), UTILCollidedNeedles)

                    myData &= "A:2;"
                    myData &= "T:0;"
                    myData &= "C:" & CInt(myNeedle).ToString & ";"
                    myData &= "SN:0;"
                    StartUTILTimer()

                Case UTILInstructionTypes.SaveSerialNumber
                    myData &= "A:2;"
                    myData &= "T:0;"
                    myData &= "C:0;"
                    myData &= "SN:1;" 'no Ok SN=2;

            End Select

            If myData.Length > 1 Then

                Return "A400;ANSUTIL;" & myData

            Else
                Return ""
            End If



        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSBXX_Generator() As String
        Dim myGlobal As New GlobalDataTO
        Try
            ArmsValues(ARMS_ELEMENTS.ID) = myArmSelected
            ArmsValues(ARMS_ELEMENTS.TMP) = "38"
            ArmsValues(ARMS_ELEMENTS.MH) = HW_DC_STATES.EN.ToString
            ArmsValues(ARMS_ELEMENTS.MHH) = HW_MOTOR_HOME_STATES.C.ToString
            ArmsValues(ARMS_ELEMENTS.MHA) = "1200"
            ArmsValues(ARMS_ELEMENTS.MV) = HW_DC_STATES.EN.ToString
            ArmsValues(ARMS_ELEMENTS.MVH) = HW_MOTOR_HOME_STATES.C.ToString
            ArmsValues(ARMS_ELEMENTS.MVA) = "300"

            Dim myData As String = ""
            myData &= "ID:" & ArmsValues(ARMS_ELEMENTS.ID).ToString & ";"
            myData &= "TMP:" & ArmsValues(ARMS_ELEMENTS.TMP).ToString & ";"
            myData &= "MH:" & ArmsValues(ARMS_ELEMENTS.MH).ToString & ";"
            myData &= "MHH:" & ArmsValues(ARMS_ELEMENTS.MHH).ToString & ";"
            myData &= "MHA:" & ArmsValues(ARMS_ELEMENTS.MHA).ToString & ";"
            myData &= "MV:" & ArmsValues(ARMS_ELEMENTS.MV).ToString & ";"
            myData &= "MVH:" & ArmsValues(ARMS_ELEMENTS.MVH).ToString & ";"
            myData &= "MVA:" & ArmsValues(ARMS_ELEMENTS.MVA).ToString & ";"

            If myData.Length > 1 Then
                Return "A400;ANSBXX;" & myData
            Else
                Return ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSDXX_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try
            ProbesValues(PROBES_ELEMENTS.ID) = myProbeSelected
            ProbesValues(PROBES_ELEMENTS.TMP) = "38"
            ProbesValues(PROBES_ELEMENTS.DST) = HW_DC_STATES.EN.ToString

            Select Case myProbeSelected
                Case "DM1" : ProbesValues(PROBES_ELEMENTS.DFQ) = (1880 + (85 * Rnd(999) * Rnd(1))).ToString
                Case "DR1" : ProbesValues(PROBES_ELEMENTS.DFQ) = (883 + (102 * Rnd(939) * Rnd(2))).ToString
                Case "DR2" : ProbesValues(PROBES_ELEMENTS.DFQ) = (805 + (152 * Rnd(299) * Rnd(9))).ToString
            End Select

            ProbesValues(PROBES_ELEMENTS.D) = HW_GENERIC_DIAGNOSIS.OK.ToString
            ProbesValues(PROBES_ELEMENTS.DCV) = "500"
            ProbesValues(PROBES_ELEMENTS.PTH) = "25"
            ProbesValues(PROBES_ELEMENTS.PTHD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            ProbesValues(PROBES_ELEMENTS.PH) = HW_DC_STATES.EN.ToString
            ProbesValues(PROBES_ELEMENTS.PHD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            ProbesValues(PROBES_ELEMENTS.CD) = HW_MOTOR_HOME_STATES.C.ToString

            Dim myData As String = ""
            myData &= "ID:" & ProbesValues(PROBES_ELEMENTS.ID).ToString & ";"
            myData &= "TMP:" & ProbesValues(PROBES_ELEMENTS.TMP).ToString & ";"
            myData &= "DST:" & ProbesValues(PROBES_ELEMENTS.DST).ToString & ";"
            myData &= "DFQ:" & ProbesValues(PROBES_ELEMENTS.DFQ).ToString & ";"
            myData &= "D:" & ProbesValues(PROBES_ELEMENTS.D).ToString & ";"
            myData &= "DCV:" & ProbesValues(PROBES_ELEMENTS.DCV).ToString & ";"
            myData &= "PTH:" & ProbesValues(PROBES_ELEMENTS.PTH).ToString & ";"
            myData &= "PTHD:" & ProbesValues(PROBES_ELEMENTS.PTHD).ToString & ";"
            myData &= "PH:" & ProbesValues(PROBES_ELEMENTS.PH).ToString & ";"
            myData &= "PHD:" & ProbesValues(PROBES_ELEMENTS.PHD).ToString & ";"
            myData &= "CD:" & ProbesValues(PROBES_ELEMENTS.CD).ToString & ";"

            If myData.Length > 1 Then
                Return "A400;ANSDXX;" & myData
            Else
                Return ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSRXX_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try
            RotorsValues(ROTORS_ELEMENTS.ID) = myRotorSelected
            RotorsValues(ROTORS_ELEMENTS.TMP) = "38"
            RotorsValues(ROTORS_ELEMENTS.MR) = HW_DC_STATES.EN.ToString
            RotorsValues(ROTORS_ELEMENTS.MRH) = HW_MOTOR_HOME_STATES.C.ToString
            RotorsValues(ROTORS_ELEMENTS.MRA) = "1345"
            RotorsValues(ROTORS_ELEMENTS.FTH) = "25"
            RotorsValues(ROTORS_ELEMENTS.FTHD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.FH) = HW_DC_STATES.EN.ToString
            RotorsValues(ROTORS_ELEMENTS.FHD) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.PF1) = "1000"
            RotorsValues(ROTORS_ELEMENTS.PF1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.PF2) = "2000"
            RotorsValues(ROTORS_ELEMENTS.PF2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.PF3) = "3000"
            RotorsValues(ROTORS_ELEMENTS.PF3D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.PF4) = "4000"
            RotorsValues(ROTORS_ELEMENTS.PF4D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.FF1) = "1001"
            RotorsValues(ROTORS_ELEMENTS.FF1D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.FF2) = "2002"
            RotorsValues(ROTORS_ELEMENTS.FF2D) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.RC) = HW_MOTOR_HOME_STATES.C.ToString
            RotorsValues(ROTORS_ELEMENTS.CB) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            RotorsValues(ROTORS_ELEMENTS.CBE) = ""

            Dim myData As String = ""
            myData &= "ID:" & RotorsValues(ROTORS_ELEMENTS.ID).ToString & ";"
            myData &= "TMP:" & RotorsValues(ROTORS_ELEMENTS.TMP).ToString & ";"
            myData &= "MR:" & RotorsValues(ROTORS_ELEMENTS.MR).ToString & ";"
            myData &= "MRH:" & RotorsValues(ROTORS_ELEMENTS.MRH).ToString & ";"
            myData &= "MRA:" & RotorsValues(ROTORS_ELEMENTS.MRA).ToString & ";"
            myData &= "FTH:" & RotorsValues(ROTORS_ELEMENTS.FTH).ToString & ";"
            myData &= "FTHD:" & RotorsValues(ROTORS_ELEMENTS.FTHD).ToString & ";"
            myData &= "FH:" & RotorsValues(ROTORS_ELEMENTS.FH).ToString & ";"
            myData &= "FHD:" & RotorsValues(ROTORS_ELEMENTS.FHD).ToString & ";"
            myData &= "PF1:" & RotorsValues(ROTORS_ELEMENTS.PF1).ToString & ";"
            myData &= "PF1D:" & RotorsValues(ROTORS_ELEMENTS.PF1D).ToString & ";"
            myData &= "PF2:" & RotorsValues(ROTORS_ELEMENTS.PF2).ToString & ";"
            myData &= "PF2D:" & RotorsValues(ROTORS_ELEMENTS.PF2D).ToString & ";"
            myData &= "PF3:" & RotorsValues(ROTORS_ELEMENTS.PF3).ToString & ";"
            myData &= "PF3D:" & RotorsValues(ROTORS_ELEMENTS.PF3D).ToString & ";"
            myData &= "PF4:" & RotorsValues(ROTORS_ELEMENTS.PF4).ToString & ";"
            myData &= "PF4D:" & RotorsValues(ROTORS_ELEMENTS.PF4D).ToString & ";"
            myData &= "FF1:" & RotorsValues(ROTORS_ELEMENTS.FF1).ToString & ";"
            myData &= "FF1D:" & RotorsValues(ROTORS_ELEMENTS.FF1D).ToString & ";"
            myData &= "FF2:" & RotorsValues(ROTORS_ELEMENTS.FF2).ToString & ";"
            myData &= "FF2D:" & RotorsValues(ROTORS_ELEMENTS.FF2D).ToString & ";"
            myData &= "RC:" & RotorsValues(ROTORS_ELEMENTS.RC).ToString & ";"
            myData &= "CB:" & RotorsValues(ROTORS_ELEMENTS.CB).ToString & ";"
            myData &= "CBE:" & RotorsValues(ROTORS_ELEMENTS.CBE).ToString & ";"

            If myData.Length > 1 Then
                Return "A400;ANSRXX;" & myData
            Else
                Return ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSCPU_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try
            CpuValues(CPU_ELEMENTS.CPU_TEMP) = "38"
            CpuValues(CPU_ELEMENTS.CPU_CAN) = HW_GENERIC_DIAGNOSIS.OK.ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_BM1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_BR1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_BR2) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_AG1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_AG2) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_DM1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_DR1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_DR2) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_RR1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_RM1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_GLF) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_SF1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_CAN_JE1) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_MC) = HW_MOTOR_HOME_STATES.C.ToString
            CpuValues(CPU_ELEMENTS.CPU_BUZ) = HW_DC_STATES.EN.ToString
            CpuValues(CPU_ELEMENTS.CPU_FWFM) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_BBFM) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_ISE) = CInt(HW_GENERIC_DIAGNOSIS.OK).ToString
            CpuValues(CPU_ELEMENTS.CPU_ISEE) = ""

            Dim myData As String = ""
            myData &= "ID:CPU;"
            myData &= "TMP:" & CpuValues(CPU_ELEMENTS.CPU_TEMP).ToString & ";"
            myData &= "CAN:" & CpuValues(CPU_ELEMENTS.CPU_CAN).ToString & ";"
            myData &= "BM1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_BM1).ToString & ";"
            myData &= "BR1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_BR1).ToString & ";"
            myData &= "BR2:" & CpuValues(CPU_ELEMENTS.CPU_CAN_BR2).ToString & ";"
            myData &= "AG1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_AG1).ToString & ";"
            myData &= "AG2:" & CpuValues(CPU_ELEMENTS.CPU_CAN_AG2).ToString & ";"
            myData &= "DM1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_DM1).ToString & ";"
            myData &= "DR1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_DR1).ToString & ";"
            myData &= "DR2:" & CpuValues(CPU_ELEMENTS.CPU_CAN_DR2).ToString & ";"
            myData &= "RR1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_RR1).ToString & ";"
            myData &= "RM1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_RM1).ToString & ";"
            myData &= "GLF:" & CpuValues(CPU_ELEMENTS.CPU_CAN_GLF).ToString & ";"
            myData &= "SF1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_SF1).ToString & ";"
            myData &= "JE1:" & CpuValues(CPU_ELEMENTS.CPU_CAN_JE1).ToString & ";"
            myData &= "MC:" & CpuValues(CPU_ELEMENTS.CPU_MC).ToString & ";"
            myData &= "BUZ:" & CpuValues(CPU_ELEMENTS.CPU_BUZ).ToString & ";"
            myData &= "FWFM:" & CpuValues(CPU_ELEMENTS.CPU_FWFM).ToString & ";"
            myData &= "BBFM:" & CpuValues(CPU_ELEMENTS.CPU_BBFM).ToString & ";"
            myData &= "ISE:" & CpuValues(CPU_ELEMENTS.CPU_ISE).ToString & ";"
            myData &= "ISEE:" & CpuValues(CPU_ELEMENTS.CPU_ISEE).ToString & ";"

            If myData.Length > 1 Then
                Return "A400;ANSCPU;" & myData
            Else
                Return ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSF_Generator(ByVal pBoard As POLL_IDs) As String
        Dim myGlobal As New GlobalDataTO
        Try
            FwValues(FW_INFO.ID) = myFwSelected
            FwValues(FW_INFO.SMC) = "987654321"

            FwValues(FW_INFO.FWV) = "0.0"
            FwValues(FW_INFO.FWCRC) = FW_GENERIC_RESULT.OK.ToString
            FwValues(FW_INFO.FWCRCV) = "0x3FD345A1"
            FwValues(FW_INFO.FWCRCS) = "4582"

            FwValues(FW_INFO.HWV) = "5"


            Dim myData As String = ""
            myData &= "ID:" & FwValues(FW_INFO.ID).ToString & ";"
            myData &= "SMC:" & FwValues(FW_INFO.SMC).ToString & ";"

            myData &= "FWV:" & FwValues(FW_INFO.FWV).ToString & ";"
            myData &= "FWCRC:" & FwValues(FW_INFO.FWCRC).ToString & ";"
            myData &= "FWCRCV:" & FwValues(FW_INFO.FWCRCV).ToString & ";"
            myData &= "FWCRCS:" & FwValues(FW_INFO.FWCRCS).ToString & ";"

            myData &= "HWV:" & FwValues(FW_INFO.HWV).ToString & ";"

            If myData.Length > 1 Then
                Dim myBoard As String = Nothing
                Select Case pBoard
                    Case POLL_IDs.BM1, POLL_IDs.BR1, POLL_IDs.BR2, POLL_IDs.AG1, POLL_IDs.AG2 : myBoard = "ANSFBX"
                    Case POLL_IDs.DM1, POLL_IDs.DR1, POLL_IDs.DR2 : myBoard = "ANSFDX"
                    Case POLL_IDs.RM1, POLL_IDs.RR1 : myBoard = "ANSFRX"
                    Case POLL_IDs.GLF : myBoard = "ANSFGL"
                    Case POLL_IDs.SF1 : myBoard = "ANSFSF"
                    Case POLL_IDs.JE1 : myBoard = "ANSFJE"
                End Select
                Return "A400;" & myBoard & ";" & myData
            Else
                Return ""
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSFCP_Generator() As String
        Dim myGlobal As New GlobalDataTO
        Try
            FwValues(FW_INFO.ID) = myFwSelected
            FwValues(FW_INFO.SMC) = "123456789"

            FwValues(FW_INFO.RV) = "0.78"
            FwValues(FW_INFO.CRC) = FW_GENERIC_RESULT.OK.ToString
            FwValues(FW_INFO.CRCV) = "0x12345ABC"
            FwValues(FW_INFO.CRCS) = "9864"

            FwValues(FW_INFO.FWV) = "0.7"
            FwValues(FW_INFO.FWCRC) = FW_GENERIC_RESULT.OK.ToString
            FwValues(FW_INFO.FWCRCV) = "0x3FD345A1"
            FwValues(FW_INFO.FWCRCS) = "4582"

            FwValues(FW_INFO.HWV) = "5"
            FwValues(FW_INFO.ASN) = "834999999"



            Dim myData As String = ""
            myData &= "ID:" & FwValues(FW_INFO.ID).ToString & ";"
            myData &= "SMC:" & FwValues(FW_INFO.SMC).ToString & ";"

            myData &= "RV:" & FwValues(FW_INFO.RV).ToString & ";"
            myData &= "CRC:" & FwValues(FW_INFO.CRC).ToString & ";"
            myData &= "CRCV:" & FwValues(FW_INFO.CRCV).ToString & ";"
            myData &= "CRCS:" & FwValues(FW_INFO.CRCS).ToString & ";"

            myData &= "FWV:" & FwValues(FW_INFO.FWV).ToString & ";"
            myData &= "FWCRC:" & FwValues(FW_INFO.FWCRC).ToString & ";"
            myData &= "FWCRCV:" & FwValues(FW_INFO.FWCRCV).ToString & ";"
            myData &= "FWCRCS:" & FwValues(FW_INFO.FWCRCS).ToString & ";"

            myData &= "HWV:" & FwValues(FW_INFO.HWV).ToString & ";"
            myData &= "ASN:" & FwValues(FW_INFO.ASN).ToString & ";"

            If myData.Length > 1 Then
                Return "A400;ANSFCP;" & myData
            Else
                Return ""
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private myFWUpdateAction As FwUpdateActions = FwUpdateActions.None

    Private Function ANSFWU_Generator() As String

        Dim myGlobal As New GlobalDataTO

        Try

            'System.Threading.Thread.Sleep(5000)

            Dim myCalculatedCRC As String = "0X48AC2AA8"

            If MyClass.myFWUpdateAction <> FwUpdateActions.None Then
                Dim myAction As String = "A:" & CInt(CInt(MyClass.myFWUpdateAction)).ToString & ";"
                Dim myResult As String
                Dim myCRC As String
                Dim myCPU As String
                Dim myPER As String
                Dim myMAN As String

                Select Case MyClass.myFWUpdateAction

                    Case FwUpdateActions.QueryCRC32
                        myResult = "S:0;"
                        myCRC = "CRC:" & myCalculatedCRC & ";"
                        myCPU = "CPU:0;"
                        myPER = "PER:0;"
                        myMAN = "MAN:0;"

                    Case FwUpdateActions.QueryNeeded
                        myResult = "S:0;"
                        myCRC = "CRC:0;"
                        myCPU = "CPU:1;"
                        myPER = "PER:1;"
                        myMAN = "MAN:1;"

                    Case Else
                        myResult = "S:0;"
                        myCRC = "CRC:0;"
                        myCPU = "CPU:0;"
                        myPER = "PER:0;"
                        myMAN = "MAN:0;"

                End Select

                'A400;ANSFWU;A:5;S:0;CRC:0;CPU:0;PER:0;MAN:0;
                Dim myData As String = myAction & myResult & myCRC & myCPU & myPER & myMAN
                If myData.Length > 1 Then
                    Return "A400;ANSFWU;" & myData
                Else
                    Return ""
                End If
            Else
                Return ""
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function STATUS_Generator(ByVal pStatus As AnalyzerManagerStatus, _
                                                ByVal pAction As AnalyzerManagerAx00Actions, _
                                                ByVal pTimeExpected As Integer) As String

        Dim myGlobal As New GlobalDataTO

        Try

            Dim myStatus As String = "S:" & CInt(pStatus).ToString & ";"
            Dim myAction As String = "A:" & CInt(pAction).ToString & ";"
            Dim myTime As String = "T:" & pTimeExpected.ToString & ";"

            'A400;STATUS;S:1;AC:90;T:5;C:0;W:0;R:0;E:0;I:0;
            Dim myData As String = myAction & myStatus & myAction & myTime & "C:0;W:0;R:0;E:0;I:0;"
            If myData.Length > 1 Then
                Return "A400;STATUS;" & myData
            Else
                Return ""
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Private Sub GenerateCommandAnswerData()
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    Dim Rnd As New Random()
                    Dim PhMain As Integer = CInt(Rnd.Next(950000, 1000000))
                    Dim PhRef As Integer = CInt(Rnd.Next(90000, 500000))
                    Dim EncoderCounts As Integer = CInt(Rnd.Next(0, 2))
                    Dim LevelDetected As Integer = CInt(Rnd.Next(0, 2))

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSCMD;END:1;" & _
                                                                              "MP:" & PhMain.ToString & ";" & _
                                                                              "RP:" & PhRef.ToString & ";" & _
                                                                              "ENC:" & EncoderCounts.ToString & ";" & _
                                                                              "LD:" & LevelDetected.ToString & ";")

                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub SimulateElementsUpdatement(ByVal pInstructionSent As String)
        Try

            If pInstructionSent.Contains("Action add in queue:") Then
                Exit Sub
            End If

            Dim ToSimulateResponse As Boolean = True

            Dim Instructions() As String = pInstructionSent.Split(CChar(";"))
            For c As Integer = 0 To Instructions.Length - 1 Step 1

                If Instructions(c).Length > 0 Then
                    If IsNumeric(Instructions(c)(0)) Then

                        'ToSimulateResponse = False

                        'MANIFOLD/FLUIDICS/PHOTOMETRICS ELEMENTS
                        If Instructions(c).Contains("JE1.") Or _
                            Instructions(c).Contains("SF1.") Or _
                            Instructions(c).Contains("GLF.") Then

                            Dim IsManifold As Boolean = Instructions(c).Contains("JE1.")
                            Dim IsFluidics As Boolean = Instructions(c).Contains("SF1.")
                            Dim IsPhotometrics As Boolean = Instructions(c).Contains("GLF.")

                            'ToSimulateResponse = True

                            Dim IsValue As Boolean = False
                            Dim myGroup As String = Instructions(c).Substring(Instructions(c).IndexOf(":") + 1, 4)
                            Dim i As Integer = Instructions(c).IndexOf(myGroup)
                            Dim subInstruc As String = Instructions(c).Substring(i)
                            Dim j As Integer = subInstruc.LastIndexOf(CChar(":")) 'if has parameters
                            If j < 0 Then j = subInstruc.LastIndexOf(CChar(".")) 'if not
                            Dim myElement As String = subInstruc.Substring(0, j)
                            Dim IsRelative As Boolean = False

                            Dim myValue As String = ""

                            'VALVES
                            If subInstruc.Contains("EON") Or subInstruc.Contains("EOF") Then
                                myElement = myElement.Replace(".", "_")
                                myElement = myElement.Replace("_E", "_EV")
                                If Instructions(c).Contains("EON") Then
                                    myValue = "1"
                                    IsValue = True
                                ElseIf Instructions(c).Contains("EOF") Then
                                    myValue = "0"
                                    IsValue = True
                                Else
                                    IsValue = False
                                End If
                            End If

                            'DC Activable elements (PUMPS, MOTORS, etc)
                            If Not IsValue Then
                                If subInstruc.Contains("DCS:") Or subInstruc.Contains("DCE") Then
                                    myElement = myElement.Replace(".", "_")
                                    If Instructions(c).Contains("DCS:") Then
                                        myElement = myElement.Replace("_DCS", "")
                                        myValue = HW_DC_STATES.EN.ToString
                                        IsValue = True
                                    ElseIf Instructions(c).Contains("DCE") Then
                                        myElement = myElement.Replace("_DCE", "")
                                        myValue = HW_DC_STATES.DI.ToString
                                        IsValue = True
                                    Else
                                        IsValue = False
                                    End If
                                End If
                            End If

                            'MOTORS
                            If Not IsValue Then

                                If subInstruc.Contains(".MHO") Or _
                                subInstruc.Contains(".MMR") Or _
                                subInstruc.Contains(".MMA") Then
                                    Dim k As Integer = subInstruc.LastIndexOf(CChar("."))
                                    myElement = subInstruc.Substring(0, k).Replace(".", "_")

                                    'fix the inconsistency in names between LAX00 and Firmware
                                    If IsManifold Then
                                        If myElement.Contains("JE1_M1") Then myElement = myElement.Replace("JE1_M1", "JE1_MS")
                                        If myElement.Contains("JE1_M2") Then myElement = myElement.Replace("JE1_M2", "JE1_MR1")
                                        If myElement.Contains("JE1_M3") Then myElement = myElement.Replace("JE1_M3", "JE1_MR2")
                                    End If
                                    If IsFluidics And myElement = "SF1_M1" Then
                                        myElement = "SF1_MS"
                                    End If

                                    If IsPhotometrics And myElement = "GLF_MP" Then
                                        myElement = "GLF_MW"
                                    End If

                                    If Instructions(c).Contains(".MHO") Then
                                        myElement = myElement + "H"
                                        myValue = HW_MOTOR_HOME_STATES.C.ToString
                                        IsValue = True

                                    ElseIf Instructions(c).Contains(".MMR") Then
                                        myElement = myElement + "A"
                                        myValue = subInstruc.Substring(j + 1)
                                        IsValue = True
                                        IsRelative = True
                                    ElseIf Instructions(c).Contains(".MMA") Then
                                        myElement = myElement + "A"
                                        myValue = subInstruc.Substring(j + 1)
                                        IsValue = True
                                    Else
                                        IsValue = False
                                    End If
                                End If
                            End If

                            'STIRRERS
                            If Not IsValue Then
                                If IsFluidics Then
                                    myElement = myElement.Replace(".", "_")
                                    If Instructions(c).Contains("DCS") Then
                                        myValue = "1"
                                        IsValue = True
                                    ElseIf Instructions(c).Contains("DCE") Then
                                        myValue = "0"
                                        IsValue = True
                                    Else
                                        IsValue = False
                                    End If

                                    If myElement = "SF1_AG1" Then
                                        myElement = "SF1_ST1"
                                    End If
                                    If myElement = "SF1_AG2" Then
                                        myElement = "SF1_ST2"
                                    End If

                                End If
                            End If

                            If IsValue Then
                                If IsManifold Then UpdateManifold(myElement, myValue, IsRelative)
                                If IsFluidics Then UpdateFluidics(myElement, myValue, IsRelative)
                                If IsPhotometrics Then UpdatePhotometrics(myElement, myValue.ToString, IsRelative)
                            End If
                        End If
                    End If
                End If
            Next

            If pInstructionSent.Contains("GLF.FO.FAC") Then
                System.Threading.Thread.Sleep(500)
                Me.BsButton12.PerformClick()
            ElseIf pInstructionSent.Contains(".DE.LDE") Then 'level detection
                System.Threading.Thread.Sleep(500)
                MyClass.GenerateCommandAnswerData()
            Else
                If ToSimulateResponse Then
                    System.Threading.Thread.Sleep(100)
                    MyClass.GenerateCommandAnswerData()
                    'SimulatedResponse = "A400;ANSCMD;END:1;"
                    'AutoCommandResponse()
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton4.Click
        'AdjustmentsDataGenerator()
        AdjustmentsGenerator()
    End Sub

    Private Sub BsReceptionButton_Error_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceptionButton_Error.Click
        Try
            'Simulate instruction reception
            If BsTextWrite_End.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions

                        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite_Error.Text.Trim)

                    End If
                End If

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Btn_ANSCMD_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_ANSCMD_OK.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSCMD;END:1;")

                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Btn_ANSCMD_ERR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_ANSCMD_ERR.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSCMD;END:0;")

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton7.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    Dim strOperation As String
                    strOperation = "A400;ANSBLD;BLW:1;P:1;MP:2001;RP:100121;MD:4032;RD:4010;IT:12;DAC:100;P:2;MP:30005;RP:401025;MD:4510;RD:4050;IT:12;DAC:250;P:3;MP:8008;RP:92028;MD:4802;RD:4704;IT:12;DAC:100;P:4;MP:100066;RP:10426;MD:4602;RD:4060;IT:12;DAC:0;P:5;MP:104;RP:900004;MD:4032;RD:4000;IT:12;DAC:200;P:6;MP:1033;RP:80023;MD:4032;RD:4030;IT:12;DAC:15;P:7;MP:17;RP:7327;MD:4702;RD:4030;IT:12;DAC:100;P:8;MP:9202;RP:20022;MD:4002;RD:4000;IT:12;DAC:70;P:11;MP:8209;RP:92019;MD:4812;RD:4714;IT:11;DAC:101;"

                    myGlobal = myAnalyzerManager.SimulateInstructionReception(strOperation)

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub BsButton6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton6.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    Dim strOperation As String
                    strOperation = "A400;ANSBLD;BLW:1;P:1;MP:908001;RP:920120;MD:4132;RD:4012;IT:12;DAC:180;P:8;MP:908202;RP:930020;MD:4232;RD:4020;IT:12;DAC:80;P:6;MP:908033;RP:920022;MD:4332;RD:4033;IT:12;DAC:10;P:5;MP:908004;RP:920021;MD:4432;RD:4060;IT:12;DAC:200;P:2;MP:908005;RP:923020;MD:4512;RD:4070;IT:12;DAC:255;P:4;MP:908066;RP:920420;MD:4622;RD:4062;IT:12;DAC:0;P:7;MP:908707;RP:920320;MD:4742;RD:4032;IT:12;DAC:110;P:3;MP:998008;RP:922020;MD:4862;RD:4794;IT:12;DAC:109;P:11;MP:998009;RP:922022;MD:4860;RD:4792;IT:10;DAC:129;"

                    myGlobal = myAnalyzerManager.SimulateInstructionReception(strOperation)

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub BsButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton5.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSSDM;S:2;AC:6;ST:0;NC:0;NCP:0;DH:12;DM:20;DS:32;T:0;NR:0;CR:0;NE:0;CE:0;")

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub BsButton11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton11.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSSDM;S:2;AC:6;ST:1;NC:5;NCP:1;DH:12;DM:20;DS:32;T:0;NR:2;CR:1;NE:0;CE:0;")

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton10.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSSDM;S:2;AC:6;ST:2;NC:5;NCP:5;DH:12;DM:20;DS:32;T:0;NR:2;CR:2;NE:0;CE:0;")

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton9.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSSDM;S:2;AC:6;ST:3;NC:5;NCP:5;DH:12;DM:20;DS:32;T:0;NR:2;CR:3;NE:1;CE:3;")

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton8.Click
        Try
            'Simulate instruction reception
            If BsTextWrite_End.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions

                        'If AdjustmentsRequested Then
                        '    AdjustmentsGenerator()
                        '    AdjustmentsRequested = False
                        '    Exit Sub
                        'End If

                        'If AbsorbanceRequested Then
                        '    AbsorbanceDataGenerator()
                        '    AbsorbanceRequested = False
                        '    Exit Sub
                        'End If


                        myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;STATUS;S:2;A:1;T:0;C:1;W:0;R:0;E:0;I:1;")

                    End If
                End If

            End If
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub BsButton12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton12.Click
        Try
            MyClass.GenerateCommandAnswerData()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ANSINF_Generator()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            'Simulate instruction reception
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions

                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSERR;N:5;E:111;E:110;E:121;E:120;E:131;")

                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'FLUIDICS
    Private Sub BsButton15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton13.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSJEX_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'MANIFOLD
    Private Sub BsButton14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton14.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSSFX_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ' ANSFCP
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSFCP_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ' ANSFW
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSF_Generator(POLL_IDs.CPU)
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ' ANSBXX
    Private Sub BsButton15_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton15.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSBXX_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private myArmSelected As String
    Private myArmSelected2 As String
    Private Sub BsLabel13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLabel13.Click
        Select Case myArmSelected
            Case "BM1"
                myArmSelected = "BR1"
                myArmSelected2 = "(2)"
            Case "BR1"
                myArmSelected = "BR2"
                myArmSelected2 = "(3)"
            Case "BR2"
                myArmSelected = "AG1"
                myArmSelected2 = "(5)"
            Case "AG1"
                myArmSelected = "AG2"
                myArmSelected2 = "(6)"
            Case "AG2"
                myArmSelected = "BM1"
                myArmSelected2 = "(4)"
        End Select
        Me.BsLabel13.Text = myArmSelected
        Me.BsLabel16.Text = myArmSelected2
    End Sub

    Private Sub BsButton16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton16.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSDXX_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private myProbeSelected As String
    Private myProbeSelected2 As String
    Private Sub BsLabel14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLabel14.Click
        Select Case myProbeSelected
            Case "DR1"
                myProbeSelected = "DR2"
                myProbeSelected2 = "(8)"
            Case "DR2"
                myProbeSelected = "DM1"
                myProbeSelected2 = "(9)"
            Case "DM1"
                myProbeSelected = "DR1"
                myProbeSelected2 = "(7)"
        End Select
        Me.BsLabel14.Text = myProbeSelected
        Me.BsLabel17.Text = myProbeSelected2
    End Sub

    Private Sub BsButton17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton17.Click
        Try
            'Simulate instruction reception
            SimulatedResponse = ANSRXX_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private myRotorSelected As String
    Private myRotorSelected2 As String
    Private Sub BsLabel15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLabel15.Click
        Select Case myRotorSelected
            Case "RR1"
                myRotorSelected = "RM1"
                myRotorSelected2 = "(11)"
            Case "RM1"
                myRotorSelected = "RR1"
                myRotorSelected2 = "(10)"
        End Select
        Me.BsLabel15.Text = myRotorSelected
        Me.BsLabel18.Text = myRotorSelected2
    End Sub

    Private myFwSelected As String
    Private myFwSelected2 As String
    Private myFwSelected3 As String
    Private Sub BsLabel12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLabel12.Click
        Select Case myFwSelected
            Case "CPU"
                myFwSelected = "BM1"
                myFwSelected2 = "BX"
                myFwSelected3 = "(4)"
            Case "BM1"
                myFwSelected = "BR1"
                myFwSelected2 = "BX"
                myFwSelected3 = "(2)"
            Case "BR1"
                myFwSelected = "BR2"
                myFwSelected2 = "BX"
                myFwSelected3 = "(3)"
            Case "BR2"
                myFwSelected = "AG1"
                myFwSelected2 = "BX"
                myFwSelected3 = "(5)"
            Case "AG1"
                myFwSelected = "AG2"
                myFwSelected2 = "BX"
                myFwSelected3 = "(6)"
            Case "AG2"
                myFwSelected = "DR1"
                myFwSelected2 = "DX"
                myFwSelected3 = "(7)"
            Case "DR1"
                myFwSelected = "DR2"
                myFwSelected2 = "DX"
                myFwSelected3 = "(8)"
            Case "DR2"
                myFwSelected = "DM1"
                myFwSelected2 = "DX"
                myFwSelected3 = "(9)"
            Case "DM1"
                myFwSelected = "RR1"
                myFwSelected2 = "RX"
                myFwSelected3 = "(10)"
            Case "RR1"
                myFwSelected = "RM1"
                myFwSelected2 = "RX"
                myFwSelected3 = "(11)"
            Case "RM1"
                myFwSelected = "GLF"
                myFwSelected2 = "GL"
                myFwSelected3 = "(12)"
            Case "GLF"
                myFwSelected = "SF1"
                myFwSelected2 = "SF"
                myFwSelected3 = "(14)"
            Case "SF1"
                myFwSelected = "JE1"
                myFwSelected2 = "JE"
                myFwSelected3 = "(13)"
            Case "JE1"
                myFwSelected = "CPU"
                myFwSelected2 = "CP"
                myFwSelected3 = "(1)"
        End Select
        Me.BsLabel12.Text = myFwSelected
        Me.BsLabel19.Text = myFwSelected3
    End Sub

    Private Sub BsReceivedTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        BsReceivedTextBox.SelectionStart = BsReceivedTextBox.Text.Length
    End Sub

    Private Sub BsReceivedTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsReceivedTextBox.TextChanged
        Try

            BsReceivedTextBox.Select(BsReceivedTextBox.Text.Length, 0)
            BsReceivedTextBox.Refresh()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton18.Click
        Dim myGlobal As New GlobalDataTO
        Try

            ANSISE_Generator("CMD:0;")

            'Acknowledge
            '<ISE!>

            'calibracion
            '<CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 000000D\>
            '<BMV Li 001.3 Na 895.6 K 006.6 Cl 725.6><AMV Li 258.6 Na 688.7 K 115.8 Cl 789.5><CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 A4F000D\>"

            'serum
            '<SER Li 99.88 Na 77.66 K 55.44 Cl 33.22 000000D\>
            '<SMV Li 023.5 Na 089.6 K 115.9 Cl 723.2><AMV Li 645.3 Na 985.2 K 156.3 Cl 568.9><SER Li 99.88 Na 77.66 K 55.44 Cl 33.22 000000D\>

            'urine 1
            '<URN Na 132.6 K 356.2 Cl 988.6 000000D\>
            '<UMV Na 346.8 K 568.2 Cl 794.3><BMV Na 983.4 K 543.7 Cl 682.8><URN Na 132.6 K 356.2 Cl 988.6 000000D\>

            'Read mv
            '<AMV Li 242.9 Na 216.5 K 184.1 Cl 201.2 000000D\>

            'Pump Calib
            '<PMC A 3000 B 2000 W 1000 000000D\>

            'Bubble Calib
            '<BBC A 111 M 222 L 333 000000D\>

            'Checksum
            '<ISV 7BA9>

            'Read Page 0
            '<DSN 37363431313933FFÙ><DDT 00 0000A6F1EA1E060B00640029057802BC01903400490112A3032004E2019A13A5Ù>

            'Read Page 1
            '<DSN 37363431313933FFÙ><DDT 01 000000000000F7FFFFFFFFFFFFFFFFFF00000000E0FFFFFFFFFFFFFFFF02FFFF,>

            'ERRORS
            '<ERC URN S000000C>


            'Dim myPreparationID As Integer = 0
            'Dim myISEResultStr As String = "<ISE!>"

            'Me.BsReceivedTextBox.Clear()

            'Dim myInstruction As New List(Of InstructionParameterTO)
            'Dim myPar1 As InstructionParameterTO = New InstructionParameterTO
            'Dim myPar2 As InstructionParameterTO = New InstructionParameterTO
            'Dim myPar3 As InstructionParameterTO = New InstructionParameterTO
            'Dim myPar4 As InstructionParameterTO = New InstructionParameterTO
            'Dim myPar5 As InstructionParameterTO = New InstructionParameterTO

            'With myPar1
            '    .InstructionType = "ANSISE"
            '    .ParameterIndex = 1
            '    .ParameterValue = "A400"
            'End With
            'myInstruction.Add(myPar1)

            'With myPar2
            '    .InstructionType = "ANSISE"
            '    .ParameterIndex = 2
            '    .ParameterValue = "ANSISE"
            'End With
            'myInstruction.Add(myPar2)

            'With myPar3
            '    .InstructionType = "ANSISE"
            '    .ParameterIndex = 3
            '    .ParameterValue = myPreparationID.ToString
            'End With
            'myInstruction.Add(myPar3)

            'With myPar4
            '    .InstructionType = "ANSISE"
            '    .ParameterIndex = 4
            '    .ParameterValue = myISEResultStr
            'End With
            'myInstruction.Add(myPar4)

            ''Dim myUtil As New Utilities.

            'myGlobal = myAnalyzerManager.ProcessRecivedISEResult(myInstruction)
            'Dim myISEResultData As New ISEResultsDataTO
            'Dim myISEResult As ISEResultTO = myAnalyzerManager.LastISEResult
            'If myISEResult IsNot Nothing Then
            '    Dim str As New System.Text.StringBuilder
            '    With myISEResult
            '        str.Append(.ReceivedResults & vbCrLf & vbCrLf)
            '        If .ConcentrationValues.HasData Then
            '            str.Append("Concentration:" & vbTab & "Li: " & .ConcentrationValues.Li.ToString & vbTab & "Na: " & .ConcentrationValues.Na.ToString & vbTab & "K: " & .ConcentrationValues.K.ToString & vbTab & "Cl: " & .ConcentrationValues.Cl.ToString & vbCrLf)
            '        End If
            '        If .SerumMilivolts.HasData Then
            '            str.Append("Serum (mv):" & vbTab & "Li: " & .SerumMilivolts.Li.ToString & vbTab & "Na: " & .SerumMilivolts.Na.ToString & vbTab & "K: " & .SerumMilivolts.K.ToString & vbTab & "Cl: " & .SerumMilivolts.Cl.ToString & vbCrLf)
            '        End If
            '        If .UrineMilivolts.HasData Then
            '            str.Append("Urine (mv):" & vbTab & "Li: " & .UrineMilivolts.Li.ToString & vbTab & "Na: " & .UrineMilivolts.Na.ToString & vbTab & "K: " & .UrineMilivolts.K.ToString & vbTab & "Cl: " & .UrineMilivolts.Cl.ToString & vbCrLf)
            '        End If
            '        If .CalibratorAMilivolts.HasData Then
            '            str.Append("Calibrator A:" & vbTab & "Li: " & .CalibratorAMilivolts.Li.ToString & vbTab & "Na: " & .CalibratorAMilivolts.Na.ToString & vbTab & "K: " & .CalibratorAMilivolts.K.ToString & vbTab & "Cl: " & .CalibratorAMilivolts.Cl.ToString & vbCrLf)
            '        End If
            '        If .CalibratorBMilivolts.HasData Then
            '            str.Append("Calibrator B:" & vbTab & "Li: " & .CalibratorBMilivolts.Li.ToString & vbTab & "Na: " & .CalibratorBMilivolts.Na.ToString & vbTab & "K: " & .CalibratorBMilivolts.K.ToString & vbTab & "Cl: " & .CalibratorBMilivolts.Cl.ToString & vbCrLf)
            '        End If
            '        If .CalibrationResults.HasData Then
            '            str.Append("Calib. results:" & vbTab & "Li: " & .CalibrationResults.Li.ToString & vbTab & "Na: " & .CalibrationResults.Na.ToString & vbTab & "K: " & .CalibrationResults.K.ToString & vbTab & "Cl: " & .CalibrationResults.Cl.ToString & vbCrLf)
            '        End If
            '        If .PumpsCalibrationValues.HasData Then
            '            str.Append("Pumps calib:" & vbTab & "A: " & .PumpsCalibrationValues.PumpA.ToString & vbTab & "B: " & .PumpsCalibrationValues.PumpB.ToString & vbTab & "W: " & .PumpsCalibrationValues.PumpW.ToString & vbCrLf)
            '        End If
            '        If .BubbleDetCalibrationValues.HasData Then
            '            str.Append("Bubble calib:" & vbTab & "A: " & .BubbleDetCalibrationValues.ValueA.ToString & vbTab & "M: " & .BubbleDetCalibrationValues.ValueM.ToString & vbTab & "L: " & .BubbleDetCalibrationValues.ValueL.ToString & vbCrLf)
            '        End If
            '        If .ChecksumValue.Trim.Length > 0 Then
            '            str.Append("Checksum:" & vbTab & .ChecksumValue & vbCrLf)
            '        End If
            '        If .DallasSNData IsNot Nothing Then
            '            str.Append("Dallas Chip:" & vbTab & .DallasSNData.SNDataString & vbCrLf)
            '            str.Append("Family Code:" & vbTab & .DallasSNData.FamilyCode & vbCrLf)
            '            str.Append("SerialNumber:" & vbTab & .DallasSNData.SerialNumber & vbCrLf)
            '            str.Append("CRC:" & vbTab & .DallasSNData.CRC & vbCrLf)
            '            str.Append(vbCrLf)
            '        End If
            '        If .DallasPage00Data IsNot Nothing Then
            '            str.Append("Dallas Page 00:" & vbTab & vbTab & .DallasPage00Data.Page00DataString & vbCrLf)
            '            str.Append("Lot Number:" & vbTab & vbTab & .DallasPage00Data.LotNumber & vbCrLf)
            '            str.Append("Expiration Date:" & vbTab & .DallasPage00Data.ExpirationYear.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage00Data.ExpirationMonth.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage00Data.ExpirationDay.ToString.PadRight(2, CChar("0")) & vbCrLf)
            '            str.Append("Initial Volume A:" & vbTab & .DallasPage00Data.InitialCalibAVolume.ToString & vbCrLf)
            '            str.Append("Initial Volume B:" & vbTab & .DallasPage00Data.InitialCalibBVolume.ToString & vbCrLf)
            '            str.Append("Distributor Code:" & vbTab & .DallasPage00Data.DistributorCode & vbCrLf)
            '            str.Append("Security Code:" & vbTab & vbTab & .DallasPage00Data.SecurityCode & vbCrLf)
            '            str.Append("CRC:" & vbTab & vbTab & .DallasPage00Data.CRC & vbCrLf)
            '            str.Append(vbCrLf)
            '        End If
            '        If .DallasPage01Data IsNot Nothing Then
            '            str.Append("Dallas Page 01:" & vbTab & vbTab & .DallasPage01Data.Page01DataString & vbCrLf)
            '            str.Append("Consumption A:" & vbTab & .DallasPage01Data.ConsumptionCalA.ToString & vbCrLf)
            '            str.Append("Consumption B:" & vbTab & .DallasPage01Data.ConsumptionCalB.ToString & vbCrLf)
            '            str.Append("Installation Date:" & vbTab & .DallasPage01Data.InstallationYear.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage01Data.InstallationMonth.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage01Data.InstallationDay.ToString.PadRight(2, CChar("0")) & vbCrLf)
            '            str.Append("No good byte:" & vbTab & vbTab & .DallasPage01Data.NoGoodByte & vbCrLf)
            '        End If
            '        If .ResultErrors IsNot Nothing Then
            '            str.Append("ERROR:" & vbTab & vbTab & .ResultErrorsString & vbCrLf)
            '            Dim count As Integer = 1
            '            For Each Err As ISEErrorTO In .ResultErrors
            '                If Not Err.IsCancelError Then
            '                    str.Append("Error " & count.ToString & ":" & vbTab)
            '                End If

            '                If Err.IsCancelError Then
            '                    If Err.ErrorCycle <> ISEErrorTO.ErrorCycles.None Then
            '                        str.Append("Cycle: " & Err.ErrorCycle.ToString & vbTab)
            '                    End If
            '                    str.Append("Because of: " & Err.Affected & vbCrLf)
            '                Else
            '                    str.Append("Affected: " & Err.Affected & vbCrLf)
            '                End If
            '                count += 1
            '            Next
            '        End If
            '        Me.BsReceivedTextBox.Text = str.ToString


            '    End With

            '    myISEResultData.ISEResultsData.Add(myISEResult)

            'End If

            ''SERIALIZE TEST
            'If myISEResultData.ISEResultsData.Count > 0 Then
            '    Dim FS As System.IO.FileStream
            '    FS = System.IO.File.OpenWrite("C:\ISEResults.xml")
            '    Dim serializer As New System.Xml.Serialization.XmlSerializer(myISEResultData.GetType)
            '    serializer.Serialize(FS, myISEResultData)
            '    FS.Close()
            '    FS.Dispose()
            'End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private Sub ANSISE_Generator(ByVal pISEQuery As String)
        Dim myAction As ISECommands
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myActionStr As String = ""
            Dim myCMDIndex As Integer = pISEQuery.IndexOf("CMD:")
            If myCMDIndex >= 0 Then
                Dim myrest As String = pISEQuery.Substring(myCMDIndex)
                If myrest.Length > 0 Then
                    Dim myendIndex As Integer = myrest.IndexOf(";")
                    If myendIndex > 4 Then
                        myActionStr = myrest.Substring(4, myendIndex - 4)
                        If IsNumeric(myActionStr) Then
                            myAction = CType(CInt(myActionStr), ISECommands)

                            Dim myData As String = ""

                            Select Case myAction
                                Case ISECommands.POLL
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.CALB
                                    Threading.Thread.Sleep(3000)
                                    myData &= "P:0;" & "R:<CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 A4F000D\><CAL Li 70.33 Na 99.28 K 86.77 Cl 74.53 A4F000D\>;"
                                Case ISECommands.SAMP
                                    myData &= "P:0;" & "R:<ISE!>;"
                                    ' >> STRT 
                                    ' << myData &= "P:0;" & "R:<SMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>;"
                                Case ISECommands.URINE_ONE
                                    myData &= "P:0;" & "R:<ISE!>;"
                                    ' >> ISECommands.URINE_TWO 
                                    ' << <ISE!>
                                    ' >> STRT
                                    ' << myData &= "P:0;" & "R:<UMV Na xxx.x K xxx.x Cl xxx.x><BMV Na xxx.x K xxx.x Cl xxx.x><URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec>;"
                                Case ISECommands.CLEAN
                                    myData &= "P:0;" & "R:<ISE!>;"
                                    ' >> STRT
                                    ' << myData &= "P:0;" & "R:<ISE!>;"
                                    'Case ISECommands.CLEAN_START    ' ??
                                    '    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.PUMP_CAL
                                    Threading.Thread.Sleep(3000)
                                    myData &= "P:0;" & "R:<PMC A 2999 B 2999 W 1000 000000D\>;"
                                    ' >> STRT
                                    ' << myData &= "P:0;" & "R:<PMC A 3000 B 2000 W 1000 000000D\>;"
                                Case ISECommands.BUBBLE_CAL
                                    myData &= "P:0;" & "R:<BBC A 111 M 222 L 333 000000D>;"
                                Case ISECommands.VERSION_CHECKSUM
                                    myData &= "P:0;" & "R:<ISV 7BA9>;"
                                Case ISECommands.SHOW_BUBBLE_CAL
                                    myData &= "P:0;" & "R:<BBC A 111 M 222 L 333 000000D>;"
                                Case ISECommands.SHOW_PUMP_CAL
                                    myData &= "P:0;" & "R:<PMC A 3000 B 2000 W 1000 000000D\>;"
                                Case ISECommands.LAST_SLOPES
                                    myData &= "P:0;" & "R:<BMV Li 001.3 Na 895.6 K 006.6 Cl 725.6><AMV Li 258.6 Na 688.7 K 115.8 Cl 789.5><CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 A4F000D\>;"
                                Case ISECommands.READ_mV
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<AMV Li 98.9 Na 58.5 K 184.1 Cl 196.2 000000D\>;"
                                Case ISECommands.READ_PAGE_0_DALLAS
                                    Threading.Thread.Sleep(2000)
                                    myData &= "P:0;" & "R:<DSN 37363431313933FFÙ><DDT 00 0000A6F1EA1E060B00640029057802BC01903400490112A3032004E2019A13A5Ù>;"
                                Case ISECommands.READ_PAGE_1_DALLAS
                                    Threading.Thread.Sleep(2000)
                                    myData &= "P:0;" & "R:<DSN 37363431313933FFÙ><DDT 01 000000000000F7FFFFFFFFFFFF0A080BF00000000E0FFFFFFFFFFFFFFFF02FFFF,>;"
                                Case ISECommands.MAINTENANCE
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.DSPA
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.DSPB
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.PURGEA
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.PURGEB
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.PRIME_CALA
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.PRIME_CALB
                                    Threading.Thread.Sleep(1000)
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.DEBUG_mV_ONE
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.DEBUG_mV_TWO
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.DEBUG_mV_OFF
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.MAINTENANCE
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.START
                                    ' Results depending of the previous ISE instruction
                                Case ISECommands.WRITE_DALLAS
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.WRITE_CALA_CONSUMPTION
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.WRITE_CALB_CONSUMPTION
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.WRITE_DAY_INSTALL
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.WRITE_MONTH_INSTALL
                                    myData &= "P:0;" & "R:<ISE!>;"
                                Case ISECommands.WRITE_YEAR_INSTALL
                                    myData &= "P:0;" & "R:<ISE!>;"
                            End Select


                            If myData.Length > 1 Then
                                'myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ANSISE;" & myData)
                            End If

                        End If
                    End If

                End If


            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function ANSCBR_Generator(ByVal myReader As String, ByVal myStatus As String) As String
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myData As String = ""
            myData &= "R:" & myReader & ";"
            myData &= "ST:" & myStatus & ";"
            myData &= "N:1;"
            myData &= "P:87;"
            myData &= "D:1;"
            myData &= "V:1;"

            If myData.Length > 1 Then
                Return "A400;ANSCBR;" & myData
            Else
                Return ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ANSCONFIG_Generator() As String
        Dim myGlobal As New GlobalDataTO
        Try
            Return "A400;STATUS;S:" & CInt(CurrentStatus).ToString & ";A:33;T:0;C:0;W:0;R:0;E:0;I:1;"

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'ANSFWU
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Try
            'Simulate instruction reception
            If myFWUpdateAction = FwUpdateActions.None Then myFWUpdateAction = FwUpdateActions.StartUpdate
            SimulatedResponse = MyClass.ANSFWU_Generator()
            If SimulatedResponse.Length > 0 Then
                myAnalyzerManager.SimulateInstructionReception(SimulatedResponse)
                SimulatedResponse = ""
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub



End Class