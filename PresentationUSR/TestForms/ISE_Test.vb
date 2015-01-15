Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class ISE_Test
    Private mOnlyTreated As Boolean = False

    Private Sub butClearReceived_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butClearReceived.Click
        txtReceivedData.Clear()
    End Sub

    Private Sub butClearProcessed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butClearProcessed.Click
        txtProcessedData.Clear()
    End Sub

    Private Sub butProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butProcess.Click
        Try
            'Simulate instruction reception
            If txtReceivedData.Text.Trim <> "" Then
                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myGlobal As New GlobalDataTO

                    Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                    If myAnalyzerManager.CommThreadsStarted Then
                        'Short instructions
                        myGlobal = myAnalyzerManager.SimulateInstructionReception(txtReceivedData.Text.Trim)

                        'Me.DataGridView1.DataSource = DirectCast (myGlobal.SetDatos, 
                    End If
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Delegate Sub UpdateResponseDelegate(ByVal text As String)
    Public Sub UpdateResponse(ByVal text As String)
        If Me.InvokeRequired Then
            Dim updDelegate As New UpdateResponseDelegate(AddressOf UpdateResponse)
            Me.Invoke(updDelegate, New Object() {text})
        Else
            txtProcessedData.Text &= ">> " & text & vbCrLf
            'Ir a ultima linea
            txtProcessedData.SelectionStart = txtProcessedData.Text.Length
            txtProcessedData.ScrollToCaret()
        End If
    End Sub
#Region "Communications Board Testings & Simulate Send Next Preparation"
    Private WithEvents myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

    Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles myAnalyzerManager.ReceptionEvent
        Try
            If mOnlyTreated AndAlso pTreated Then
                UpdateResponse(pInstructionReceived)
            ElseIf Not mOnlyTreated Then
                UpdateResponse(pInstructionReceived)
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub

    Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles myAnalyzerManager.SendEvent
        Try
            txtProcessedData.Text &= pInstructionSent & vbCrLf

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try
    End Sub
