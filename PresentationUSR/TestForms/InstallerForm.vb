Option Strict On
Option Explicit On
Option Infer On


Imports Biosystems.Ax00.Global
Imports System.Windows.Forms
Imports LIS.Biosystems.Ax00.LISCommunications
Imports System.Xml
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Microsoft.Win32
Imports Biosystems.Ax00.Global.GlobalEnumerates

Public Class InstallerForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Public WithEvents MDIAnalyzerManager As AnalyzerManager

    Protected wfPrecarga As Biosystems.Ax00.PresentationCOM.UiAx00StartUp


    Private Sub InstallerForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'ControlEspera()
        'AppVerNumberlbl.Text = My.Application.Info.Version.ToString()
        ''dbVerNumberlbl.Text = ConfigurationManager.AppSettings("DataBaseVersion")
        'dbVerNumberlbl.Text = GlobalBase.DataBaseVersion
        'testLims()

        'Dim myGlobalbase As New GlobalBase
        CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
        ScreenAccessControl()
    End Sub

    Private Sub testLims()
        Try
            Dim myResult As New GlobalDataTO
            'Dim myESWrapperDelegate As New ESWrapper

            'myResult = myESWrapperDelegate.Connect()
            'myResult = myESWrapperDelegate.ReleaseAllChannels(10)
            'myResult = myESWrapperDelegate.GetPendingMessages(nothing)

            'Dim MessageID As String = String.Empty
            'Dim ChannelID As String = String.Empty
            'Dim DateTime As String = String.Empty
            'Dim ApplicationVersion As String = String.Empty
            'Dim AnalyzerModel As String = String.Empty
            'Dim AnalyzerSerialNumber As String = String.Empty

            'Dim myHeaderString As String = ""
            'Dim myBodyString As String = ""
            'Dim myFootherString As String = ""

            'Dim SpecimenIDList As New List(Of String)
            'SpecimenIDList.Add("1")
            'SpecimenIDList.Add("2")
            'SpecimenIDList.Add("3")
            'SpecimenIDList.Add("4")


            'myHeaderString = "<command type=""message"" xmlns=""http://www.nte.es/schema/udc-interface-v1.0"">" & _
            '                 "<header xmlns=""http://www.nte.es/schema/udc-interface-v1.0"">" &
            '                 "<id>" & MessageID & "</id>" & _
            '                 "<channel><id>" & ChannelID & "</id></channel>" & _
            '                 "<metadata><container>" & _
            '                 "<processMode>production</processMode>" & _
            '                 "<priority>2</priority>" & _
            '                 "<transmissionMode>unsolicited</transmissionMode>" & _
            '                 "<action>request</action>" & _
            '                 "<object>workOrder</object>" & _
            '                 "<date>" & DateTime & "</date>" & _
            '                 "</container>" & _
            '                 "</metadata>" & _
            '                 "</header>" & _
            '                 "<body xmlns=""http://www.nte.es/schema/udc-interface-v1.0"">" & _
            '                 "<message>" & _
            '                 "<ci:service xmlns:ci=""http://www.nte.es/schema/clinical-information-v1.0"">" & _
            '                 "<ci:data>order</ci:data>" & _
            '                 "<ci:type>new</ci:type>"

            'For Each SpecimenID As String In SpecimenIDList
            '    myBodyString &= "<ci:specimen set=""particular""/>" & _
            '                        "<ci:id>" & SpecimenID & "</ci:id>" ' & _
            '    '"</ci:specimen>"
            'Next

            'myFootherString = ("</ci:service> <ci:source xmlns:ci=""http://www.nte.es/schema/clinical-information-v1.0"">" & _
            '                   "<ci:companyName>Biosystems</ci:companyName>" & _
            '                   "<ci:OSVersion>" & ApplicationVersion & "</ci:OSVersion>" & _
            '                   "<ci:model>" & AnalyzerModel & "</ci:model>" & _
            '                   "<ci:serialNumber>" & AnalyzerSerialNumber & "</ci:serialNumber>" & _
            '                   "</ci:source>" & _
            '                   "</message>" & _
            '                   "</body>" & _
            '                   "</command>")


            'Dim myXmlDoc As New XmlDocument
            'myXmlDoc.LoadXml(myHeaderString & myBodyString & myFootherString)

            'MessageBox.Show(myXmlDoc.InnerXml)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim myGlobalData As New GlobalDataTO
            Dim myXmlDoc As New XmlDocument
            myXmlDoc.LoadXml(RichTextBox1.Text)
            Dim myEsTranslator As New ESxmlTranslator("1", "A400", "1111")
            myGlobalData = myEsTranslator.DecodeXMLNotification(Nothing, myXmlDoc)
            Dim i As Integer = 0

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub


    Private Sub SaveLISValue()
        Try
            Dim myLISMappingsDS As New LISMappingsDS
            Dim myLISMappingsRow As LISMappingsDS.vcfgLISMappingRow
            Dim myAllTestByTypeDS As New AllTestsByTypeDS
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAllTestByTypeDelegate As New AllTestByTypeDelegate
            Dim myLISMappingsDelegate As New LISMappingsDelegate
            Dim myAllTestByTypeRow As AllTestsByTypeDS.vparAllTestsByTypeRow

            'Mappings
            myLISMappingsRow = myLISMappingsDS.vcfgLISMapping.NewvcfgLISMappingRow()
            myLISMappingsRow.ValueType = "SAMPLE_TYPES"
            myLISMappingsRow.ValueId = "CSF"
            myLISMappingsRow.LISValue = "RRR"
            myLISMappingsDS.vcfgLISMapping.AddvcfgLISMappingRow(myLISMappingsRow)

            'STD
            myAllTestByTypeRow = myAllTestByTypeDS.vparAllTestsByType.NewvparAllTestsByTypeRow()

            myAllTestByTypeRow.TestID = 1
            myAllTestByTypeRow.LISValue = "SER"
            myAllTestByTypeRow.TestType = "STD"

            myAllTestByTypeDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myAllTestByTypeRow)

            'ISE
            myAllTestByTypeRow = myAllTestByTypeDS.vparAllTestsByType.NewvparAllTestsByTypeRow()

            myAllTestByTypeRow.TestID = 1
            myAllTestByTypeRow.LISValue = "SER"
            myAllTestByTypeRow.TestType = "ISE"

            myAllTestByTypeDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myAllTestByTypeRow)

            'CALC
            myAllTestByTypeRow = myAllTestByTypeDS.vparAllTestsByType.NewvparAllTestsByTypeRow()

            myAllTestByTypeRow.TestID = 1
            myAllTestByTypeRow.LISValue = "SER"
            myAllTestByTypeRow.TestType = "CALC"

            myAllTestByTypeDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myAllTestByTypeRow)

            'OFFS
            myAllTestByTypeRow = myAllTestByTypeDS.vparAllTestsByType.NewvparAllTestsByTypeRow()

            myAllTestByTypeRow.TestID = 1
            myAllTestByTypeRow.LISValue = "SER"
            myAllTestByTypeRow.TestType = "OFFS"

            myAllTestByTypeDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myAllTestByTypeRow)

            'myGlobalDataTO = myLISMappingsDelegate.UpdateLISValues(Nothing, myLISMappingsDS, myAllTestByTypeDS)

            If myGlobalDataTO.HasError Then
                MessageBox.Show(myGlobalDataTO.ErrorMessage)
            End If


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Varios_Click(sender As Object, e As EventArgs) Handles Varios.Click

        Try
            LoadLISValues()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub LoadLISValues()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myLISMappingsDelegate As New LISMappingsDelegate
            myGlobalDataTO = myLISMappingsDelegate.ReadAll(Nothing, "ENG")
            If Not myGlobalDataTO.HasError Then
                DataGridView1.DataSource = DirectCast(myGlobalDataTO.SetDatos, LISMappingsDS).vcfgLISMapping
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub SaveLISValues()
        Try

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim myILISUtilities As New UiLISUtilities
        myILISUtilities.ShowDialog()
        'SaveLISValue()
    End Sub


    Private Sub ScreenAccessControl()
        Try
            If CurrentUserLevel = "OPERATOR" Then
                EditButton.Enabled = False
                SaveButton.Enabled = False
                ButtonCancel.Enabled = False

            ElseIf CurrentUserLevel = "SUPERVISOR" Then

            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " ScreenAccessControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub


    Private Sub fillDS()
        Try
            Dim SWOTDAO As New SavedWSOrderTestsDelegate
            Dim mySavedWSDelegate As New SavedWSDelegate

            Dim myGlobalData As New GlobalDataTO
            Dim myDS As New SavedWSOrderTestsDS

            myGlobalData = SWOTDAO.GetOrderTestsBySavedWSID(Nothing, 17)
            If Not myGlobalData.HasError Then
                myDS = DirectCast(myGlobalData.SetDatos, SavedWSOrderTestsDS)
            End If
            For Each Row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In myDS.tparSavedWSOrderTests.Rows
                Row.AwosID = System.Guid.NewGuid.ToString()
                Row.SpecimenID = "123" & Row.SampleID
                Row.ESOrderID = System.Guid.NewGuid.ToString()
                Row.ESPatientID = System.Guid.NewGuid.ToString()
                Row.LISPatientID = Row.SampleID
                Row.SetCreationOrderNull()

            Next

            'System.Guid.NewGuid.ToString()

            myGlobalData = mySavedWSDelegate.SaveFromLIS(Nothing, "NUEVO2", myDS, Now)



            'myGlobalData = SWSD.
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub



