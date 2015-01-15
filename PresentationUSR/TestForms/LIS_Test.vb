Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Configuration
Imports LIS.Biosystems.Ax00.LISCommunications
Imports System.Xml
Imports Biosystems.Ax00.DAL.DAO
Imports System.Threading
Imports System.Net.Sockets
Imports System.Diagnostics

Public Class LIS_Test
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm
    'Private Base As New BSBaseForm

#Region "FormSettings"""

    NotInheritable Class FormSettings
        Inherits ApplicationSettingsBase
        <UserScopedSetting> <DefaultSettingValue("915, 657")> _
        Public Property FormSize() As Size
            Get
                Return DirectCast(Me("FormSize"), Size)
            End Get
            Set(value As Size)
                Me("FormSize") = value
            End Set
        End Property
    End Class

    Private myFormSettings As New FormSettings

#End Region


#Region "Private Constants"
    Private Const UDCSchema As String = "http://www.nte.es/schema/udc-interface-v1.0"
    Private Const ClinicalInfoSchema As String = "http://www.nte.es/schema/clinical-information-v1.0"
#End Region

    Private xmlTranslator As ESxmlTranslator
    Private myWrapper As ESWrapper

    Private myBetaWaterMark As Label
    Public Sub New(Optional ByRef pAnalyzerModel As String = "")
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        MyClass.myBetaWaterMark = New Label
        With MyClass.myBetaWaterMark
            .ForeColor = Color.Black
            .Font = New Font(Me.Font.FontFamily, 50, FontStyle.Bold)
            .Width = 300
            .Height = 200
            .TextAlign = ContentAlignment.MiddleCenter
        End With

        Me.Controls.Add(MyClass.myBetaWaterMark)


    End Sub

    Private Sub Form_Sergio_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        myFormSettings.FormSize = Me.Size
        myFormSettings.Save()
    End Sub



    Private Sub Form_Sergio_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.Size = myFormSettings.FormSize

        'Me.myWrapper = IAx00MainMDI.MDILISManager 'PARA PROBAR DEBE SER PÚBLICA
        'Me.myWrapper = New ESWrapper("TEST", "1", "BA400", "SN0000466432")
        Me.xmlTranslator = New ESxmlTranslator("1", "BA400", "SN0000466432")

        Me.EncodeComboBox.Items.Add("GetAwosAccept")
        Me.EncodeComboBox.Items.Add("GetAwosReject")
        Me.EncodeComboBox.Items.Add("GetAwosResults")
        Me.EncodeComboBox.Items.Add("GetCreateChannel")
        Me.EncodeComboBox.Items.Add("GetDeleteAllMessages")
        Me.EncodeComboBox.Items.Add("GetDeleteIncomingMessages")
        Me.EncodeComboBox.Items.Add("GetDeleteMessage")
        Me.EncodeComboBox.Items.Add("GetHostQuery")
        Me.EncodeComboBox.Items.Add("GetMessageStorage")
        Me.EncodeComboBox.Items.Add("GetPendingMessages")
        Me.EncodeComboBox.Items.Add("GetQueryAll")
        Me.EncodeComboBox.Items.Add("DecodeXMLExceptions")

        Me.DecodeComboBox.Items.Add("DecodeXMLServiceTag")
        Me.DecodeComboBox.Items.Add("DecodeXMLPatientTag")
        Me.DecodeComboBox.Items.Add("DecodeXMLOrderTag")

    End Sub