#End Region

    Private Function GetISEAction() As ISEManager.ISEProcedures
        If myAnalyzerManager.ISE_Manager.CurrentCommandTO Is Nothing Then myAnalyzerManager.ISE_Manager.CurrentCommandTO = New ISECommandTO

        If cmbISEAction.SelectedItem Is Nothing Then Return ISEManager.ISEProcedures.None

        'Retrieve value of the selected item. 
        Dim enumType As Type = GetType(ISEManager.ISEProcedures)
        Dim selection As String = DirectCast(cmbISEAction.SelectedItem, String)
        Dim value As ISEManager.ISEProcedures = DirectCast([Enum].Parse(enumType, selection), ISEManager.ISEProcedures)
        myAnalyzerManager.ISE_Manager.CurrentProcedure = value

        If value = ISEManager.ISEProcedures.CalibrateBubbles Then
            myAnalyzerManager.ISE_Manager.CurrentProcedure = ISEManager.ISEProcedures.SingleReadCommand
            myAnalyzerManager.ISE_Manager.CurrentCommandTO.ISECommandID = GlobalEnumerates.ISECommands.BUBBLE_CAL
            'myAnalyzerManager.ISE_Manager.LastISEResult = New ISEResultTO With {.ISEResultType = ISEResultTO.ISEResultTypes.BBC}
            'myAnalyzerManager.ISE_Manager.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BBC
        End If
        If value = ISEManager.ISEProcedures.Clean Then
            myAnalyzerManager.ISE_Manager.CurrentCommandTO.ISECommandID = GlobalEnumerates.ISECommands.CLEAN
        End If

        Return value
    End Function
    Private Sub cmbISEAction_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbISEAction.SelectedIndexChanged
        Dim value As ISEManager.ISEProcedures = GetISEAction()

        grpCalib1.Visible = (value = ISEManager.ISEProcedures.CalibrateElectrodes)
        grpCalib2.Visible = (value = ISEManager.ISEProcedures.CalibrateElectrodes)

        grpPumps.Visible = (value = ISEManager.ISEProcedures.CalibratePumps)

        grpBubbles.Visible = (value = ISEManager.ISEProcedures.CalibrateBubbles)

        chkERC.Checked = False
    End Sub

    Private Sub txtReceivedData_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtReceivedData.TextChanged
        butClearReceived.Enabled = Not String.IsNullOrEmpty(txtReceivedData.Text)
        butProcess.Enabled = Not String.IsNullOrEmpty(txtReceivedData.Text)
    End Sub

    Private Sub txtProcessedData_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtProcessedData.TextChanged
        txtProcessedData.Enabled = Not String.IsNullOrEmpty(txtProcessedData.Text)
        butClearProcessed.Enabled = Not String.IsNullOrEmpty(txtProcessedData.Text)
    End Sub


    Private Function GetCalibrationsString() As String
        Dim str As String = ""
        If Not chkCalib1.Checked Then Return str
        str &= "<CAL Li " & txtCalib1Li.Text & " Na " & txtCalib1Na.Text & " K " & txtCalib1K.Text & " Cl " & txtCalib1Cl.Text & " " & txtCalib1Res.Text & "\>"

        If Not chkCalib2.Checked Then Return str
        str &= "<CAL Li " & txtCalib2Li.Text & " Na " & txtCalib2Na.Text & " K " & txtCalib2K.Text & " Cl " & txtCalib2Cl.Text & " " & txtCalib2Res.Text & "\>"

        Return str
    End Function

    Private Function GetPumpsString() As String
        Dim str As String = ""
        If Not chkPumps.Checked Then Return str
        '<PMC A 2162 B 2187 W 2229>
        str &= "<PMC A " & txtPumpsA.Text & " B " & txtPumpsB.Text & " W " & txtPumpsW.Text & ">"
        Return str
    End Function

    Private Function GetBubblesString() As String
        Dim str As String = ""
        If Not chkBubbles.Checked Then Return str
        '<BBC A 196 M 107 L 019>
        str &= "<BBC A " & txtBubblesA.Text & " M " & txtBubblesM.Text & " L " & txtBubblesL.Text & ">"
        Return str
    End Function

    Private Function GetERCOperationString() As String
        Select Case GetISEAction()
            Case ISEManager.ISEProcedures.Clean
                Return "CLE"
            Case ISEManager.ISEProcedures.CalibrateBubbles
                Return "BBC"
            Case ISEManager.ISEProcedures.CalibrateElectrodes
                Return "CAL"
            Case ISEManager.ISEProcedures.CalibratePumps
                Return "PMC"
        End Select
        Return ""
    End Function

    Private Function GetERCCode() As String
        If cmbERCCode.SelectedItem Is Nothing Then Return "N" '"0"
        Dim str As String = cmbERCCode.SelectedItem.ToString.Substring(0, 1)
        If str = "0" Then str = "N"
        Return str
    End Function

    Private Function GetERCString() As String
        '<ERC CAL W000300>
        Dim str As String = ""
        If Not chkERC.Checked Then Return str
        str &= "<ERC " & GetERCOperationString() & " " & GetERCCode() & IIf(txtERCRes.Enabled, txtERCRes.Text, "000000").ToString & ">"

        Return str
    End Function

    Private Function GetMessageToSend() As String
        Dim msg As String = "A400;ANSISE;P:0;R:"
        Select Case GetISEAction()
            Case ISEManager.ISEProcedures.Clean
                If Not chkERC.Checked Then msg &= "<ISE!>"

            Case ISEManager.ISEProcedures.CalibrateBubbles
                msg &= GetBubblesString()

            Case ISEManager.ISEProcedures.CalibrateElectrodes
                msg &= GetCalibrationsString()

            Case ISEManager.ISEProcedures.CalibratePumps
                msg &= GetPumpsString()

        End Select
        msg &= GetERCString()
        msg &= ";"
        Return msg
    End Function

    Private Sub butGenerateProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butGenerateProcess.Click
        GetISEAction()
        txtReceivedData.Text = GetMessageToSend()
        Me.tabMain.SelectedIndex = 1
        'butProcess.PerformClick()
    End Sub


    Private Sub chkERC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkERC.CheckedChanged
        lblERCCode.Enabled = chkERC.Checked
        cmbERCCode.Enabled = chkERC.Checked
        lblERCRes.Enabled = chkERC.Checked AndAlso (GetERCCode() = "0")
        txtERCRes.Enabled = chkERC.Checked AndAlso (GetERCCode() = "0")
        If GetISEAction() = ISEManager.ISEProcedures.CalibrateBubbles Then
            chkBubbles.Checked = Not chkERC.Checked
        End If
    End Sub
    Private Sub cmbERCCode_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbERCCode.SelectedIndexChanged
        lblERCRes.Enabled = chkERC.Checked AndAlso (GetERCCode() = "0")
        txtERCRes.Enabled = chkERC.Checked AndAlso (GetERCCode() = "0")
    End Sub

    Private Sub chkCalib1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCalib1.CheckedChanged
        lblCalib1Li.Enabled = chkCalib1.Checked
        lblCalib1Na.Enabled = chkCalib1.Checked
        LblCalib1K.Enabled = chkCalib1.Checked
        lblCalib1Cl.Enabled = chkCalib1.Checked
        txtCalib1Li.Enabled = chkCalib1.Checked
        txtCalib1Na.Enabled = chkCalib1.Checked
        txtCalib1K.Enabled = chkCalib1.Checked
        txtCalib1Cl.Enabled = chkCalib1.Checked
        lblCalib1Res.Enabled = chkCalib1.Checked
        txtCalib1Res.Enabled = chkCalib1.Checked

        If Not chkCalib1.Checked AndAlso GetISEAction() = ISEManager.ISEProcedures.CalibrateElectrodes Then
            chkCalib2.Checked = False
            chkERC.Checked = True
        End If
        chkCalib2.Enabled = chkCalib1.Checked
    End Sub

    Private Sub chkCalib2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCalib2.CheckedChanged
        lblCalib2Li.Enabled = chkCalib2.Checked
        lblCalib2Na.Enabled = chkCalib2.Checked
        lblCalib2K.Enabled = chkCalib2.Checked
        lblCalib2Cl.Enabled = chkCalib2.Checked
        txtCalib2Li.Enabled = chkCalib2.Checked
        txtCalib2Na.Enabled = chkCalib2.Checked
        txtCalib2K.Enabled = chkCalib2.Checked
        txtCalib2Cl.Enabled = chkCalib2.Checked
        lblCalib2Res.Enabled = chkCalib2.Checked
        txtCalib2Res.Enabled = chkCalib2.Checked
        If Not chkCalib2.Checked AndAlso GetISEAction() = ISEManager.ISEProcedures.CalibrateElectrodes Then
            chkERC.Checked = True
        End If
    End Sub

    Private Sub grpCalib1_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles grpCalib1.VisibleChanged
        chkCalib1.Checked = grpCalib1.Visible
    End Sub

    Private Sub grpCalib2_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles grpCalib2.VisibleChanged
        chkCalib2.Checked = grpCalib2.Visible
    End Sub

    Private Sub grpPumps_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles grpPumps.VisibleChanged
        chkPumps.Checked = grpPumps.Visible
    End Sub

    Private Sub chkBubbles_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBubbles.VisibleChanged
        chkBubbles.Checked = grpBubbles.Visible
    End Sub

    Private Sub chkBubbles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBubbles.CheckedChanged
        lblBubblesA.Enabled = chkBubbles.Checked
        lblBubblesL.Enabled = chkBubbles.Checked
        lblBubblesM.Enabled = chkBubbles.Checked
        txtBubblesA.Enabled = chkBubbles.Checked
        txtBubblesL.Enabled = chkBubbles.Checked
        txtBubblesM.Enabled = chkBubbles.Checked
        If GetISEAction() = ISEManager.ISEProcedures.CalibrateBubbles Then
            chkERC.Checked = Not chkBubbles.Checked
        End If
    End Sub

    Private Sub chkPumps_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPumps.CheckedChanged
        lblPumpsA.Enabled = chkPumps.Checked
        lblPumpsB.Enabled = chkPumps.Checked
        lblPumpsW.Enabled = chkPumps.Checked
        txtPumpsA.Enabled = chkPumps.Checked
        txtPumpsB.Enabled = chkPumps.Checked
        txtPumpsW.Enabled = chkPumps.Checked
        If Not chkPumps.Checked AndAlso GetISEAction() = ISEManager.ISEProcedures.CalibratePumps Then
            chkERC.Checked = True
        End If
    End Sub

    Private Sub txtRes_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtERCRes.KeyDown, txtCalib1Res.KeyDown, txtCalib2Res.KeyDown
        If e.KeyCode > Keys.F AndAlso _
           e.KeyCode <= Keys.Z AndAlso _
           Not e.Control AndAlso _
           Not e.Alt Then
            'The user has pressed a letter key greater than F, which would be allowed by the mask, so reject it.
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub chkOnlyTreated_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkOnlyTreated.CheckedChanged
        mOnlyTreated = chkOnlyTreated.Checked
    End Sub

    Private Sub butHistAlarms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butHistAlarms.Click
        Dim histoFrm As New Historycal
        histoFrm.Show()

    End Sub

    Private Sub butTestGrid_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butTestGrid.Click
        Dim grdForm As New FormStressGrids
        grdForm.Show()
    End Sub


End Class