#Region "OLD"

    'Private Sub Updateprocess()
    '    'Dim myLogAcciones As New ApplicationLogManager()

    '    Try
    '        Dim mydbmngDelegate As New DataBaseManagerDelegate()
    '        GlobalBase.CreateLogActivity("ANTES DE VALIDAR SI EXISTE LA BD", "InstallerForm", EventLogEntryType.Information, False)

    '        If mydbmngDelegate.DataBaseExist(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword) Then
    '            GlobalBase.CreateLogActivity("EXISTE LA BD", "InstallerForm", EventLogEntryType.Information, False)
    '            Dim myDBUpdateManager As New DataBaseUpdateManagerDelegate
    '            'If myDBUpdateManager.UpdateDatabase(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword) Then
    '            '    MessageBox.Show("Actalizacion de base de datos correcta")
    '            'End If
    '        Else
    '            GlobalBase.CreateLogActivity("ANTES DE INSTALAR LA BD", "InstallerForm", EventLogEntryType.Information, False)
    '            Dim myDBInstallerDelegate As New DataBaseInstallerManagerDelegate()
    '            'If myDBInstallerDelegate.InstallApplicationDataBase(DAOBase.DBServer, DAOBase.CurrentDB, _
    '            '                                                    DAOBase.DBLogin, DAOBase.DBPassword) Then
    '            '    MessageBox.Show("Instalacion de base de datos correcta")
    '            'Else
    '            '    MessageBox.Show("Instalacion de base de datos incorrecta")
    '            'End If
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show(ex.Message)

    '    End Try
    'End Sub

    'Private Sub ControlEspera()
    '    ' Supendemos la logica de diseño
    '    Me.SuspendLayout()
    '    ' Instanciamos el obj Preload para este invocador
    '    wfPrecarga = New Biosystems.Ax00.PresentationCOM.IAx00StartUp(Me)
    '    ' Mostramos
    '    wfPrecarga.Show()
    '    ' Forzamos la logica de diseño
    '    Me.ResumeLayout(True)
    '    ' Obligamos a refrescar el area de diseño inmediatamente
    '    wfPrecarga.Refresh()
    '    ' Finalmente lanzamos el subhilo de ejecucion
    '    bwPrecarga.RunWorkerAsync()
    'End Sub

    'Private Sub InstallUpdateSyncWithInfo()
    '    Dim info As UpdateCheckInfo = Nothing

    '    If (ApplicationDeployment.IsNetworkDeployed) Then
    '        Dim AD As ApplicationDeployment = ApplicationDeployment.CurrentDeployment

    '        Try
    '            info = AD.CheckForDetailedUpdate()
    '        Catch dde As DeploymentDownloadException
    '            MessageBox.Show("The new version of the application cannot be downloaded at this time. " + ControlChars.Lf & ControlChars.Lf & "Please check your network connection, or try again later. Error: " + dde.Message)
    '            Return
    '        Catch ioe As InvalidOperationException
    '            MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " & ioe.Message)
    '            Return
    '        End Try

    '        If (info.UpdateAvailable) Then
    '            Dim doUpdate As Boolean = True

    '            If (Not info.IsUpdateRequired) Then
    '                Dim dr As DialogResult = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButtons.OKCancel)
    '                If (Not System.Windows.Forms.DialogResult.OK = dr) Then
    '                    doUpdate = False
    '                End If
    '            Else
    '                ' Display a message that the app MUST reboot. Display the minimum required version.
    '                MessageBox.Show("This application has detected a mandatory update from your current " & _
    '                    "version to version " & info.MinimumRequiredVersion.ToString() & _
    '                    ". The application will now install the update and restart.", _
    '                    "Update Available", MessageBoxButtons.OK, _
    '                    MessageBoxIcon.Information)
    '            End If

    '            If (doUpdate) Then
    '                Try
    '                    AD.Update()
    '                    MessageBox.Show("The application has been upgraded, and will now restart.")
    '                    System.Windows.Forms.Application.Restart()


    '                Catch dde As DeploymentDownloadException
    '                    MessageBox.Show("Cannot install the latest version of the application. " & ControlChars.Lf & ControlChars.Lf & "Please check your network connection, or try again later.")
    '                    Return
    '                End Try
    '            End If
    '        End If
    '    End If
    'End Sub

    'Private Sub bwPrecarga_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwPrecarga.DoWork
    '    Updateprocess()
    'End Sub

    'Private Sub bwPrecarga_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwPrecarga.RunWorkerCompleted
    '    wfPrecarga.Dispose()
    'End Sub

    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    '    Dim myOrderTestForm As New OrderTest
    '    myOrderTestForm.ShowDialog()
    'End Sub

    'Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
    '    InstallUpdateSyncWithInfo()
    'End Sub

    'Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    'End Sub