#Region "LIS"

    Private Sub BsButton1_Click(sender As Object, e As EventArgs) Handles BsButton1.Click
        Dim myUtils As New Utilities
        Dim ResultData As New GlobalDataTO
        Dim myXML As String = ""
        Dim myGuid As String = CStr(myUtils.GetNewGUID.SetDatos)

        If EncodeComboBox.SelectedItem IsNot Nothing Then
            Select Case EncodeComboBox.SelectedItem.ToString
                Case "GetAwosAccept"
                    Dim myAwosIds As New List(Of String)
                    For s As Integer = 1 To 8
                        myAwosIds.Add(System.Guid.NewGuid.ToString())
                    Next
                    If IsWrapper Then
                        ResultData = Me.myWrapper.AcceptCompleteOrder(Nothing)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetAwosAccept(Nothing, System.Guid.NewGuid.ToString(), myAwosIds))
                    End If

                Case "GetAwosReject"
                    Dim myAwosIds As New List(Of String)
                    For s As Integer = 1 To 8
                        myAwosIds.Add(System.Guid.NewGuid.ToString())
                    Next
                    If IsWrapper Then
                        'ResultData = Me.myWrapper.RejectAwos(Nothing, System.Guid.NewGuid.ToString(), myAwosIds)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetAwosReject(Nothing, System.Guid.NewGuid.ToString(), myAwosIds))
                    End If

                Case "GetAwosResults"
                Case "GetCreateChannel"
                    If IsWrapper Then
                        ResultData = Me.myWrapper.CreateChannel(Nothing)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetCreateChannel(Nothing, System.Guid.NewGuid.ToString()))
                    End If


                Case "GetDeleteAllMessages"
                    If IsWrapper Then
                        ResultData = Me.myWrapper.DeleteAllMessages(Nothing)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetDeleteAllMessages(Nothing))
                    End If

                Case "GetDeleteIncomingMessages"

                Case "GetDeleteMessage"
                    If IsWrapper Then
                        ResultData = Me.myWrapper.DeleteMessage(Nothing, System.Guid.NewGuid.ToString())
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetDeleteMessage(Nothing, System.Guid.NewGuid.ToString()))
                    End If

                Case "GetHostQuery"
                    Dim mySpecimes As New List(Of String)
                    For s As Integer = 1 To 8
                        mySpecimes.Add(System.Guid.NewGuid.ToString())
                    Next
                    If IsWrapper Then
                        ' ResultData = Me.myWrapper.HostQuery(Nothing, System.Guid.NewGuid.ToString(), mySpecimes)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetHostQuery(Nothing, System.Guid.NewGuid.ToString(), mySpecimes))
                    End If

                Case "GetMessageStorage"
                    If IsWrapper Then

                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetMessageStorage(Nothing, 99, "C:\OUTPUT", 99, "C:\INPUT"))
                    End If

                Case "GetPendingMessages"
                    If IsWrapper Then
                        ResultData = Me.myWrapper.GetPendingMessages(Nothing)
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetPendingMessages(Nothing))
                    End If

                Case "GetQueryAll"
                    If IsWrapper Then
                        'ResultData = Me.myWrapper.QueryAll(Nothing, System.Guid.NewGuid.ToString())
                    Else
                        myXML = MyClass.GetXmlResult(Me.xmlTranslator.GetQueryAll(Nothing, System.Guid.NewGuid.ToString()))
                    End If

                Case "DecodeXMLExceptions"
                    Me.xmlTranslator.DecodeXMLExceptions(Me.myTextBox.Text)

            End Select

            Me.myTextBox.Text = FormatToXml(myXML)
            Me.myTextBox.Focus()

        End If

    End Sub

    Private Function GetXmlResult(ByRef pmyGlobal As GlobalDataTO) As String
        If Not pmyGlobal.HasError AndAlso pmyGlobal.SetDatos IsNot Nothing Then
            Return CStr(pmyGlobal.SetDatos)
        Else
            Return "ERROR: " & pmyGlobal.ErrorCode & vbCrLf & pmyGlobal.ErrorMessage
        End If
    End Function

    Private Function FormatToXml(ByVal pXmlString As String) As String
        Dim res As String

        Dim XMLText As String = pXmlString

        XMLText = Replace(XMLText, vbTab, "") ' Remove any Tabs from the XML string
        XMLText = Replace(XMLText, "<", vbCrLf & "<") ' Add a Cr/Lf aefore each Start/End section
        XMLText = Replace(XMLText, ">", ">" & vbCrLf) ' Add a Cr/Lf after each Start/End section

        Dim txtDisplay As String = ""
        Dim array() As String
        Dim CurrIndent As Integer

        array = Split(XMLText, vbCrLf) ' Seperate by Cr/Lf
        CurrIndent = 0 ' Set Current Ident to Zero

        With txtDisplay ' txtDisplay is a RichTextBox
            For i As Integer = 0 To array.Length - 1 ' For each XML line
                array(i) = array(i).Trim ' 
                If array(i) <> "" Then ' Ignore blanks
                    If array(i).StartsWith("</") Then ' Decrease Current Indent
                        CurrIndent = CurrIndent - 1
                        txtDisplay &= (StrDup(CurrIndent, vbTab) & array(i) & vbCrLf) ' Print the End Section
                    ElseIf array(i).StartsWith("<") Then ' Start Section
                        txtDisplay &= (StrDup(CurrIndent, vbTab) & array(i) & vbCrLf) ' Print the Start Section
                        If Not array(i).EndsWith("/>") Then
                            CurrIndent = CurrIndent + 1 ' Increae Indent
                        End If
                    Else ' Data Item
                        txtDisplay &= (StrDup(CurrIndent, vbTab) & array(i) & vbCrLf) ' Print the Data Item
                    End If
                End If
            Next i ' Next XML line

        End With

        res = txtDisplay
        Return res
    End Function

