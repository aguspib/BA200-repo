
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class TestSimulatorAX00

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
   

#Region "Definitions"
    Private WithEvents myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)


#End Region



#Region "Communications Board Testings"

    Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles myAnalyzerManager.ReceptionEvent
        Try
            If Not pTreated Then
                If BsReceivedTextBox.Text.Length > 5000 Then BsReceivedTextBox.Clear()
                BsReceivedTextBox.Text += ">> " & pInstructionReceived & vbCrLf
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub

  

    'Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles myAnalyzerManager.SendEvent



    '    Try

    '        If BsReceivedTextBox.Text.Length > 500 Then
    '            BsReceivedTextBox.Clear()
    '        End If

    '        BsReceivedTextBox.Text += "<< " & pInstructionSent & vbCrLf

    '        SimulationTest(pInstructionSent)


    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
    '    End Try
    'End Sub

   
    'Private Sub SimulationTest(ByVal pInstructionSent As String)

    '    Dim myGlobal As New GlobalDataTO

    '    Try

    '        Dim mySensorsList As New List(Of SENSOR)

    '        If pInstructionSent.Contains("READSENSOR") Then

    '            If pInstructionSent.Contains("WSL") Then
    '                mySensorsList.Add(SENSOR.WASHING_SOLUTION_LEVEL)
    '            End If
    '            If pInstructionSent.Contains("HCL") Then
    '                mySensorsList.Add(SENSOR.HIGH_CONTAMINATION_LEVEL)
    '            End If
    '            If pInstructionSent.Contains("DWL1") Then
    '                mySensorsList.Add(SENSOR.DISTILLED_WATER_EMPTY)
    '            End If
    '            If pInstructionSent.Contains("DWL2") Then
    '                mySensorsList.Add(SENSOR.DISTILLED_WATER_FULL)
    '            End If
    '            If pInstructionSent.Contains("LCL1") Then
    '                mySensorsList.Add(SENSOR.LOW_CONTAMINATION_EMPTY)
    '            End If
    '            If pInstructionSent.Contains("LCL2") Then
    '                mySensorsList.Add(SENSOR.LOW_CONTAMINATION_FULL)
    '            End If

    '            ''FOR TESTING
    '            'If pInstructionSent.Contains("XTEMP1") Then
    '            '    mySensorsList.Add(SENSOR.TEST_TEMP1)
    '            'End If

    '            'If pInstructionSent.Contains("XTEMP2") Then
    '            '    mySensorsList.Add(SENSOR.TEST_TEMP2)
    '            'End If

    '            'If pInstructionSent.Contains("XDOOR") Then
    '            '    mySensorsList.Add(SENSOR.TEST_DOOR_OPEN)
    '            'End If

    '            'If pInstructionSent.Contains("XPUMP1") Then
    '            '    mySensorsList.Add(SENSOR.TEST_PUMP1)
    '            'End If

    '            'If pInstructionSent.Contains("XBATTERY") Then
    '            '    mySensorsList.Add(SENSOR.TEST_BATTERY)
    '            'End If

    '        End If

    '        If pInstructionSent.Contains("TANKTESTEMPTYLC") Then
    '            TanksTestON = False
    '            TanksTestFase = 1
    '            TanksTestStepTime = 0
    '        End If

    '        If pInstructionSent.Contains("TANKTESTFILLDW") Then
    '            TanksTestON = False
    '            TanksTestFase = 3
    '            TanksTestStepTime = 0
    '        End If

    '        If pInstructionSent.Contains("TANKTESTTRANSFER") Then
    '            TanksTestON = False
    '            TanksTestFase = 5
    '            TanksTestStepTime = 0
    '        End If

    '        If pInstructionSent.Contains("TANKSTESTEXIT") Then
    '            TanksTestON = False
    '            TanksTestFase = 0
    '            TanksTestStepTime = 0
    '        End If

    '        SensorDataGenerator(mySensorsList)

    '        If pInstructionSent.Contains("ABSORBANCE") Then
    '            AbsorbanceDataGenerator()
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub
    


