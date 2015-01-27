Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Calculations
Imports LIS.Biosystems.Ax00.LISCommunications

Public Class bsReception
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Definitions"
    Private WithEvents myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    Private mdiESWrapperCopy As ESWrapper = CType(AppDomain.CurrentDomain.GetData("GlobalLISManager"), ESWrapper) 'AG 15/03/2013
#End Region

#Region "AG Initial Generate Instructions Testings"




#End Region

#Region "AG Calculations"

    Private Sub BsExecuteCalc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExecuteCalc.Click
        'Try
        Dim myGlobal As New GlobalDataTO
        Dim myExec As Integer = 1
        If IsNumeric(BsExecution.Text) Then
            myExec = CInt(BsExecution.Text.ToString)

            'Execute calculations test
            Dim myCalc As New CalculationsDelegate()
            'myGlobal = myCalc.CalculateExecution(Nothing, myExec, "SN0000099999_Ax400", "2010010501", False, "")
            myGlobal = myCalc.CalculateExecutionNEW(Nothing, UiAx00MainMDI.ActiveAnalyzer, UiAx00MainMDI.ActiveWorkSession, myExec, False, "")
            If myGlobal.HasError Then
                Exit Sub
            End If

            ''Validate execution readings test
            'Dim myRead As New WSReadingsDelegate
            'myGlobal = myRead.ValidateByExecutionID(Nothing, "SN0000099999_Ax400", "2010010501", myExec)
            'Dim myResult As Boolean = False
            'myResult = DirectCast(myGlobal.SetDatos, Boolean)
            'If myResult Then

            'End If

        End If

        'Catch ex As Exception

        'End Try
    End Sub


#End Region