#End Region


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        fillDS()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Dim myLIS As New List(Of String)

        myLIS.Add("ASKING,564654464")

        Dim myGlobalData As New GlobalDataTO
        Dim myTesteo As New BarcodePositionsWithNoRequestsDelegate
        myGlobalData = myTesteo.UpdateLISStatus(Nothing, myLIS, "NOINFO")


    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim resultdata As New GlobalDataTO

        Dim myXmlMessages As New xmlMessagesDelegate
        'resultdata = myXmlMessages.CancelAWOSId(Nothing, "94")
        If Not resultdata.HasError Then
            MessageBox.Show("OK")
        ElseIf resultdata.HasError Then
            MessageBox.Show(resultdata.ErrorCode.ToString)
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            Dim myGlobalData As New GlobalDataTO

            'Dim myOrderTestsDelegate As New OrderTestsDelegate
            'myGlobalData = myOrderTestsDelegate.LoadLISSavedWS(Nothing, "834000103", "2013021401", "PENDING", True)

            'myGlobalData = myOrderTestsDelegate.ProcessLISPatientOTs(Nothing, "834000103", "2013031401", myWorSessionResultsDS, mySavedWSOrderTestsDS, _
            '                                                         "KIKO", False, myrejectedDS)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        'Dim Utilities As New Utilities
        Dim myGlobalData As New GlobalDataTO
        'myGlobalData = Utilities.SaveSynapseEventLog("MyEventLog", "C:\Temp\")

        If myGlobalData.HasError Then
            MessageBox.Show(myGlobalData.ErrorMessage)
        Else
            MessageBox.Show("FIN")
        End If

    End Sub

    Private Sub BsButton1_Click(sender As Object, e As EventArgs) Handles BsButton1.Click
        Dim regKey As RegistryKey
        'Dim ver As Decimal
        regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\NTE\communication\3", True)
        regKey.SetValue("traceEnable", &H1001)

        If regKey.GetValue("traceEnable3") Is Nothing Then
            regKey.SetValue("traceEnable3", &H1001, RegistryValueKind.DWord)
        End If

        'ver = CDec(regKey.GetValue("Version", 0.0))
        'If ver < 1.1 Then
        '    regKey.SetValue("Version", 1.1)
        'End If
        regKey.Close()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        ''Copiar el archivo de log del Sysnapse 
        'Dim hEventLog As IntPtr
        'Dim lretv As Integer
        'hEventLog = OpenEventLog(vbNullString, "UDC")
        'If hEventLog = IntPtr.Zero Then
        '    System.Diagnostics.Debug.Write("OpenEvent Log Failed")
        '    Exit Sub
        'End If
        'lretv = BackupEventLog(hEventLog, Application.StartupPath & "\appback1.evt")
        'If lretv = 0 Then
        '    Debug.Write("BackupEventLog Failed")
        '    Exit Sub
        'End If

        'Dim myGlobalData As GlobalDataTO
        Dim myWSReadingDS As New twksWSReadingsDS
        Dim myWSReadingRow As twksWSReadingsDS.twksWSReadingsRow

        myWSReadingRow = myWSReadingDS.twksWSReadings.NewtwksWSReadingsRow

        myWSReadingRow.Pause = True


        Dim mytwksWSReadingDelegate As New WSReadingsDelegate



    End Sub

    Private Sub ClearEventLog()
        Try
            Dim myEventLog As New EventLog
            myEventLog.Log = "UDC"

            Dim i As Integer = 0

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Declare Function BackupEventLog Lib "advapi32.dll" Alias "BackupEventLogA" (ByVal hEventLog As IntPtr, ByVal lpBackupFileName As String) As Integer
    Private Declare Function CloseEventLog Lib "advapi32.dll" (ByVal hEventLog As IntPtr) As IntPtr
    Private Declare Function OpenEventLog Lib "advapi32.dll" Alias "OpenEventLogA" (ByVal lpUNCServerName As String, ByVal lpSourceName As String) As IntPtr

    Private Sub BsButton2_Click(sender As Object, e As EventArgs) Handles BsButton2.Click
        EnableLISWaitTimer(True, 31)
    End Sub

    Public Sub EnableLISWaitTimer(ByVal pStatus As Boolean, Optional pTotalSpecimen As Integer = 0)
        Dim interval As Double = 0

        'TR 16/07/2013
        If pStatus Then
            Dim myResultData As GlobalDataTO
            Dim myUserSettingDelegate As New UserSettingsDelegate

            myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_LIS_WAIT_TIME.ToString())

            If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                interval = 1000 * CType(myResultData.SetDatos, Integer)
            Else
                interval = CDbl(GlobalConstants.AUTO_LIS_WAIT_TIME * 1000)
            End If
            'Multiply by the amount of speciments
            If pTotalSpecimen > 0 Then
                'Get package size
                myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_QUERY_PACKAGE.ToString())
                Dim PackageSize As Integer = 1
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    PackageSize = CType(myResultData.SetDatos, Integer)
                End If

                Dim CalcPakcageAndSpecimen As Single = CSng(pTotalSpecimen / PackageSize)

                Dim IntPart As Integer = CInt(CalcPakcageAndSpecimen)
                'Get the decimal part of the numeber
                Dim DecPart As Single = CalcPakcageAndSpecimen - IntPart
                If DecPart > 0 Then
                    'Add 1 to CalcPackageAndSpeciment
                    CalcPakcageAndSpecimen = IntPart + 1
                Else
                    CalcPakcageAndSpecimen = IntPart
                End If

                interval = interval * CalcPakcageAndSpecimen

                RichTextBox1.Text = interval.ToString()



            End If
        End If
        'TR 16/07/2013 -END.

    End Sub

    'Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
    '    MultilanguageResourcesDelegate.GetResourceText(Nothing, "MSG_CRITICAL_TESTS_PAUSEMODE", )
    '    BSCustomMessageBox.Show(Me, infoCritical, My.Application.Info.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, _
    '                                            "", waitButton, tooltipPause) = DialogResult.Yes Then Return
    'End Sub
End Class