#End Region

    

    'FOR SIMULATING
    Private myLastWSValue As Double = 512
    Private myLastHCValue As Double = 512

    Private myLastTEMP1Value As Double = 0
    Private myLastTEMP2Value As Double = 0

    Private TanksTestON As Boolean = False
    Private TanksTestFase As Integer = -1
    Private TanksTestStepTime As Integer = 0
    'Private Sub SensorDataGenerator(ByVal pSensors As List(Of SENSOR))
    '    Try

    '        'System.Threading.Thread.Sleep(500) 'QUITAR

    '        If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '            Dim myGlobal As New GlobalDataTO


    '            If myAnalyzerManager.CommThreadsStarted Then
    '                'build simulated monitor data values
    '                Dim mySensors As String = ""
    '                Dim myValues As String = ""

    '                Dim myTrend As Integer = 1
    '                If Now.Second <= 30 Then
    '                    myTrend = 1
    '                Else
    '                    myTrend = -1
    '                End If

    '                'BALANCE TANKS
    '                '********************************************************************************

    '                Dim myValue As Double

    '                If pSensors IsNot Nothing Then
    '                    If pSensors.Contains(SENSOR.WASHING_SOLUTION_LEVEL) Then

    '                        myValue = myLastWSValue + myTrend * 50.4 * (Rnd(1))
    '                        If myValue > 1024 Then myValue = 1024
    '                        If myValue < 0 Then myValue = 0
    '                        myLastWSValue = myValue
    '                        mySensors &= "WSL" & "|"
    '                        myValues &= myValue.ToString("0") & "|"

    '                    End If

    '                    If pSensors.Contains(SENSOR.HIGH_CONTAMINATION_LEVEL) Then

    '                        myValue = myLastHCValue - myTrend * 40.3 * (Rnd(1))
    '                        If myValue > 1024 Then myValue = 1024
    '                        If myValue < 0 Then myValue = 0
    '                        myLastHCValue = myValue
    '                        mySensors &= "HCL" & "|"
    '                        myValues &= myValue.ToString("0") & "|"

    '                    End If



    '                    'INTERMEDIATE TANKS
    '                    '************************************************************************************
    '                    If TanksTestON And TanksTestFase > 0 Then
    '                        If pSensors.Contains(SENSOR.DISTILLED_WATER_EMPTY) And _
    '                        pSensors.Contains(SENSOR.DISTILLED_WATER_FULL) And _
    '                        pSensors.Contains(SENSOR.LOW_CONTAMINATION_EMPTY) And _
    '                        pSensors.Contains(SENSOR.LOW_CONTAMINATION_FULL) Then

    '                            mySensors &= "DWL2" & "|" 'TOP
    '                            mySensors &= "DWL1" & "|" 'BOTTOM

    '                            mySensors &= "LCL2" & "|" 'TOP
    '                            mySensors &= "LCL1" & "|" 'BOTTOM

    '                            TanksTestStepTime += 1

    '                            If TanksTestStepTime = 5 Then
    '                                TanksTestFase += 1
    '                            End If

    '                            If TanksTestStepTime >= 10 Then

    '                                'Dim myAnswer As String = ""
    '                                'Select Case TanksTestFase
    '                                '    'PONER EN GLOBALENUMERATES pLayerInstrucionReception
    '                                '    Case 2 : myAnswer = "A400;TANKTESTEMPTYLC_OK;S:2;AC:6;T:0;C:6;W:0;R:0;E:0;"
    '                                '    Case 4 : myAnswer = "A400;TANKTESTFILLDW_OK;S:2;AC:6;T:0;C:6;W:0;R:0;E:0;"
    '                                '    Case 6 : myAnswer = "A400;TANKTESTTRANSFER_OK;S:2;AC:6;T:0;C:6;W:0;R:0;E:0;"
    '                                'End Select
    '                                'If myAnswer.Length > 0 Then
    '                                '    myGlobal = myAnalyzerManager.SimulateInstructionReception(myAnswer)
    '                                '    TanksTestFase = 0
    '                                '    TanksTestON = False
    '                                '    Exit Sub
    '                                'End If

    '                                TanksTestFase = 0
    '                            End If

    '                            'The TOP Detector is ON (0) when the Level is over the detector
    '                            'The BOTTOM Detector is ON (1) when the Level is under the detector

    '                            Select Case TanksTestFase
    '                                Case 1 'DW BOTTOM, LC TOP
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                    myValues &= "0" & "|" 'TOP on 
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                Case 2 'DW BOTTOM, LC MIDDLE
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                Case 3 'DW BOTTOM, LC BOTTOM
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                Case 4 'DW MIDDLE, LC BOTTOM
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                Case 5 'DW TOP, LC BOTTOM
    '                                    myValues &= "0" & "|" 'TOP on
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                Case 6 'DW MIDDLE, LC MIDDLE
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                                Case 7 'DW BOTTOM, LC TOP
    '                                    myValues &= "1" & "|" 'TOP off
    '                                    myValues &= "1" & "|" 'BOTTOM on

    '                                    myValues &= "0" & "|" 'TOP on
    '                                    myValues &= "0" & "|" 'BOTTOM off

    '                            End Select

    '                        End If
    '                    End If



    '                    '****************TESTING******************************************
    '                    If pSensors.Contains(SENSOR.REACTIONS_ROTOR_TEMP) Then
    '                        myValue = myLastTEMP1Value + myTrend * 11.4 * (Rnd(3))
    '                        If myValue > 70 Then myValue = 70
    '                        If myValue < -50 Then myValue = -50
    '                        myLastTEMP1Value = myValue
    '                        mySensors &= "XTEMP1" & "|"
    '                        myValues &= myValue.ToString("0.0") & "|"
    '                    End If

    '                    If pSensors.Contains(SENSOR.REAGENTS_ROTOR1_TEMP) Then
    '                        myValue = myLastTEMP2Value - myTrend * 9.5 * (Rnd(2))
    '                        If myValue > 70 Then myValue = 70
    '                        If myValue < -50 Then myValue = -50
    '                        myLastTEMP2Value = myValue
    '                        mySensors &= "XTEMP2" & "|"
    '                        myValues &= myValue.ToString("0.0") & "|"
    '                    End If

    '                    If pSensors.Contains(SENSOR.SAMPLES_COVER) Then
    '                        myValue = Now.Second Mod 4
    '                        mySensors &= "XPUMP1" & "|"
    '                        myValues &= myValue.ToString("0") & "|"
    '                    End If

    '                    If pSensors.Contains(SENSOR.REACTIONS_COVER) Then
    '                        myValue = Now.Second Mod 4
    '                        mySensors &= "XDOOR" & "|"
    '                        myValues &= myValue.ToString("0") & "|"
    '                    End If

    '                    If pSensors.Contains(SENSOR.WASHING_SOLUTION_LEVEL) Then
    '                        myValue = 100 * Now.Second / 60
    '                        mySensors &= "XBATTERY" & "|"
    '                        myValues &= myValue.ToString("0") & "|"
    '                    End If
    '                End If



    '                'send

    '                If mySensors.Length > 1 Then
    '                    Application.DoEvents()
    '                    mySensors = mySensors.Substring(0, mySensors.Length - 1)
    '                    myValues = myValues.Substring(0, myValues.Length - 1).Replace(",", ".")

    '                    myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;SENSORS_RECEIVED;SENSORS:" & mySensors & ";VALUES:" & myValues & ";S:2;AC:6;T:0;C:6;W:0;R:0;E:0;")
    '                End If

    '            End If
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Private Sub AbsorbanceDataGenerator()

    '    Try
    '        Dim myAbs As String = ""
    '        Dim AbsorbanceData As New List(Of Double)

    '        For a As Integer = 1 To 200 Step 1
    '            If a > 0 And a <= 37 Then
    '                AbsorbanceData.Add(9)
    '            ElseIf a > 37 And a < 42 Then
    '                Select Case a
    '                    Case 38 : AbsorbanceData.Add(8.5)
    '                    Case 39 : AbsorbanceData.Add(6.5)
    '                    Case 40 : AbsorbanceData.Add(4.5)
    '                    Case 41 : AbsorbanceData.Add(3.5)
    '                End Select

    '            ElseIf a = 42 Then
    '                AbsorbanceData.Add(3)

    '            ElseIf a > 42 And a < 47 Then
    '                Select Case a
    '                    Case 43 : AbsorbanceData.Add(3.5)
    '                    Case 44 : AbsorbanceData.Add(4.4)
    '                    Case 45 : AbsorbanceData.Add(5.3)
    '                    Case 46 : AbsorbanceData.Add(6.2)
    '                End Select

    '            ElseIf a >= 47 And a <= 77 Then
    '                AbsorbanceData.Add(7)

    '            ElseIf a > 77 And a < 87 Then
    '                Select Case a
    '                    Case 78 : AbsorbanceData.Add(6.2)
    '                    Case 79 : AbsorbanceData.Add(5.3)
    '                    Case 80 : AbsorbanceData.Add(4.4)
    '                    Case 81 : AbsorbanceData.Add(3.5)
    '                    Case 82 : AbsorbanceData.Add(3)
    '                    Case 83 : AbsorbanceData.Add(3.5)
    '                    Case 84 : AbsorbanceData.Add(4.4)
    '                    Case 85 : AbsorbanceData.Add(5.3)
    '                    Case 86 : AbsorbanceData.Add(6.2)
    '                End Select

    '            ElseIf a >= 87 And a <= 117 Then
    '                AbsorbanceData.Add(7)

    '            ElseIf a > 117 And a < 127 Then
    '                Select Case a
    '                    Case 118 : AbsorbanceData.Add(6.2)
    '                    Case 119 : AbsorbanceData.Add(5.3)
    '                    Case 120 : AbsorbanceData.Add(4.4)
    '                    Case 121 : AbsorbanceData.Add(3.5)
    '                    Case 122 : AbsorbanceData.Add(3)
    '                    Case 123 : AbsorbanceData.Add(3.5)
    '                    Case 124 : AbsorbanceData.Add(4.4)
    '                    Case 125 : AbsorbanceData.Add(5.3)
    '                    Case 126 : AbsorbanceData.Add(6.2)
    '                End Select

    '            ElseIf a >= 127 And a <= 157 Then
    '                AbsorbanceData.Add(7)

    '            ElseIf a > 157 And a < 167 Then
    '                Select Case a
    '                    Case 158 : AbsorbanceData.Add(3.5)
    '                    Case 159 : AbsorbanceData.Add(4.5)
    '                    Case 160 : AbsorbanceData.Add(6.5)
    '                    Case 161 : AbsorbanceData.Add(8.5)
    '                End Select

    '            ElseIf a >= 162 And a <= 200 Then
    '                AbsorbanceData.Add(9)

    '            End If

    '        Next

    '        myAbs = ""
    '        For Each A As Double In AbsorbanceData
    '            myAbs &= A.ToString("0.0").Replace(",", ".") & "|"
    '        Next

    '        myAbs = myAbs.Substring(0, myAbs.Length - 1)

    '        If myAbs.Length > 1 Then
    '            Dim myGlobal As New GlobalDataTO
    '            myGlobal = myAnalyzerManager.SimulateInstructionReception("A400;ABSORBANCE_RECEIVED;ABSORBANCE:" & myAbs & ";S:2;AC:6;T:0;C:6;W:0;R:0;E:0;")
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try


    'End Sub

    Private Sub AnswerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnswerButton.Click
        Try
            'Simulate instruction reception

            If BsTextWrite_Endw.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Me.Enabled = False

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions
                        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite_Endw.Text.Trim)

                        'tanks test
                        Select Case TanksTestFase
                            Case 1, 3, 5
                                TanksTestON = True


                        End Select
                    End If

                    Me.Enabled = True
                End If

            End If


        Catch ex As Exception
            Throw ex
        Finally
            Me.Enabled = True
        End Try
    End Sub

    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    Private Sub TestSimulatorAX00_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated

    End Sub

    Private Sub BsReceivedTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceivedTextBox.TextChanged

    End Sub

    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    '    Try
    '        Dim myGlobal As New GlobalDataTO

    '        myGlobal = myAnalyzerManager.SimulateInstructionReception(BsTextWrite_Endw.Text.Trim)

    '        Application.DoEvents()

    '        System.Threading.Thread.Sleep(1000)

    '        Dim mySensorsAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    '        If mySensorsAnalyzerManager.CommThreadsStarted Then
    '            myGlobal = mySensorsAnalyzerManager.SimulateInstructionReception("A400;SENSORS_RECEIVED;SENSORS:" & "WSL" & ";VALUES:" & "357" & ";S:2;AC:6;T:0;C:6;W:0;R:0;E:0;")
    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub
End Class