#Region "AG - Several Instruction Generation & Reception Testings"

    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLockProcess.Click
        Dim resultData As New GlobalDataTO
        'Try
        'Lock executions process
        Dim myExecutions As New ExecutionsDelegate
        Const myRotorName As String = "SAMPLES"
        Const myPrepID As Integer = 3
        Const myCellNumber As Integer = 91
        Dim myOKPerformedFlag As Boolean = False
        Dim turnToPendingFlag As Boolean = False

        'Volume missing over the reagent or sample rotors
        resultData = myExecutions.ProcessVolumeMissing(Nothing, myPrepID, myRotorName, myCellNumber, myOKPerformedFlag, UiAx00MainMDI.ActiveWorkSession, UiAx00MainMDI.ActiveAnalyzer, turnToPendingFlag)

        ''SPECIAL CASE ONLY FOR Volume missing over the reactions rotor (dilutions)
        ''These code must be the same as used in method ProcessArmStatusRecived marked as ('Level detection fails (no diluted sample) in reactions rotors)
        'resultData = myExecutions.GetExecutionByPreparationID(Nothing, myPrepID, IAx00MainMDI.ActiveWorkSession, IAx00MainMDI.ActiveAnalyzer)
        'If Not resultData.HasError Then
        '    Dim myExecutionsDS As New ExecutionsDS
        '    myExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)

        '    If myExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
        '        Dim myExecID As Integer = 0
        '        If Not myExecutionsDS.twksWSExecutions(0).IsExecutionIDNull Then myExecID = myExecutionsDS.twksWSExecutions(0).ExecutionID

        '        resultData = myExecutions.UpdateStatusByExecutionID(Nothing, "LOCKED", myExecID, IAx00MainMDI.ActiveWorkSession, IAx00MainMDI.ActiveAnalyzer)
        '    End If
        'End If


        If resultData.HasError Then
            MsgBox(resultData.ErrorMessage)
        End If

        ''Additional info for prep locked
        'resultData = myExecutions.GetDataForPreparationLockedAlarmAdditionalInfo(Nothing, "SN0000099999_Ax400", "2011042701", 8)
        'If resultData.HasError Then
        '    MsgBox(resultData.ErrorMessage)
        'Else
        '    MsgBox(CType(resultData.SetDatos, String))
        'End If

        'Catch ex As Exception

        'End Try
    End Sub


    Private Sub BsSendPrep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSendPrep.Click
        Try
            Dim myglobalto As New GlobalDataTO
            Dim myInstruction As New Instructions
            Dim myInstrucList As New List(Of InstructionParameterTO)
            'obtener los valores para los parametros para las preparaciones
            myglobalto = myInstruction.GeneratePreparation(CType(BsTextBoxToSend.Text.Trim(), Integer)) 'Llamada standard.
            'myglobalto = myInstruction.GenerateISEPreparation(CType(BsTextBoxToSend.Text.Trim(), Integer)) 'Llamada ISE.
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

    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceive.Click
        'Try
        'Simulate instruction reception
        If String.Compare(TextBox5.Text.Trim, "", False) <> 0 Then
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
        'Catch ex As Exception

        'End Try

    End Sub

#End Region

#Region "Communications Board Testings & Simulate Send Next Preparation"

    Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles myAnalyzerManager.ReceptionEvent
        Try
            If pTreated Then
                BsReceivedTextBox.Text += ">> " & pInstructionReceived & vbCrLf
                'Ir a ultima linea
                BsReceivedTextBox.SelectionStart = BsReceivedTextBox.Text.Length
                BsReceivedTextBox.ScrollToCaret()
            End If

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub

    Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles myAnalyzerManager.SendEvent
        Try
            BsReceivedTextBox.Text += pInstructionSent & vbCrLf

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

                'Software testings
                Dim myInstruction As String = BsInstruction.Text.ToUpper.Trim

                'Firmware (setup) testings)
                If Not BsInstructionComboBox.SelectedItem Is Nothing Then
                    myInstruction = BsInstructionComboBox.SelectedItem.ToString
                Else
                    myInstruction = ""
                End If

                Select Case myInstruction
                    Case "CONNECT"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT, True) 'CONNECT

                    Case "STATE"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'STATE

                    Case "STANDBY"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True) 'STANDBY

                    Case "SLEEP"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True) 'SLEEP

                    Case "RUNNING"
                        myAnalyzerManager.ActiveAnalyzer = UiAx00MainMDI.ActiveAnalyzer
                        myAnalyzerManager.ActiveWorkSession = UiAx00MainMDI.ActiveWorkSession

                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True) 'RUNNING

                    Case "START"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.START, True) 'START

                    Case "TEST"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION, True) 'START

                    Case "ENDRUN"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True) 'ENDRUN

                    Case "INFO OFF"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)

                    Case "INFO ON"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                    Case "CONFIG"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True) 'CONFIG

                    Case "POLLRD"
                        myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLRD, True, Nothing, 3) 'POLLRD

                        'Case "ABORT"
                        '    myGlobal = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True) 'ENDRUN

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

                If String.Compare(myTitle, "", False) <> 0 Then
                    Me.ShowMessage(myTitle, myGlobal.ErrorCode)
                    'Else
                    'BsReceivedTextBox.Text += myAnalyzerManager.InstructionSent & vbCrLf
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsCommTestings_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsCommTestings_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            Me.Cursor = Cursors.Default

        End Try

    End Sub

    Private Sub BsClearReception_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsClearReception.Click
        Try
            BsReceivedTextBox.Clear()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsClearReception_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsClearReception_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub


    Private Sub BsSendNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSendNext.Click
        Try
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                Dim ResultData As New GlobalDataTO
                ResultData = myAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION, True, Nothing, 1, "")

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsSendNext_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsSendNext_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub
#End Region

    Private Sub BsButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsClear.Click
        TextBox5.Clear()
    End Sub




    Private Sub BsButton2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton2.Click
        'Try
        Dim myGlobal As New GlobalDataTO
        Dim myExec As Integer = 1

        If IsNumeric(BsExecution.Text) Then
            myExec = CInt(BsExecution.Text.ToString)

            'Execute calculations test
            Dim myCalc As New CalculationsDelegate()
            'myGlobal = myCalc.CalculateExecution(Nothing, myExec, "SN0000099999_Ax400", "2010010501", False, "")
            'myGlobal = myCalc.CalculateExecution(Nothing, myExec, IAx00MainMDI.ActiveAnalyzer, IAx00MainMDI.ActiveWorkSession, False, "")

            myGlobal = myCalc.CalculateExecutionNEW(Nothing, UiAx00MainMDI.ActiveAnalyzer, UiAx00MainMDI.ActiveWorkSession, myExec, False, "", False, Nothing)
            If myGlobal.HasError Then
                Exit Sub
            End If

            ''Validate execution readings test
            'Dim myRead As New WSReadingsDelegate
            'myGlobal = myRead.ValidateByExecutionID(Nothing, "SN0000099999_Ax400", "2010010501", myExec)
            'Dim myResult As Boolean = False
            'myResult = DirectCast(myGlobal.SetDatos, Boolean)
            'If myResult Then

            'End If

        End If

        'Catch ex As Exception

        'End Try
    End Sub

    Private Sub bsDecodeEnBase2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDecodeEnBase2.Click
        Try
            'Decodificar en base 2
            'Const errorValue As Integer = 5 'En base2 el 5 (0x101) está formado por 1 + 4 (0x001 or 0x100)
            'Dim errorValue As Integer = 11 'En base2 el 11 (0x1011) está formado por 1 + 2 + 8 (0x001 or 0x010 or 0x1000)

            'If (errorValue And 1) = 1 Then MessageBox.Show("Contiene el 1")
            'If (errorValue And 2) = 2 Then MessageBox.Show("Contiene el 2")
            'If (errorValue And 4) = 4 Then MessageBox.Show("Contiene el 4")
            'If (errorValue And 8) = 8 Then MessageBox.Show("Contiene el 8")

            'Pruebas BA-2006
            Dim myDlgate As New OrderCalculatedTestsDelegate
            Dim resultData As New GlobalDataTO
            resultData = myDlgate.GetOrderTestsToExcludeInPatientsReport(Nothing)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub BsHistoricCalibCurve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsHistoricCalibCurve.Click
        Try
            'Datos para RSAT 'Curva1 OK -En ResActuales 18-10-2012.SAT'
            'Dim analyzerID As String = "834000114"
            'Dim worksessionID As String = "2012101901"
            'Dim histOrderTestID As Integer = 6
            'Dim testName As String = "ACID GLPR-COPY"
            'Dim sampleType As String = "SER"

            'Datos para RSAT 'Curva2 OK -En ResActuales 18-10-2012.SAT'
            Dim analyzerID As String = "834000114"
            Dim worksessionID As String = "2012101901" '"2012101901", "2012101501" 
            Dim histOrderTestID As Integer = 20 '6, 15, 20
            Dim testName As String = "IGG" '"ACID GLPR-COPY", "IGG"
            Dim sampleType As String = "SER"


            Dim resultData As New GlobalDataTO
            'Get Avg Results
            Dim dlgate As New HisWSResultsDelegate
            Dim avgResults As New ResultsDS
            resultData = dlgate.GetAvgResultsForCalibCurve(Nothing, histOrderTestID, analyzerID, worksessionID)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                avgResults = CType(resultData.SetDatos, ResultsDS)
            End If

            'Get Replic Results
            Dim dlgate2 As New HisWSExecutionsDelegate
            Dim exeResults As New ExecutionsDS
            resultData = dlgate2.GetExecutionResultsForCalibCurve(Nothing, histOrderTestID, analyzerID, worksessionID)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                exeResults = CType(resultData.SetDatos, ExecutionsDS)
            End If

            ''Get Curve Points
            'Dim curve As New HisWSCurveResultsDelegate
            'resultData = curve.GetResults(Nothing, 6, "834000114", "2012101701")

            'Open the calib curve screen
            'Inform the properties
            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
             (From row In exeResults.vwksWSExecutionsResults _
              Where row.TestName = testName AndAlso row.SampleClass = "CALIB" AndAlso row.SampleType = sampleType _
              Select row).ToList()
            If TestList.Count > 0 Then
                'RH 19/10/2010 Introduce the Using statement
                Using myCurveForm As New UiResultsCalibCurve
                    With myCurveForm
                        .ActiveAnalyzer = analyzerID
                        .ActiveWorkSession = worksessionID
                        .AnalyzerModel = "A400"
                        .AverageResults = avgResults
                        .ExecutionResults = exeResults
                        .SelectedTestName = TestList(0).TestName
                        .SelectedSampleType = TestList(0).SampleType
                        .SelectedFullTestName = TestList(0).TestName
                        .SelectedLot = "5684DEF"
                        .SelectedCalibrator = "CALIB AGP"

                        .AcceptedRerunNumber = (From row In avgResults.vwksResults _
                                                Where row.OrderTestID = TestList(0).OrderTestID _
                                                AndAlso row.AcceptedResultFlag = True _
                                                Select row.RerunNumber).First
                        .HistoricalMode = True
                    End With
                    myCurveForm.ShowDialog()
                End Using
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub bsNewGUID_Click(sender As Object, e As EventArgs) Handles bsNewGUID.Click
        'Dim myUtils As New Utilities
        Dim myGlobal As New GlobalDataTO
        myGlobal = Utilities.GetNewGUID
        If Not myGlobal.HasError Then
            MessageBox.Show(CType(myGlobal.SetDatos, String))
        Else
            MessageBox.Show(myGlobal.ErrorMessage)
        End If

    End Sub



    Private Sub bsCustomSelection_Click(sender As Object, e As EventArgs) Handles bsCustomSelection.Click

        Try
            Dim resultData As New GlobalDataTO

            'Shown the Positioning Warnings Screen
            Using AuxMe As New UiSortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "STD"
                AuxMe.ShowDialog()
            End Using


        Catch ex As Exception

        End Try
    End Sub
End Class