#End Region


    Private Sub BsButton3_Click(sender As Object, e As EventArgs) Handles BsButton3.Click

        Dim resultData As New GlobalDataTO

        If Me.myTextBox.Text.Trim.Length > 0 Then
            If DecodeComboBox.SelectedItem IsNot Nothing Then

                'LIS MAPPING DATASET
                '**************************************************************************************
                Dim myLISMappingsDS As New LISMappingsDS
                Dim myLISRow As LISMappingsDS.vcfgLISMappingRow

                myLISRow = myLISMappingsDS.vcfgLISMapping.NewvcfgLISMappingRow()
                With myLISRow
                    .BeginEdit()
                    .ValueType = "SER"
                    .ValueId = "Na"
                    .LISValue = "SodiumLIS"
                    .EndEdit()
                End With
                myLISMappingsDS.vcfgLISMapping.AddvcfgLISMappingRow(myLISRow)

                myLISRow = myLISMappingsDS.vcfgLISMapping.NewvcfgLISMappingRow()
                With myLISRow
                    .BeginEdit()
                    .ValueType = "SER"
                    .ValueId = "Glu"
                    .LISValue = "GlucosaLIS"
                    .EndEdit()
                End With
                myLISMappingsDS.vcfgLISMapping.AddvcfgLISMappingRow(myLISRow)

                myLISRow = myLISMappingsDS.vcfgLISMapping.NewvcfgLISMappingRow()
                With myLISRow
                    .BeginEdit()
                    .ValueType = "URI"
                    .ValueId = "Urea"
                    .LISValue = "UreaLIS"
                    .EndEdit()
                End With
                myLISMappingsDS.vcfgLISMapping.AddvcfgLISMappingRow(myLISRow)

                myLISMappingsDS.AcceptChanges()

                'TEST DATASET ********************************************************************************************
                '
                Dim myTestsDS As New AllTestsByTypeDS
                Dim myTestRow As AllTestsByTypeDS.vparAllTestsByTypeRow

                myTestRow = myTestsDS.vparAllTestsByType.NewvparAllTestsByTypeRow
                With myTestRow
                    .BeginEdit()
                    .TestType = "SER"
                    .TestID = 99
                    .TestName = "Glucose"
                    .LISValue = "GlucoseLISTest"
                    .EndEdit()
                End With
                myTestsDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myTestRow)

                myTestRow = myTestsDS.vparAllTestsByType.NewvparAllTestsByTypeRow
                With myTestRow
                    .BeginEdit()
                    .TestType = "SER"
                    .TestID = 88
                    .TestName = "Ureaaaa"
                    .LISValue = "UreaLISTest"
                    .EndEdit()
                End With
                myTestsDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myTestRow)

                myTestRow = myTestsDS.vparAllTestsByType.NewvparAllTestsByTypeRow
                With myTestRow
                    .BeginEdit()
                    .TestType = "URI"
                    .TestID = 77
                    .TestName = "Sodium"
                    .LISValue = "SodiumLISTest"
                    .EndEdit()
                End With
                myTestsDS.vparAllTestsByType.AddvparAllTestsByTypeRow(myTestRow)

                myTestsDS.AcceptChanges()

                Select Case DecodeComboBox.SelectedItem.ToString
                    Case "DecodeXMLServiceTag"
                        Dim myServiceText As String = Me.myTextBox.Text

                        Dim xmlDoc As XmlDocument = New XmlDocument()

                        '<dummy xmlns:ci="http://www.nte.es/schema/clinical-information-v1.0" >

                        xmlDoc.LoadXml(myServiceText)
                        Dim myServiceNode As XmlNode = xmlDoc.DocumentElement.ChildNodes(0)
                        resultData = Me.xmlTranslator.DecodeXMLServiceTag(myServiceNode, myLISMappingsDS, myTestsDS)
                        If resultData.SetDatos IsNot Nothing Then
                            Dim myRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = TryCast(resultData.SetDatos, SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                            Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS

                            mySavedWSOrderTestsDS.tparSavedWSOrderTests.ImportRow(myRow)
                            mySavedWSOrderTestsDS.AcceptChanges()
                            Dim myContents As String = mySavedWSOrderTestsDS.GetXml
                            If resultData.HasError Then
                                myContents = resultData.ErrorCode.ToString & vbCrLf & myContents
                            End If
                            MessageBox.Show(myContents)

                        End If


                    Case "DecodeXMLPatientTag"
                        Dim myPatientText As String = Me.myTextBox.Text
                        Dim xmlDoc As XmlDocument = New XmlDocument()
                        xmlDoc.LoadXml(myPatientText)
                        Dim myPatientNode As XmlNode = xmlDoc.DocumentElement.ChildNodes(0)

                        resultData = Me.xmlTranslator.DecodeXMLPatientTag(myPatientNode)
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            Dim myPatientsDS As PatientsDS = TryCast(resultData.SetDatos, PatientsDS)
                            If myPatientsDS IsNot Nothing Then
                                Dim myContents As String = myPatientsDS.GetXml
                                MessageBox.Show(myContents)
                            End If
                        ElseIf resultData.HasError Then
                            MessageBox.Show(resultData.ErrorCode.ToString)
                        End If

                    Case "DecodeXMLOrderTag"
                        Dim myOrderText As String = Me.myTextBox.Text
                        Dim xmlDoc As XmlDocument = New XmlDocument()
                        xmlDoc.LoadXml(myOrderText)
                        Dim myOrderNode As XmlNode = xmlDoc.DocumentElement.ChildNodes(0)

                        resultData = Me.xmlTranslator.DecodeXMLOrderTag(myOrderNode)
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            Dim myOrdersDS As OrdersDS = TryCast(resultData.SetDatos, OrdersDS)
                            If myOrdersDS IsNot Nothing Then
                                Dim myContents As String = myOrdersDS.GetXml
                                MessageBox.Show(myContents)
                            End If
                        ElseIf resultData.HasError Then
                            MessageBox.Show(resultData.ErrorCode.ToString)
                        End If

                End Select

            End If
        Else
            MessageBox.Show("Paste the Xml Tag in the TextBox!")
        End If


    End Sub

    Private Sub BsButton2_Click(sender As Object, e As EventArgs) Handles BsButton2.Click

        Dim resultdata As New GlobalDataTO

        Dim myMasterDataDlg As New MasterDataDelegate
        resultdata = myMasterDataDlg.GetList(Nothing, "SAMPLE_TYPES")
        If Not resultdata.HasError AndAlso resultdata.SetDatos IsNot Nothing Then
            Dim myMasterDS As MasterDataDS = CType(resultdata.SetDatos, MasterDataDS)
            myMasterDS.tcfgMasterData.ToList.ForEach(Function(m) ModifyRow(m)) 'recorrer dataset y modificarlo
            myMasterDS.tcfgMasterData.ToList.Insert(myMasterDS.tcfgMasterData.Count, myMasterDS.tcfgMasterData.NewtcfgMasterDataRow) 'insertar fila
        End If



        Dim myXmlMessages As New xmlMessagesDelegate
        resultdata = myXmlMessages.CancelAWOSID(Nothing, "79", "AnalyzerID", "WorkSessionID")
        If Not resultdata.HasError Then
            MessageBox.Show("OK")
        ElseIf resultdata.HasError Then
            MessageBox.Show(resultdata.ErrorCode.ToString)
        End If

    End Sub

    Private Function ModifyRow(ByVal m As MasterDataDS.tcfgMasterDataRow) As MasterDataDS.tcfgMasterDataRow
        m.Description = "XXXXX"
        m.TS_DateTime = DateTime.Now
        Return m
    End Function

    Private IsWrapper As Boolean = False
    Private Sub WrapperCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles WrapperCheckBox.CheckedChanged
        IsWrapper = Me.WrapperCheckBox.Checked

    End Sub

    Private Sub BsButton5_Click(sender As Object, e As EventArgs) Handles BsButton5.Click

        Dim resultData As New GlobalDataTO
        Dim myOTD As New OrderTestsDelegate

        Try


            'data perparation

            Dim mySavedWSID As Integer = 2
            Dim myRerunLISMode As String = "BIO" ' "ANALYZER"
            Dim myTestType As String = "STD"

            Dim myWorkSessionResultDS As New WorkSessionResultDS
            Dim mySavedWSOrderTestsDAO As New tparSavedWSOrderTestsDAO
            resultData = mySavedWSOrderTestsDAO.ReadBySavedWSID(Nothing, mySavedWSID)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                resultData = myOTD.GetPatientOrderTests(Nothing, IAx00MainMDI.ActiveWorkSession)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)


                    Dim myPatientOrderTestList As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                    myPatientOrderTestList = (From a In mySavedWSOrderTestsDS.tparSavedWSOrderTests _
                                              Where a.SampleClass = "PATIENT" _
                                              And a.TestType = myTestType _
                                              Select a).ToList()

                    For Each SavedWSOTRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In myPatientOrderTestList


                        Dim myPatientOTOnActiveWSList As List(Of WorkSessionResultDS.PatientsRow)
                        myPatientOTOnActiveWSList = (From a In myWorkSessionResultDS.Patients Where a.SampleClass = "PATIENT" _
                                                            AndAlso a.SampleID = SavedWSOTRow.SampleID _
                                                            AndAlso a.TestType = SavedWSOTRow.TestType _
                                                            AndAlso a.TestID = SavedWSOTRow.TestID _
                                                            AndAlso a.SampleType = SavedWSOTRow.SampleType _
                                                            Select a).ToList()



                        If myPatientOTOnActiveWSList.Count > 0 Then

                            Dim myFinalOrderTestsLISInfoDS As New OrderTestsLISInfoDS
                            Dim myRepetitionsToAddDS As New WSRepetitionsToAddDS

                            'Dim myRepetitionRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = myRepetitionsToAddDS.twksWSRepetitionsToAdd.NewtwksWSRepetitionsToAddRow
                            'With myRepetitionRow
                            '    .BeginEdit()
                            '    .WorkSessionID = IAx00MainMDI.ActiveWorkSession
                            '    .AnalyzerID = IAx00MainMDI.ActiveAnalyzer
                            '    .WorkSessionID = IAx00MainMDI.ActiveWorkSession
                            '        .OrderTestID = myPatientOTOnActiveWSList.First.OrderTestID
                            '    .SampleClass = "PATIENT"
                            '    .RerunNumber = 1
                            '    .EndEdit()
                            'End With
                            'myRepetitionsToAddDS.twksWSRepetitionsToAdd.AddtwksWSRepetitionsToAddRow(myRepetitionRow)
                            'myRepetitionsToAddDS.AcceptChanges()

                            resultData = myOTD.VerifyRerunOfLISPatientOT(myPatientOTOnActiveWSList.First, SavedWSOTRow, myRerunLISMode, myFinalOrderTestsLISInfoDS, _
                                                                         myRepetitionsToAddDS, Now)

                            'resultData = myOTD.VerifyRerunOfManualPatientOT(myPatientOTOnActiveWSList.First, _
                            '                                                SavedWSOTRow, _
                            '                                                myRerunLISMode, _
                            '                                                myFinalOrderTestsLISInfoDS, _
                            '                                                myRepetitionsToAddDS)

                            If Not resultData.HasError Then
                                If myRepetitionsToAddDS.twksWSRepetitionsToAdd.Count > 0 Then



                                ElseIf myFinalOrderTestsLISInfoDS.twksOrderTestsLISInfo.Count > 0 Then



                                End If
                            Else
                                MessageBox.Show(SavedWSOTRow.TestType & ": " & resultData.ErrorCode.ToString)
                            End If
                        End If

                    Next
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub


#Region "EventLogMonitor"

    Private objLog As EventLog
    Private m_ListenerMonitorPort As Integer
    Private m_objListener_Monitor As System.Net.Sockets.TcpListener
    Private listeningThread As Thread

    Private Sub BsButton6_Click(sender As Object, e As EventArgs) Handles BsButton6.Click

        Me.myTextBox.Font = New Font(Me.myTextBox.Font.FontFamily, 8)

        'attach event handler: so we can monitor local events
        'we cannot monitor events on remote computer this way :
        'http://support.microsoft.com/?scid=kb;EN;815314
        'Receive Event Notifications
        'You can receive event notification when an entry is written to a particular log. To do this, implement the EntryWritten event handler for the instance of the EventLog. Also, set EnableRaisingEvents to true.
        'Note You can only receive event notifications when entries are written on the local computer. You cannot receive notifications for entries that are written on remote computers.

        objLog = New EventLog("UDC")
        AddHandler objLog.EntryWritten, AddressOf ApplicationLog_OnEntryWritten
        objLog.EnableRaisingEvents = True

        listeningThread = New Threading.Thread(AddressOf ListenForWatchers)
        listeningThread.Start(Me)

    End Sub

    Public Sub ApplicationLog_OnEntryWritten(ByVal [source] As Object, ByVal e As EntryWrittenEventArgs)
        Try


            Dim myText As String = e.Entry.EntryType.ToString & " - " & e.Entry.TimeGenerated.ToString("dd/MM/yy HH:mm:ss")
            myText &= vbCrLf & e.Entry.Message & vbCrLf

            Me.UIThread(Function() DisplayEntry(myText))

            If e.Entry.EntryType = EventLogEntryType.Error Then
                Dim a As Integer = 0
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Function DisplayEntry(ByVal pEntry As String) As Boolean
        Me.myTextBox.Text &= vbCrLf & pEntry
    End Function

    Public Sub ListenForWatchers(ByVal objState As Object)
        Dim objUI As LIS_Test

        Try
            objUI = CType(objState, LIS_Test)

            m_objListener_Monitor = New TcpListener(New System.Net.IPAddress(0), 1) 'm_ListenerMonitorPort
            m_objListener_Monitor.Start()

            Do

                Dim objClient As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                objClient = m_objListener_Monitor.AcceptSocket()

                Do While objClient.Available = 0
                    'wait...
                    If Not objClient.Connected Then
                        Throw New System.Exception("!Did not receive data!Or Not Connected")
                    End If
                Loop

                If objClient.Available > 0 Then
                    Dim InBytes(objClient.Available) As Byte
                    objClient.Receive(InBytes, objClient.Available, SocketFlags.None)
                Else
                    'TODO
                End If
            Loop Until False


        Catch err As Exception
            MessageBox.Show(err.Message)
        End Try
    End Sub

    Private Sub BsButton7_Click(sender As Object, e As EventArgs) Handles BsButton7.Click

        If Not m_objListener_Monitor Is Nothing Then
            Try
                listeningThread = Nothing
                m_objListener_Monitor = Nothing
                RemoveHandler objLog.EntryWritten, AddressOf ApplicationLog_OnEntryWritten
                objLog.Dispose()
                objLog = Nothing

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End If
    End Sub

