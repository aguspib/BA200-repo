Option Strict On
Option Explicit On

Imports System.Management

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.Windows.Forms
Imports Biosystems.Ax00.BL
Imports System.IO

Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.Global.TO

Imports Biosystems.Ax00.InfoAnalyzer

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities

Public Class FormSergio
    Inherits BSAdjustmentBaseForm

    'Private myAnalyzerManager As New AnalyzerManager(My.Application.Info.AssemblyName, "A400")
    Public Sub New(ByRef pMDI As Ax00ServiceMainMDI)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myServiceMDI = pMDI
        myMDI = pMDI
        'myAnalyzerManager = pMDI.MDIAnalyzerManager
    End Sub


    Private myMDI As Ax00ServiceMainMDI

    Private Sub FormSergio_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            'Dim myUtil As New Utilities.
            TaskBarState = InitialTaskBarState
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub



    Private Sub FormSergio_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myProdName As String = My.Application.Info.ProductName

            'Dim myUtil As New Utilities.
            InitialTaskBarState = TaskBarState

            RefreshTaskbarState()


            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = GetIconName("ADJUSTMENT")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myGlobal = ResizeImage(myImage, New Size(16, 16))
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myImage = CType(myGlobal.SetDatos, Image)
                        BsButton14.Image = myImage
                        BsButton14.ImageAlign = ContentAlignment.MiddleLeft
                    End If
                End If

            End If

            Me.BsDataGridView1.Rows.Add()
            Me.BsDataGridView1.Rows.Add()
            Me.BsDataGridView1.Rows.Add()
            Me.BsDataGridView1.Rows.Add()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub



    Private Sub BsExceptionsButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsExceptionsButton.Click
        Try
            Throw New Exception
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BsCloseButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsCloseButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BsButton1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton1.Click
        ChangeScripts()
    End Sub


    Private WithEvents myScreenDelegate As FwScriptsEditionDelegate



    Private Function ChangeScripts() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO


        Try

            Dim myText As String = ""



            Dim myData As String = ""
            Dim objReader As StreamReader
            Dim path As String = "C:\FWScripts1.xml"
            objReader = New StreamReader(path)
            myData = objReader.ReadToEnd()
            objReader.Close()

            myText = myData

            Dim myLines() As String = myText.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)

            Dim ScriptsText As New List(Of List(Of String))
            Dim InitialText As New List(Of String)
            Dim FinalText As New List(Of String)
            Dim ScriptNum As Integer = 0
            Dim scriptText As New List(Of String)

            Dim isScriptsArea As Boolean = False

            For L As Integer = 0 To myLines.Length - 1

                If myLines(L).Trim = "<FwScriptTO>" Then
                    isScriptsArea = True
                    scriptText.Add(myLines(L))
                ElseIf myLines(L).Trim = "</FwScriptTO>" Then
                    If scriptText.Count > 0 Then
                        scriptText.Add(myLines(L))
                        ScriptsText.Add(scriptText)
                    End If
                    scriptText = New List(Of String)
                ElseIf myLines(L).Trim = "</FwScripts>" Then
                    FinalText.Add(myLines(L))
                Else
                    If Not isScriptsArea Then
                        InitialText.Add(myLines(L))
                    Else
                        If scriptText.Count > 0 Then
                            scriptText.Add(myLines(L))
                        ElseIf FinalText.Count > 0 Then
                            FinalText.Add(myLines(L))
                        End If
                    End If

                End If
            Next


            Dim myNewScripts As New List(Of List(Of String))

            For Each S As List(Of String) In ScriptsText
                Dim myID As Integer = 0
                Dim myTime As Integer = 0
                Dim newInstruction As New List(Of String)
                For Each I As String In S
                    If I.Trim.Contains("<InstructionID>") Then
                        Dim index1 As Integer = I.IndexOf(">")
                        Dim index2 As Integer = I.Substring(index1 + 1).IndexOf("<")
                        myID = CInt(I.Substring(index1 + 1, index2)) + 1
                    End If
                    If I.Trim.Contains("<Timer>") Then
                        Dim index1 As Integer = I.IndexOf(">")
                        Dim index2 As Integer = I.Substring(index1 + 1).IndexOf("<")
                        myTime = CInt(I.Substring(index1 + 1, index2)) + 1000
                    End If
                Next
                newInstruction.Add(vbTab & vbTab & vbTab & "<InstructionTO>")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<InstructionID>" & myID.ToString & "</InstructionID>")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<Sequence>" & myID.ToString & "</Sequence>")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<Timer>" & myTime.ToString & "</Timer>")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<Code>END</Code>")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<Params />")
                newInstruction.Add(vbTab & vbTab & vbTab & vbTab & "<EnableEdition>false</EnableEdition>")
                newInstruction.Add(vbTab & vbTab & vbTab & "    </InstructionTO>")

                Dim ind As Integer = 0
                Dim myNewScript As New List(Of String)
                For Each I As String In S
                    If Not I.Trim.Contains("</Instructions>") Then
                        myNewScript.Add(I)
                    Else
                        ind = S.IndexOf(I) - 1
                        Exit For
                    End If
                Next

                For Each I As String In newInstruction
                    myNewScript.Add(I)
                Next
                For Each I As String In S
                    If S.IndexOf(I) > ind Then
                        myNewScript.Add(I)
                    End If
                Next

                myNewScripts.Add(myNewScript)

            Next


            Dim ResultText As String = ""
            For Each S As String In InitialText
                ResultText &= S & vbCrLf
            Next
            For Each S As List(Of String) In myNewScripts
                For Each I As String In S
                    ResultText &= I & vbCrLf
                Next
            Next
            For Each S As String In FinalText
                ResultText &= S & vbCrLf
            Next


            Dim myStreamWriter As StreamWriter = File.CreateText("C:\FWScripts2.xml")
            myStreamWriter.Write(ResultText)
            myStreamWriter.Close()



            myGlobal.SetDatos = ResultText




        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ImportAdjustmentsRestoreFile ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ImportAdjustmentsRestoreFile", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    Private Sub BsButton2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton2.Click
        Try
            Dim a As String = Environment.GetFolderPath(Environment.SpecialFolder.System)

            'XP
            'C:\Windows\System32\config\AppEvent.Evt
            'SysEvent.Evt

            'Win 7
            'C:\Windows\System\WinEvt\...

            Log1()

            Exit Sub

            'Dim class1 As ManagementClass = New ManagementClass("WINMGMTS:\\.\ROOT\cimv2")

            'For Each ob As ManagementObject In class1.GetInstances
            '
            'If ob.GetPropertyValue("FileSize") IsNot Nothing Then
            '
            'Dim o As Object = CType(ob.GetPropertyValue("FileSize"), System.Int64)
            '
            'End If
            '
            'Next

            'myResponse.SetDatos = myServices

            ''http://wutils.com/wmi/
            'Dim wmiClass
            'wmiClass = GetObject("WINMGMTS:\\.\ROOT\cimv2:" + "CIM_LogicalFile")
            'Wscript.Echo(wmiClass.AccessMask.Origin) 'or other property name
            'InstancesOf()

            'Dim oWMI, Instances, Instance

            ''Get base WMI object, "." means computer name (local)
            'oWMI = GetObject("WINMGMTS:\\.\ROOT\cimv2")

            ''Get instances of CIM_LogicalFile 
            'Instances = oWMI.InstancesOf("CIM_LogicalFile")


            ''Enumerate instances  
            'For Each Instance In Instances
            '    'Do something with the instance
            '    Wscript.Echo(Instance.AccessMask) 'or other property name
            'Next 'Instance

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function QueryLog() As Collection

        'Set up the new collection
        QueryLog = New Collection

        'Set the WMI scope options 
        Dim oWMI_Scope As New ManagementScope
        oWMI_Scope.Path.Server = "." 'local
        oWMI_Scope.Path.Path = "\\.\root\CIMV2"
        oWMI_Scope.Path.NamespacePath = "root\CIMV2"

        ' Set impersonation level
        oWMI_Scope.Options.Authentication = AuthenticationLevel.Default
        oWMI_Scope.Options.Impersonation = ImpersonationLevel.Impersonate
        oWMI_Scope.Options.EnablePrivileges = True

        'Define the WMI query 
        Dim oWMI_Query As New ObjectQuery

        oWMI_Query.QueryString = "SELECT * FROM Win32_NTLogEvent"

        'Create the WMI search engine 
        Dim oWMI_Results As New ManagementObjectSearcher(oWMI_Scope, oWMI_Query)

        ' Iterate through the resulting collection 
        'Dim oWMI_Object As Object
        For Each oWMI_Object As ManagementObject In oWMI_Results.Get()
            Dim pp As String = ""
            Dim n As PropertyDataCollection = oWMI_Object.Properties
            For Each P As PropertyData In n
                pp &= P.Name & vbCrLf
            Next
            Dim o As Object = CType(oWMI_Object.GetPropertyValue("RecordNumber"), Int64)
        Next oWMI_Object

        ' Clean up
        'oWMI_Object = Nothing
        oWMI_Scope = Nothing
        oWMI_Query = Nothing
        oWMI_Results = Nothing

    End Function

    Private Sub Log1()
        Try

            Dim classInstance As New ManagementObject( _
                "root\CIMV2", _
                "Win32_NTEventlogFile.Name='C:\WINDOWS\system32\config\AppEvent.Evt'", Nothing)

            Dim inParams As ManagementBaseObject = classInstance.GetMethodParameters("Copy")

            inParams("FileName") = "c:\sample.txt"

            Dim outParams As ManagementBaseObject = _
                classInstance.InvokeMethod("Copy", inParams, Nothing)

            Console.WriteLine("Out parameters:")
            Console.WriteLine("ReturnValue: {0}", outParams("ReturnValue"))

        Catch err As ManagementException
            MessageBox.Show("An error occurred while trying to execute the WMI method: " & err.Message)
        End Try
    End Sub

    Private Sub BsButton3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton3.Click
        Try

            Dim myISECommand As New ISECommandTO()
            With myISECommand
                .ISEMode = ISEModes.Low_Level_Control
                .ISECommandID = ISECommands.CALB
                .P1 = "0"
                .P2 = "0"
                .P3 = "0"
                .SampleTubePos = 1
                .SampleTubeType = "" 'ISESampleTubeTypes.No_Tube
                .SampleRotorType = 1
                .SampleVolume = 100
            End With

            myMDI.SEND_ISE_COMMAND(myISECommand)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private InitialTaskBarState As TaskBarStates

    Private Sub BsButton4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton4.Click
        Try
            'Dim myUtil As New Utilities.

            Select Case TaskBarState
                Case TaskBarStates.AUTOHIDE, TaskBarStates.AUTOHIDE_ALWAYSONTOP, TaskBarStates.AUTOHIDE_CLOCK, TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK
                    TaskBarAutoHide = False
                   
                Case Else
                    TaskBarAutoHide = True

            End Select

            RefreshTaskbarState()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton5.Click
        Try
            'Dim myUtil As New Utilities.

            Select Case TaskBarState
                Case TaskBarStates.ALWAYSONTOP, TaskBarStates.AUTOHIDE_ALWAYSONTOP, TaskBarStates.ALWAYSONTOP_CLOCK, TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK
                    TaskBarAlwaysOnTop = False
                Case Else
                    TaskBarAlwaysOnTop = True
            End Select

            RefreshTaskbarState()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private Sub RefreshTaskbarState()
        Try
            'Dim myUtil As New Utilities.
            Select Case TaskBarState
                Case TaskBarStates.AUTOHIDE, TaskBarStates.AUTOHIDE_CLOCK
                    BsButton4.Text = "Not Auto Hide"
                    BsButton5.Text = "Always On Top"

                Case TaskBarStates.ALWAYSONTOP, TaskBarStates.ALWAYSONTOP_CLOCK
                    BsButton4.Text = "Auto Hide"
                    BsButton5.Text = "Not Always On Top"

                Case TaskBarStates.AUTOHIDE_ALWAYSONTOP, TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK
                    BsButton4.Text = "Not Auto Hide"
                    BsButton5.Text = "Not Always On Top"
            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Sub





    Private Sub BsButton9_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton9.Click
        Dim myGlobal As New GlobalDataTO
        Try
            '<SER Li 0.93 Na - 0.1 K 2.82 Cl 104.4 0030500b>
            '<URN Na - 1 K 48 Cl 329 00000028>
            '<SER Li  11.00 Na 132.1 K 52.01 Cl 658.1 00000000>
            '<ERC COM T000000>

            Dim myPreparationID As Integer = 17
            Dim myISEResult As String = "<URN Na - 0.1 K 2.82 Cl 104.4 000000EC>" '

            Dim myInstruction As New List(Of InstructionParameterTO)
            Dim myPar1 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar2 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar3 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar4 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar5 As InstructionParameterTO = New InstructionParameterTO

            With myPar1
                .InstructionType = "ANSISE"
                .ParameterIndex = 1
                .ParameterValue = "A400"
            End With
            myInstruction.Add(myPar1)

            With myPar2
                .InstructionType = "ANSISE"
                .ParameterIndex = 2
                .ParameterValue = "ANSISE"
            End With
            myInstruction.Add(myPar2)

            With myPar3
                .InstructionType = "ANSISE"
                .ParameterIndex = 3
                .ParameterValue = myPreparationID.ToString
            End With
            myInstruction.Add(myPar3)

            With myPar4
                .InstructionType = "ANSISE"
                .ParameterIndex = 4
                .ParameterValue = myISEResult
            End With
            myInstruction.Add(myPar4)

            'Dim myAnalyzer As New AnalyzerManager("", "")
            'myGlobal = myAnalyzer.ProcessRecivedISEResult(myInstruction)

            Dim myISEResultTO As New ISEResultTO
            myISEResultTO.ReceivedResults = myISEResult
            '#REFACTORING
            'Dim myAnalyzer As New AnalyzerManager("", "")
            'myAnalyzer.ISE_Manager = New ISEManager(myAnalyzer, "SN0000099999_Ax400", "A400")
            'Dim myISEResultsDelegate As New ISEReception(myAnalyzer)
            'myGlobal = myISEResultsDelegate.ProcessISETESTResultsNEW(Nothing, 22, myISEResultTO, "SimpleMode", "2012032701", "SN0000099999_Ax400")

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton10_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton10.Click
        Dim myGlobal As New GlobalDataTO
        Try

            'Acknowledge
            '<ISE!>

            'calibracion
            '<CAL Li 26.25 Na 369.2 K 88.96 Cl 123.0 000000D\>
            '<CAL Li 26.25 Na 369.2 K 88.96 Cl 123.0 000000D\><CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 000000D\>
            '<CAL Li 26.25 Na 369.2 K 88.96 Cl 123.0 000000D\><ERC URN S000000C>


            '<BMV Li 001.3 Na 895.6 K 006.6 Cl 725.6><AMV Li 258.6 Na 688.7 K 115.8 Cl 789.5><CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 A4F000D\>"

            'serum
            '<SER Li 99.88 Na 77.66 K 55.44 Cl 33.22 000000D\>
            '<SMV Li 023.5 Na 089.6 K 115.9 Cl 723.2><AMV Li 645.3 Na 985.2 K 156.3 Cl 568.9><SER Li 99.88 Na 77.66 K 55.44 Cl 33.22 000000D\>

            'urine 1
            '<URN Na 132.6 K 356.2 Cl 988.6 000000D\>
            '<UMV Na 346.8 K 568.2 Cl 794.3><BMV Na 983.4 K 543.7 Cl 682.8><URN Na 132.6 K 356.2 Cl 988.6 000000D\>

            'Read mv
            '<AMV Li 242.9 Na 216.5 K 184.1 Cl 201.2>

            'Pump Calib
            '<PMC A 3000 B 2000 W 1000>

            'Bubble Calib
            '<BBC A 111 M 222 L 333>

            'Checksum
            '<ISV 7BA9>

            'Read Page 0
            '<DSN 0990BFCE060000A0~><DDT 00 B25000640029057800009C1DA202BC019003208204E2019A344054010E1F13DBã>

            'Read Page 1
            '<DSN 0990BFCE060000A0~><DDT 01 FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFý>


            'ERRORS
            '<ERC URN S000000C>


            Dim myPreparationID As Integer = 0
            Dim myISEResultStr As String = "<CAL Li 26.25 Na 369.2 K 88.96 Cl 123.0 000000D\><CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 000000D\>"

            Me.ISEResultsTextBox.Clear()

            Dim myInstruction As New List(Of InstructionParameterTO)
            Dim myPar1 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar2 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar3 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar4 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar5 As InstructionParameterTO = New InstructionParameterTO

            With myPar1
                .InstructionType = "ANSISE"
                .ParameterIndex = 1
                .ParameterValue = "A400"
            End With
            myInstruction.Add(myPar1)

            With myPar2
                .InstructionType = "ANSISE"
                .ParameterIndex = 2
                .ParameterValue = "ANSISE"
            End With
            myInstruction.Add(myPar2)

            With myPar3
                .InstructionType = "ANSISE"
                .ParameterIndex = 3
                .ParameterValue = myPreparationID.ToString
            End With
            myInstruction.Add(myPar3)

            With myPar4
                .InstructionType = "ANSISE"
                .ParameterIndex = 4
                .ParameterValue = myISEResultStr
            End With
            myInstruction.Add(myPar4)

            '#REFACTORING
            Dim myAnalyzer As IAnalyzerManager = AnalyzerController.Instance.CreateAnalyzer(String.Empty, String.Empty, False, String.Empty, String.Empty, String.Empty) '#REFACTORING

            myGlobal = myAnalyzer.ProcessRecivedISEResult(myInstruction)
            Dim myISEResultData As New ISEResultsDataTO
            Dim myISEResult As ISEResultTO = myAnalyzer.ISEAnalyzer.LastISEResult
            If myISEResult IsNot Nothing Then
                Dim str As New StringBuilder
                With myISEResult
                    str.Append(.ReceivedResults & vbCrLf & vbCrLf)
                    If .ConcentrationValues.HasData Then
                        str.Append("Concentration:" & vbTab & "Li: " & .ConcentrationValues.Li.ToString & vbTab & "Na: " & .ConcentrationValues.Na.ToString & vbTab & "K: " & .ConcentrationValues.K.ToString & vbTab & "Cl: " & .ConcentrationValues.Cl.ToString & vbCrLf)
                    End If
                    If .SerumMilivolts.HasData Then
                        str.Append("Serum (mv):" & vbTab & "Li: " & .SerumMilivolts.Li.ToString & vbTab & "Na: " & .SerumMilivolts.Na.ToString & vbTab & "K: " & .SerumMilivolts.K.ToString & vbTab & "Cl: " & .SerumMilivolts.Cl.ToString & vbCrLf)
                    End If
                    If .UrineMilivolts.HasData Then
                        str.Append("Urine (mv):" & vbTab & "Li: " & .UrineMilivolts.Li.ToString & vbTab & "Na: " & .UrineMilivolts.Na.ToString & vbTab & "K: " & .UrineMilivolts.K.ToString & vbTab & "Cl: " & .UrineMilivolts.Cl.ToString & vbCrLf)
                    End If
                    If .CalibratorAMilivolts.HasData Then
                        str.Append("Calibrator A:" & vbTab & "Li: " & .CalibratorAMilivolts.Li.ToString & vbTab & "Na: " & .CalibratorAMilivolts.Na.ToString & vbTab & "K: " & .CalibratorAMilivolts.K.ToString & vbTab & "Cl: " & .CalibratorAMilivolts.Cl.ToString & vbCrLf)
                    End If
                    If .CalibratorBMilivolts.HasData Then
                        str.Append("Calibrator B:" & vbTab & "Li: " & .CalibratorBMilivolts.Li.ToString & vbTab & "Na: " & .CalibratorBMilivolts.Na.ToString & vbTab & "K: " & .CalibratorBMilivolts.K.ToString & vbTab & "Cl: " & .CalibratorBMilivolts.Cl.ToString & vbCrLf)
                    End If
                    If .CalibrationResults1.HasData Then
                        str.Append("Calib. results 1:" & vbTab & "Li: " & .CalibrationResults1.Li.ToString & vbTab & "Na: " & .CalibrationResults1.Na.ToString & vbTab & "K: " & .CalibrationResults1.K.ToString & vbTab & "Cl: " & .CalibrationResults1.Cl.ToString & vbCrLf)
                    End If
                    If .CalibrationResults2.HasData Then
                        str.Append("Calib. results 2:" & vbTab & "Li: " & .CalibrationResults2.Li.ToString & vbTab & "Na: " & .CalibrationResults2.Na.ToString & vbTab & "K: " & .CalibrationResults2.K.ToString & vbTab & "Cl: " & .CalibrationResults2.Cl.ToString & vbCrLf)
                    End If
                    If .PumpsCalibrationValues.HasData Then
                        str.Append("Pumps calib:" & vbTab & "A: " & .PumpsCalibrationValues.PumpA.ToString & vbTab & "B: " & .PumpsCalibrationValues.PumpB.ToString & vbTab & "W: " & .PumpsCalibrationValues.PumpW.ToString & vbCrLf)
                    End If
                    If .BubbleDetCalibrationValues.HasData Then
                        str.Append("Bubble calib:" & vbTab & "A: " & .BubbleDetCalibrationValues.ValueA.ToString & vbTab & "M: " & .BubbleDetCalibrationValues.ValueM.ToString & vbTab & "L: " & .BubbleDetCalibrationValues.ValueL.ToString & vbCrLf)
                    End If
                    If .ChecksumValue.Trim.Length > 0 Then
                        str.Append("Checksum:" & vbTab & .ChecksumValue & vbCrLf)
                    End If
                    If .DallasSNData IsNot Nothing Then
                        str.Append("Dallas Chip:" & vbTab & .DallasSNData.SNDataString & vbCrLf)
                        str.Append("SerialID:" & vbTab & .DallasSNData.SerialNumber & vbCrLf)
                        str.Append("CRC:" & vbTab & .DallasSNData.CRC & vbCrLf)
                        str.Append(vbCrLf)
                    End If
                    If .DallasPage00Data IsNot Nothing Then
                        str.Append("Dallas Page 00:" & vbTab & vbTab & .DallasPage00Data.Page00DataString & vbCrLf)
                        str.Append("Lot Number:" & vbTab & vbTab & .DallasPage00Data.LotNumber & vbCrLf)
                        str.Append("Expiration Date:" & vbTab & .DallasPage00Data.ExpirationYear.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage00Data.ExpirationMonth.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage00Data.ExpirationDay.ToString.PadRight(2, CChar("0")) & vbCrLf)
                        str.Append("Initial Volume A:" & vbTab & .DallasPage00Data.InitialCalibAVolume.ToString & vbCrLf)
                        str.Append("Initial Volume B:" & vbTab & .DallasPage00Data.InitialCalibBVolume.ToString & vbCrLf)
                        str.Append("Distributor Code:" & vbTab & .DallasPage00Data.DistributorCode & vbCrLf)
                        str.Append("Security Code:" & vbTab & vbTab & .DallasPage00Data.SecurityCode & vbCrLf)
                        str.Append("CRC:" & vbTab & vbTab & .DallasPage00Data.CRC & vbCrLf)
                        str.Append(vbCrLf)
                    End If
                    If .DallasPage01Data IsNot Nothing Then
                        str.Append("Dallas Page 01:" & vbTab & vbTab & .DallasPage01Data.Page01DataString & vbCrLf)
                        str.Append("Consumption A:" & vbTab & .DallasPage01Data.ConsumptionCalA.ToString & vbCrLf)
                        str.Append("Consumption B:" & vbTab & .DallasPage01Data.ConsumptionCalB.ToString & vbCrLf)
                        str.Append("Installation Date:" & vbTab & .DallasPage01Data.InstallationYear.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage01Data.InstallationMonth.ToString.PadRight(2, CChar("0")) & "/" & .DallasPage01Data.InstallationDay.ToString.PadRight(2, CChar("0")) & vbCrLf)
                        str.Append("No good byte:" & vbTab & vbTab & .DallasPage01Data.NoGoodByte & vbCrLf)
                    End If
                    If .Errors IsNot Nothing Then
                        str.Append("ERROR:" & vbTab & vbTab & .ErrorsString & vbCrLf)
                        Dim count As Integer = 1
                        For Each Err As ISEErrorTO In .Errors
                            If Not Err.IsCancelError Then
                                str.Append("Error " & count.ToString & ":" & vbTab)
                            End If

                            If Err.IsCancelError Then
                                If Err.ErrorCycle <> ISEErrorTO.ErrorCycles.None Then
                                    str.Append("Cycle: " & Err.ErrorCycle.ToString & vbTab)
                                End If
                                str.Append("Because of: " & Err.Affected & vbCrLf)
                            Else
                                str.Append("Affected: " & Err.Affected & vbCrLf)
                            End If
                            count += 1
                        Next
                    End If
                    Me.ISEResultsTextBox.Text = str.ToString


                End With

                myISEResultData.ISEResultsData.Add(myISEResult)

            End If

            'SERIALIZE TEST
            If myISEResultData.ISEResultsData.Count > 0 Then
                Dim FS As FileStream
                FS = File.OpenWrite("C:\ISEResults.xml")
                Dim serializer As New XmlSerializer(myISEResultData.GetType)
                serializer.Serialize(FS, myISEResultData)
                FS.Close()
                FS.Dispose()
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton6_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton6.Click

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myISEInfoDelegate As New ISEDelegate
            myGlobal = myISEInfoDelegate.ReadAllInfo(Nothing, "SN0000099999_Ax400")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISEInfoDS As ISEInformationDS = CType(myGlobal.SetDatos, ISEInformationDS)
                Dim str As New StringBuilder
                For Each I As ISEInformationDS.tinfoISERow In myISEInfoDS.tinfoISE.Rows
                    str.Append(I.ISESettingID & vbTab & I.Value & vbCrLf)
                Next
                MessageBox.Show(str.ToString)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton7.Click

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myISEInfoDS As New ISEInformationDS
            Dim myISErow As ISEInformationDS.tinfoISERow

            myISErow = myISEInfoDS.tinfoISE.NewtinfoISERow
            With myISErow
                .BeginEdit()
                .AnalyzerID = "SN0000099999_Ax400"
                .ISESettingID = ISEModuleSettings.AVAILABLE_VOL_CAL_A.ToString
                .Value = (567.8).ToString
                .EndEdit()
            End With
            myISEInfoDS.tinfoISE.AddtinfoISERow(myISErow)
            myISEInfoDS.AcceptChanges()


            myISErow = myISEInfoDS.tinfoISE.NewtinfoISERow
            With myISErow
                .BeginEdit()
                .AnalyzerID = "SN0000099999_Ax400"
                .ISESettingID = ISEModuleSettings.AVAILABLE_VOL_CAL_B.ToString
                .Value = (899.3).ToString
                .EndEdit()
            End With
            myISEInfoDS.tinfoISE.AddtinfoISERow(myISErow)
            myISEInfoDS.AcceptChanges()

            Dim myISEInfoDelegate As New ISEDelegate
            myGlobal = myISEInfoDelegate.UpdateISEInfo(Nothing, "SN0000099999_Ax400", myISEInfoDS)
            If Not myGlobal.HasError Then

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton11_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton11.Click

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myISECalibHisDelegate As New ISECalibHistoryDelegate
            myGlobal = myISECalibHisDelegate.ReadAll(Nothing, "SN0000099999_Ax400")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISECalibHisDS As HistoryISECalibrationsDS = CType(myGlobal.SetDatos, HistoryISECalibrationsDS)
                Dim str As New StringBuilder
                For Each C As HistoryISECalibrationsDS.thisCalibISERow In myISECalibHisDS.thisCalibISE.Rows
                    'JB 25/07/2012 - Deleted column ActionType
                    'str.Append(C.CalibrationID.ToString & vbTab & C.CalibrationDate.ToString & vbTab & C.ConditioningType & vbTab & C.ResultsString & vbTab & C.ActionType & vbCrLf)
                    str.Append(C.CalibrationID.ToString & vbTab & C.CalibrationDate.ToString & vbTab & C.ConditioningType & vbTab & C.ResultsString & vbCrLf)
                Next
                MessageBox.Show(str.ToString)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton12_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton12.Click

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myISECalibHisDelegate As New ISECalibHistoryDelegate
            Dim myISECalibHisDS As New HistoryISECalibrationsDS


            For i As Integer = 0 To 20 Step 1
                Dim myCalibResults As String = ""
                Dim myCalibDate As DateTime = New DateTime(2012, 1, Now.Day - (20 - i), Now.Hour, Now.Minute, Now.Second)

                'JB 25/07/2012 - Adapted to ISEConditioningTypes enum
                'Dim myCalibType As ISECalibrationTypes = CType(CInt(Rnd(3)), ISECalibrationTypes)
                'If myCalibType = ISECalibrationTypes.NONE Then myCalibType = ISECalibrationTypes.ELECTRODES
                Dim values As Array = [Enum].GetValues(GetType(ISEConditioningTypes))
                Dim myCalibType As ISEConditioningTypes = DirectCast(values.GetValue(CInt(Rnd(values.Length))), ISEConditioningTypes)

                Select Case myCalibType
                    Case ISEConditioningTypes.CALB : myCalibResults = "<CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 000000D\>"
                    Case ISEConditioningTypes.PMCL : myCalibResults = "<PMC A 3000 B 2000 W 1000 000000D\>"
                    Case ISEConditioningTypes.BBCL : myCalibResults = "<BBC A 111 M 222 L 333 000000D\>"
                End Select

                myISECalibHisDS.Clear()
                Dim myISECalibHisRow As HistoryISECalibrationsDS.thisCalibISERow
                myISECalibHisRow = myISECalibHisDS.thisCalibISE.NewthisCalibISERow
                With myISECalibHisRow
                    .BeginEdit()
                    .AnalyzerID = "SN0000099999_Ax400"
                    .CalibrationDate = myCalibDate
                    .ConditioningType = myCalibType.ToString
                    .ResultsString = myCalibResults
                    .ErrorsString = ""
                    'JB 25/07/2012 - Deleted ActionType column
                    '.ActionType = ISECalibrationActionTypes.AUTO.ToString
                    .EndEdit()
                End With
                myISECalibHisDS.thisCalibISE.AddthisCalibISERow(myISECalibHisRow)
                myISECalibHisDS.AcceptChanges()

                myGlobal = myISECalibHisDelegate.AddNewCalibration(Nothing, myISECalibHisDS)

            Next i

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton13_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton13.Click
        Dim myISE As New TestISEMonitor
        myISE.Show()
    End Sub


    Private Sub BsButton14_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton14.Click
        Dim ActionButtonCol As New DataGridViewDisableButtonColumn
        ActionButtonCol.ValueType = (New Button).GetType
    End Sub

    Private Sub BsDataGridView1_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles BsDataGridView1.CellPainting
        Dim myGlobal As New GlobalDataTO
        Try
            'Dim myUtil As New Utilities.
            Dim myImage As Image = Nothing
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = GetIconName("ADJUSTMENT")
            If File.Exists(iconPath & auxIconName) Then
                myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myGlobal = ResizeImage(myImage, New Size(16, 16))
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myImage = CType(myGlobal.SetDatos, Image)
                        BsButton14.Image = myImage
                        BsButton14.ImageAlign = ContentAlignment.MiddleLeft
                    End If
                End If

            End If

            If e.ColumnIndex = 1 And e.RowIndex > 0 Then
                Dim celBoton As DataGridViewButtonCell = TryCast(Me.BsDataGridView1.Rows(e.RowIndex).Cells(0), DataGridViewButtonCell)
                If myImage IsNot Nothing Then
                    e.Graphics.DrawImage(myImage, e.CellBounds.Left, e.CellBounds.Top)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton15_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton15.Click
        Try
            Dim msg As New UiMsgBox
            Dim res As DialogResult = msg.ShowMsg("La expresidenta del Consell de Mallorca y de Unió Mallorquina (UM) benefició a una empresa audiovisual. El exvicepresidente, Miquel Nadal, condenado a dos años y siete meses de prisión", "BA400 Service", MessageBoxButtons.OKCancel, MessageBoxIcon.Error)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton16_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton16.Click
        Try
            AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True)

            Application.DoEvents()

            Dim R As Boolean = AnalyzerController.Instance.Analyzer.SynchronizeComm

            'AnalyzerController.Instance.Analyzer.StopComm()

            'Application.DoEvents()

            'AnalyzerController.Instance.Analyzer.RestartComm(False)

            'AnalyzerController.Instance.Analyzer.RestartComm(False)

            Application.DoEvents()

            AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton17_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton17.Click
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myScreenDelegate As MotorsPumpsValvesTestDelegate
            myScreenDelegate = New MotorsPumpsValvesTestDelegate("SN0000099999_Ax400", myFwScriptDelegate)

            Dim myUtilCommand As New UTILCommandTO()
            With myUtilCommand
                .ActionType = UTILInstructionTypes.NeedleCollisionTest
                .CollisionTestActionType = UTILCollisionTestActions.Enable
                .SerialNumberToSave = "0"
                .TanksActionType = UTILIntermediateTanksTestActions.NothingToDo
            End With

            myGlobal = myScreenDelegate.SendUTIL(myUtilCommand)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsButton18_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton18.Click
        Dim myGlobal As New GlobalDataTO
        Try

            Dim myCurrentUtil As UTILInstructionTypes = UTILInstructionTypes.None
            Dim myNeedle As UTILCollidedNeedles = UTILCollidedNeedles.None

            'A400;ANSUTIL;A:2;T:0;C:2;SN:0
            myCurrentUtil = UTILInstructionTypes.NeedleCollisionTest
            myNeedle = UTILCollidedNeedles.DR1

            Dim myInstruction As New List(Of InstructionParameterTO)
            Dim myPar1 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar2 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar3 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar4 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar5 As InstructionParameterTO = New InstructionParameterTO
            Dim myPar6 As InstructionParameterTO = New InstructionParameterTO

            With myPar1
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 1
                .ParameterValue = "A400"
            End With
            myInstruction.Add(myPar1)

            With myPar2
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 2
                .ParameterValue = "ANSUTIL"
            End With
            myInstruction.Add(myPar2)

            With myPar3
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 3
                .ParameterValue = CInt(myCurrentUtil).ToString
            End With
            myInstruction.Add(myPar3)

            With myPar4
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 4
                .ParameterValue = "0"
            End With
            myInstruction.Add(myPar4)

            With myPar5
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 5
                .ParameterValue = CInt(myNeedle).ToString
            End With
            myInstruction.Add(myPar5)

            With myPar6
                .InstructionType = "ANSUTIL"
                .ParameterIndex = 6
                .ParameterValue = "0"
            End With
            myInstruction.Add(myPar6)

            '#REFACTORING
            Dim myAnalyzer As IAnalyzerManager = AnalyzerController.Instance.CreateAnalyzer(String.Empty, String.Empty, False, String.Empty, String.Empty, String.Empty)
            myGlobal = myAnalyzer.ProcessANSUTILReceived(myInstruction)


        Catch ex As Exception
            Throw ex
        End Try
    End Sub


#Region "Testing for ALARMS MANAGEMENT"




    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of UI_RefreshEvents), ByVal pRefreshDs As UIRefreshDS)
        Try

            If pRefreshEventType.Contains(UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                'TODO - implementar en el managereception del MDI
                'ALARM RECEIVED
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE)
                If sensorValue > 0 Then
                    Dim myManageAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE
                    myManageAlarmType = CType(sensorValue, ManagementAlarmTypes)

                    MyClass.ManageAlarmGUITreatment(myManageAlarmType)

                    '1- Disable needed elements

                    '2- Stop current Action (Close screen may be included)

                    '3- Wait for operation is stopped

                    '4- Display FW Update if needed

                    '5- Enable Recover button if needed

                    '6- Enable Exit option


                    ScreenWorkingProcess = False
                    AnalyzerController.Instance.Analyzer.SetSensorValue(AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE) = 0 'Once updated UI clear sensor
                End If


            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".RefreshScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RefreshScreen", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'MDI
    Public IsStoppingActionByAlarm As Boolean

    'MDI called from child
    Public WriteOnly Property IsStopByAlarmFinished(ByVal pAlarmType As ManagementAlarmTypes) As Boolean
        Set(ByVal value As Boolean)
            If value Then
                MyClass.ManageAlarmGUITreatment(pAlarmType)
            End If
        End Set
    End Property

    'MDI
    Public Sub ManageAlarmGUITreatment(ByVal pManageAlarmType As ManagementAlarmTypes)
        Try
            If pManageAlarmType = ManagementAlarmTypes.NONE Then Exit Sub

            If pManageAlarmType <> ManagementAlarmTypes.OMMIT_ERROR Then

                If Not Me.IsStoppingActionByAlarm Then

                    '1 - Disable screen
                    If pManageAlarmType <> ManagementAlarmTypes.SIMPLE_ERROR Then
                        MyClass.PrepareErrorMode() 'child
                    End If

                    '2 - Stop current operation
                    Me.IsStoppingActionByAlarm = True
                    MyClass.StopCurrentOperation(pManageAlarmType) 'child

                    Exit Sub

                Else

                    Me.IsStoppingActionByAlarm = False

                    '3 - Display alarm message
                    MyClass.ShowAlarmOrSensorsWarningMessages(pManageAlarmType) 'MDI

                    '4 - particular treatment
                    If pManageAlarmType = ManagementAlarmTypes.UPDATE_FW Then

                        MyBase.myServiceMDI.ActivateMenus(False)
                        MyBase.myServiceMDI.ActivateActionButtonBar(False)

                        'Show Fw Update
                        'Me.OpenInstrumentUpdateToolScreen(True)
                        MyBase.myServiceMDI.OpenInstrumentUpdateToolScreen(True)
                        'Me.Text = My.Application.Info.ProductName & " - " & InstrumentUpdateToolStripMenuItem.Text

                    ElseIf pManageAlarmType = ManagementAlarmTypes.FATAL_ERROR Then

                        MyBase.myServiceMDI.ActivateMenus(False)
                        MyBase.myServiceMDI.ActivateActionButtonBar(False)

                    ElseIf pManageAlarmType = ManagementAlarmTypes.RECOVER_ERROR Then

                        'TODO - Close current screen 'MDI
                        'TODO - ENABLE RECOVER BUTTON 'MDI

                    ElseIf pManageAlarmType = ManagementAlarmTypes.SIMPLE_ERROR Then

                        'nothing

                    End If
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".ManageAlarmTreatment", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ManageAlarmTreatment", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'MDI ShowAlarmOrSensorsWarningMessages
    Public Sub ShowAlarmOrSensorsWarningMessages(ByVal pAlarmType As ManagementAlarmTypes)
        Try
            Dim myMessage As String = ""
            Select Case pAlarmType
                Case ManagementAlarmTypes.UPDATE_FW : myMessage = Messages.FW_UPDATE.ToString  'TODO pending MESSAGE
                Case ManagementAlarmTypes.FATAL_ERROR : myMessage = Messages.FREEZE_GENERIC_RESET.ToString  'TODO pending MESSAGE
                Case ManagementAlarmTypes.RECOVER_ERROR : myMessage = Messages.FREEZE_GENERIC_RESET.ToString  'TODO pending MESSAGE
                Case ManagementAlarmTypes.SIMPLE_ERROR : myMessage = Messages.FREEZE_GENERIC_AUTO.ToString  'TODO pending MESSAGE
            End Select

            If myMessage.Length > 0 Then

                'Get current Alarms Codes
                Dim myErrorsString As String = AnalyzerController.Instance.Analyzer.ErrorCodes

                myErrorsString = "E34, E456, E51" 'QUITAR

                MyBase.ShowMessage(My.Application.Info.ProductName, myMessage, Messages.SYSTEM_ERROR.ToString, Me, Nothing, myErrorsString)

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".ShowAlarmMessage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowAlarmMessage", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    'Child
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try

            If pAlarmType = ManagementAlarmTypes.UPDATE_FW Or _
                pAlarmType = ManagementAlarmTypes.FATAL_ERROR Or _
                pAlarmType = ManagementAlarmTypes.RECOVER_ERROR Or _
                pAlarmType = ManagementAlarmTypes.SIMPLE_ERROR Then



                'TODO perform Stop operations

                'when finished set IsStopByAlarmFinished of MDI = true

            Else
                'call to MDI in order to follow with alarm process
                MyClass.ManageAlarmGUITreatment(pAlarmType)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".StopCurrentOperation", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".StopCurrentOperation", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'Child
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'each screen must set its elements disabled unless Exit button
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".PrepareErrorMode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareErrorMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

    Private Sub BsButton19_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsButton19.Click
       
        ShowAlarmOrSensorsWarningMessages(ManagementAlarmTypes.SIMPLE_ERROR)
    End Sub

    Private Sub BsToUpper_Click(sender As Object, e As EventArgs) Handles BsToUpper.Click
        Dim s As String = Me.BsToUpperTextBox.Text.ToUpperBS()
        MessageBox.Show(s)
    End Sub

    Private Sub BsCompareButton_Click(sender As Object, e As EventArgs) Handles BsCompareButton.Click
        Dim r As Boolean = Me.BsToUpperTextBox.Text.EqualsBS(Me.BsCompareTextBox.Text)
        MessageBox.Show(r.ToString)
    End Sub
End Class

Module StringExtensions

    <Extension()>
    Public Function ToUpperBS(ByVal pExtendedString As String) As String
        Dim out As String = pExtendedString
        Try
            out = pExtendedString.ToUpperInvariant()
        Catch ex As Exception
            Throw ex
        End Try
        Return out
    End Function

    <Extension()>
    Public Function EqualsBS(ByVal pExtendedString As String, ByVal pString2 As String, Optional ByVal pCaseSensitive As Boolean = False) As Boolean
        Dim out As Boolean = False
        Try
            out = (String.Compare(pExtendedString, pString2, Not pCaseSensitive) = 0)
        Catch ex As Exception
            Throw ex
        End Try
        Return out
    End Function

End Module