#End Region

#Region "File Watcher"

    Private myFileWatcher As New BSFileSystemWatcher
    Private myCreateFileCount As Integer = 0
    Private myDeleteFileCount As Integer = 0

    Private Sub BsButton8_Click(sender As Object, e As EventArgs) Handles BsButton8.Click

        Me.myTextBox.Font = New Font(Me.myTextBox.Font.FontFamily, 8)

        MyClass.myCreateFileCount = 0
        MyClass.myDeleteFileCount = 0
        Me.lblCreateFileCount.Text = MyClass.myCreateFileCount.ToString
        Me.lblDeleteFileCount.Text = MyClass.myDeleteFileCount.ToString

        myFileWatcher.Path = "C:\Users\Sergio Garcia\Documents\BAx00 v1.1\AX00\PresentationUSR\bin\x86\Debug\Storage"

        myFileWatcher.NotifyFilter = IO.NotifyFilters.DirectoryName
        myFileWatcher.NotifyFilter = myFileWatcher.NotifyFilter Or IO.NotifyFilters.FileName
        myFileWatcher.NotifyFilter = myFileWatcher.NotifyFilter Or IO.NotifyFilters.Attributes

        ' add the handler to each event
        AddHandler myFileWatcher.Changed, AddressOf logchange
        AddHandler myFileWatcher.Created, AddressOf logchange
        AddHandler myFileWatcher.Deleted, AddressOf logchange

        ' add the rename handler as the signature is different
        AddHandler myFileWatcher.Renamed, AddressOf logrename

        'Set this property to true to start watching
        myFileWatcher.EnableRaisingEvents = True


    End Sub

    Private Sub logchange(ByVal source As Object, ByVal e As System.IO.FileSystemEventArgs)
        Dim myText As String = ""
        If e.ChangeType = IO.WatcherChangeTypes.Changed Then
            myText = "[" & e.Name & "]" & " modified" & vbCrLf
        End If
        If e.ChangeType = IO.WatcherChangeTypes.Created Then
            myText = "[" & e.Name & "]" & " created" & vbCrLf
            MyClass.myCreateFileCount += 1
        End If
        If e.ChangeType = IO.WatcherChangeTypes.Deleted Then
            myText = "[" & e.Name & "]" & " deleted" & vbCrLf
            MyClass.myDeleteFileCount -= 1
        End If

        Me.UIThread(Function() DisplayFileChanges(myText))

    End Sub

    Public Sub logrename(ByVal source As Object, ByVal e As System.IO.RenamedEventArgs)
        Dim myText As String = ""
        myText = "[" & e.OldName & "]" & " renamed to " & "[" & e.Name & "]"
        Me.UIThread(Function() DisplayFileChanges(myText))
    End Sub

    Private Function DisplayFileChanges(ByVal pChange As String) As Boolean
        Me.myTextBox.Text &= vbCrLf & Now.ToString("HH:mm:ss:fff") & " - " & pChange
        Me.lblCreateFileCount.Text = MyClass.myCreateFileCount.ToString
        Me.lblDeleteFileCount.Text = MyClass.myDeleteFileCount.ToString
    End Function

#End Region

End Class


